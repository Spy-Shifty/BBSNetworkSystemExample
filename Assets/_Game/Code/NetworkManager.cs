using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class NetworkManager : INetworkManager {
    private readonly LoadBalancingClient client = new LoadBalancingClient();
    public string AppId = "";
    public string AppVersion = "";

    public Player LocalPlayer => client.LocalPlayer;
    public int LocalPlayerID { get; private set; }

    public bool IsMaster => client.LocalPlayer.IsMasterClient;
    public ClientState State => client.State;
    public int PlayersInRoomCount => client.PlayersInRoomsCount;
    public int RoomsCount => client.RoomsCount;
    public int PlayersOnMasterServerCount => client.PlayersOnMasterCount;
    public bool IsConnected => client.IsConnected;
    public bool IsConnectedAndReady => client.IsConnectedAndReady;
    public int LastSendMessageSize { get; private set; }
    public int LastReceivedMessageSize { get; private set; }


    public event EventDataDelegate OnEventData;
    public event PlayerJoinedDelegate OnPlayerJoined;
    public event PlayerLeftDelegate OnPlayerLeft;
    public event Action OnJoinedLobby;
    public event Action OnJoinedRoom;
    public event Action OnLeftRoom;
    public event Action OnJoinedGame;
    public event Action OnConnectedToGameServer;
    public event Action OnConnectedToMasterServer;
    public event Action OnDisconnectedFromMasterServer;
    public event Action OnDisconnected;

    private readonly byte[] AllGroupsArray = new byte[0];

    private Dictionary<byte, List<Action<int, object>>> dataReceiverMap = new Dictionary<byte, List<Action<int, object>>>();
    private int networkIdCounter = 0;
    private Queue<int> freeNetworkIds = new Queue<int>();
    private RaiseEventOptions eventOption = RaiseEventOptions.Default;


    static NetworkManager instance;
    internal static NetworkManager Instance {
        get {
            if(instance == null) {
                instance = new NetworkManager();
            }
            return instance;
        }
    }

    public void RegisterForDataEvent(byte eventId, Action<int, object> dataReceiveEvent) {
        List<Action<int, object>> dataReceiveEvents;
        if (!dataReceiverMap.TryGetValue(eventId, out dataReceiveEvents)) {
            dataReceiveEvents = new List<Action<int, object>>();
            dataReceiverMap.Add(eventId, dataReceiveEvents);
        }
        dataReceiveEvents.Add(dataReceiveEvent);       
    }

    public void UnregisterForDataEvent(byte eventId, Action<int, object> dataReceiveEvent) {
        List<Action<int, object>> dataReceiveEvents;
        if (dataReceiverMap.TryGetValue(eventId, out dataReceiveEvents)) {
            dataReceiveEvents.Remove(dataReceiveEvent);
            if (!dataReceiveEvents.Any()) {
                dataReceiverMap.Remove(eventId);
            }
        }
    }

    private void HandleDataEvents(byte eventId, Player player, object data) {
        List<Action<int, object>> dataReceiveEvents;
        if (dataReceiverMap.TryGetValue(eventId, out dataReceiveEvents)) {
            foreach (var dataEvent in dataReceiveEvents) {
                try {
                    dataEvent.Invoke(player.ID, data);
                }catch(Exception ex) {
                    Debug.LogError(ex.Message + "\n" + ex.StackTrace);
                }
            }
        }
    }

    private NetworkManager() {
        client.OnEventAction += Client_OnEventAction;
        client.OnOpResponseAction += Client_OnOpResponseAction;
        client.OnStateChangeAction += Client_OnStateChangeAction;
    }

    public void Connect() {
        client.AppId = AppId;
        client.AppVersion = AppVersion;
        client.LocalPlayer.NickName = "usr" + SupportClass.ThreadSafeRandom.Next() % 99;
        client.AutoJoinLobby = false;

        AutoConnectToMaster();
    }

    public void Disconnect() {
        client.Disconnect();
    }

    private void Client_OnStateChangeAction(ClientState clientState) {
        switch (clientState) {
            case ClientState.JoinedLobby:
                LocalPlayerID = LocalPlayer.ID;
                OnJoinedLobby?.Invoke();
                break;
            case ClientState.DisconnectingFromMasterserver:
                OnDisconnectedFromMasterServer?.Invoke();
                break;
            case ClientState.ConnectedToGameserver:
                OnConnectedToGameServer?.Invoke();
                break;
            case ClientState.Joining:
                OnJoinedGame?.Invoke();
                break;
            case ClientState.Joined:
                LocalPlayerID = LocalPlayer.ID;
                OnJoinedRoom?.Invoke();
                break;
            case ClientState.Leaving:
                OnLeftRoom?.Invoke();
                break;
            case ClientState.Disconnected:
                OnDisconnected?.Invoke();
                break;
            case ClientState.ConnectedToMasterserver:
                CreateOrJoinRoom();
                OnConnectedToMasterServer?.Invoke();
                break;
            case ClientState.ConnectedToNameServer:
                if (string.IsNullOrEmpty(client.CloudRegion)){
                    client.OpGetRegions();
                }
                break;
        }
    }

    private void AutoConnectToMaster() {
        bool couldConnect = false;

        if (!string.IsNullOrEmpty(client.MasterServerAddress)) {
            couldConnect = client.Connect();
        } else {
            couldConnect = client.ConnectToRegionMaster("EU");
        }

        if (!couldConnect) {
            client.DebugReturn(DebugLevel.ERROR, "Can't connect to: " + client.CurrentServerAddress);
        }
    }

    public void CreateOrJoinRoom() {
        client.OpJoinOrCreateRoom("Main", new RoomOptions { MaxPlayers = 12 }, TypedLobby.Default);
    }


    private void Client_OnOpResponseAction(OperationResponse operationResponse) {
        if (operationResponse.ReturnCode != ErrorCode.Ok) {
            client.DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull() + " " + State);
        }
        //switch (operationResponse.OperationCode) {
        //    case OperationCode.JoinGame:
        //    case OperationCode.JoinRandomGame:
        //        OnJoinedGame?.Invoke();
        //        break;

        //    case OperationCode.JoinLobby:
        //        OnJoinedLobby?.Invoke();
        //        break;

        //    case OperationCode.LeaveLobby:
        //        OnLeftLobby?.Invoke();
        //        break;
        //}
    }

    private void Client_OnEventAction(EventData photonEvent) {
        int actorNr = 0;
        if (photonEvent.Parameters.ContainsKey(ParameterCode.ActorNr)) {
            actorNr = (int)photonEvent[ParameterCode.ActorNr];
        }

        Player player = null;
        if (actorNr > 0) {
            client.CurrentRoom.Players.TryGetValue(actorNr, out player);
        }

        switch (photonEvent.Code) {
            case EventCode.Join:
                OnPlayerJoined?.Invoke(player.ID);
                break;

            case EventCode.Leave:
                OnPlayerLeft?.Invoke(actorNr);
                break;
        }

        if (photonEvent.Parameters.ContainsKey(ParameterCode.CustomEventContent)) {
            object data = photonEvent[ParameterCode.CustomEventContent];
            if (data is byte[] dataBuffer) {
                LastReceivedMessageSize = dataBuffer.Length;
            }
            OnEventData?.Invoke(photonEvent.Code, player.ID, data);
            HandleDataEvents(photonEvent.Code, player, data);
            
        }
    }

    public void SendMessage(byte eventCode, object data, bool reliable, RaiseEventOptions eventOptions) {
        if (IsConnectedAndReady) {
            if (data is byte[] dataBuffer) {
                LastSendMessageSize = dataBuffer.Length;
            }
            client.OpRaiseEvent(eventCode, data, reliable, eventOptions);
        }
    }

    public void SetGroup(byte[] groupsToRemove, byte[] groupsToAdd) {
        client.OpChangeGroups(groupsToRemove, groupsToAdd);
    }

    public void RemoveAllGroups() {
        client.OpChangeGroups(AllGroupsArray, null);
    }

    //public void SendMessage(byte eventCode, object data, bool reliable = false) {
    //    SendMessage(eventCode, data, reliable, RaiseEventOptions.Default);
    //}    


    public void Update() {
        client.Service();
    }

    public int GetNetworkId() {
        if (freeNetworkIds.Count == 0) {
            return unchecked(++networkIdCounter);
        } else {
            return freeNetworkIds.Dequeue();
        }
    }

    public void FreeNetworkId(int id) {
        freeNetworkIds.Enqueue(id);
    }

    public void SendMessage(byte eventId, byte[] data, bool reliable, NetworkEventOptions networkEventOptions) {
        switch (networkEventOptions.Receiver) {
            case NetworkReceiverGroup.Others:
                eventOption.Receivers = ReceiverGroup.Others;
                break;
            case NetworkReceiverGroup.MasterClient:
                eventOption.Receivers = ReceiverGroup.MasterClient;
                break;
            case NetworkReceiverGroup.Target:
                break;
            default:
                break;
        }
        eventOption.TargetActors = networkEventOptions.TargetActors;
        SendMessage(eventId, data, reliable, eventOption);
    }
}









