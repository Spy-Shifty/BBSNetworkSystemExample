using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerGui : MonoBehaviour {
    public string AppId = "0d284086-365f-43b5-acb3-3ac5d0c3c4be";
    public string AppVersion = "1.0";
    
    // Update is called once per frame
    private void OnGUI() {
        GUILayout.Label(string.Format("networkManager.IsConnected: {0}", NetworkManager.Instance.IsConnected));
        GUILayout.Label(string.Format("IsConnectedAndReady: {0}", NetworkManager.Instance.IsConnectedAndReady));
        GUILayout.Label(string.Format("IsMaster: {0}", NetworkManager.Instance.IsMaster));
        GUILayout.Label(string.Format("RoomsCount: {0}", NetworkManager.Instance.RoomsCount));
        GUILayout.Label(string.Format("PlayersInRoomCount: {0}", NetworkManager.Instance.PlayersInRoomCount));
        GUILayout.Label(string.Format("PlayersOnMasterServerCount: {0}", NetworkManager.Instance.PlayersOnMasterServerCount));
        GUILayout.Label(string.Format("State: {0}", NetworkManager.Instance.State));
        GUILayout.Label(string.Format("Size of last send message: {0}", NetworkManager.Instance.LastSendMessageSize));
        GUILayout.Label(string.Format("Size of last received message: {0}", NetworkManager.Instance.LastReceivedMessageSize));

        if (!NetworkManager.Instance.IsConnected && GUILayout.Button("Connect")) {
            NetworkManager.Instance.AppId = AppId;
            NetworkManager.Instance.AppVersion = AppVersion;
            NetworkManager.Instance.Connect();
        } else if(NetworkManager.Instance.IsConnected && GUILayout.Button("Disconnect")) {
            NetworkManager.Instance.Disconnect();
        }
    }

    private void OnApplicationQuit() {
         if (NetworkManager.Instance.IsConnected) {
            NetworkManager.Instance.Disconnect();
        }
    }
}
