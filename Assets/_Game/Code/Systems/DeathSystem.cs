using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class DeathSystem : ComponentSystem {

    struct Data {
        public readonly int Length;
        //[ReadOnly]
        //[WriteOnly]
        public ComponentDataArray<DeathComponent> death;
        public EntityArray entities;
    }

    [Inject] Data data;
    private List<GameObject> gameObjectsToDestroy = new List<GameObject>();

    protected override void OnUpdate() {
        for (int i = 0; i < data.Length; i++) {
            DeathComponent death = data.death[i];
            Entity entity = data.entities[i];
            death.timer -=Time.deltaTime;

            if (death.timer < 0) {
                PostUpdateCommands.DestroyEntity(entity);
                if (EntityManager.HasComponent<Transform>(entity)) {
                    gameObjectsToDestroy.Add(EntityManager.GetComponentObject<Transform>(entity).gameObject);
                }
            }

            data.death[i] = death;
        }

        for (int i = 0; i < gameObjectsToDestroy.Count; i++) {
            Object.Destroy(gameObjectsToDestroy[i]);
        }
        gameObjectsToDestroy.Clear();
    }
}
