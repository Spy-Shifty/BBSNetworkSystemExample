using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Bootstrap {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene() {

        var entityManager = World.Active.GetExistingManager<EntityManager>();
        World.Active.SetNetworkManager(NetworkManager.Instance);

        //var entity1 = entityManager.CreateEntity(typeof(NetworkSync), typeof(NetworktOwner), typeof(ComponentA));
        //var entity2 = entityManager.CreateEntity(typeof(NetworkSync), typeof(NetworktOwner), typeof(ComponentB));
        //var entity3 = entityManager.CreateEntity(typeof(NetworkSync), typeof(NetworktOwner), typeof(ComponentA), typeof(ComponentB));

        //entityManager.SetComponentData(entity1, new ComponentA());
        //entityManager.SetComponentData(entity2, new ComponentB());
        //entityManager.SetComponentData(entity3, new ComponentA());
        //entityManager.SetComponentData(entity3, new ComponentB());
    }
}