/*
public static class NetworkManagerOld {
    private static readonly LoadBalancingClient client = new LoadBalancingClient();


    public static event Action<EventData> OnEventAction { add { client.OnEventAction += value; } remove { client.OnEventAction -= value; } }

    public static event Action<OperationResponse> OnOpResponseAction { add { client.OnOpResponseAction += value; } remove { client.OnOpResponseAction -= value; } }
    public static event Action<ClientState> OnStateChangeAction { add { client.OnStateChangeAction += value; } remove { client.OnStateChangeAction -= value; } }


    public static void ConnectToMaster() {
        client.AppId = "0d284086-365f-43b5-acb3-3ac5d0c3c4be";
        client.AppVersion ="1.0";
        client.ConnectToNameServer();
        if (!client.ConnectToRegionMaster("eu")) {
            client.DebugReturn(ExitGames.Client.Photon.DebugLevel.ERROR, "Can't connect to: " + client.CurrentServerAddress);
        }
    }

    public static void CreateOrJoinRoom() {
        client.OpJoinOrCreateRoom("Main", new RoomOptions { MaxPlayers = 12 }, TypedLobby.Default);        
    }

    public static void Send(byte eventCode, object data, bool reliable) {
        client.OpRaiseEvent(eventCode, data, reliable, RaiseEventOptions.Default);
    }

    public static void Service() {
        client.Service();
    }
}
*/