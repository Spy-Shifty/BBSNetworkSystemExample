using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[NetworkEntityFactory]
public static class EntityFactory {


    [NetworkEntityFactoryMethod(1)]
    public static Entity CreateNetPlayer(EntityManager entityManager) {
        //return entityManager.Instantiate(GameSettings.Instance.NetworkPlayerPrefab);
        GameObject gameObject = GameObject.Instantiate(GameSettings.Instance.NetworkPlayerPrefab);
        return gameObject.GetComponent<GameObjectEntity>().Entity;
    }
}
