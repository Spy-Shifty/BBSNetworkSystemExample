using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class ApplyDamageSystem : ComponentSystem {

    struct FilteredData {
        public readonly int Length;
        //[ReadOnly]
        //[WriteOnly]
        public ComponentDataArray<DamageInfo> damageInfo;
        //public ComponentArray<> comp;		
        public EntityArray entities;
    }


    [Inject] FilteredData filteredData;

    protected override void OnUpdate() {
        for (int i = 0; i < filteredData.Length; i++) {
            DamageInfo damageInfo = filteredData.damageInfo[i];

            Entity entity = filteredData.entities[i];
            if (EntityManager.HasComponent<NetworktOwner>(damageInfo.receiver)) {
                Health health = EntityManager.GetComponentData<Health>(damageInfo.receiver);
                health.value -= damageInfo.damage;
                if (health.value < 0) {
                    health.value = 0;
                    EntityManager.AddComponentData(damageInfo.receiver, new DeathComponent { timer = 3 });
                }
                EntityManager.SetComponentData(damageInfo.receiver, health);
            }

            PostUpdateCommands.DestroyEntity(entity);
        }
    }
}
