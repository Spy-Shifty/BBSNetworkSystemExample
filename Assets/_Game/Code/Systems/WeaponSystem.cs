using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WeaponSystem : ComponentSystem {

    struct Data {
        public readonly int Length;
        public ComponentDataArray<WeaponState> weaponState;
        public ComponentDataArray<WeaponAction> weaponAction;
        public ComponentArray<WeaponComponent> weaponComponent;
        public EntityArray entities;
    }

    [Inject] Data data;

    protected override void OnUpdate() {
        for (int i = 0; i < data.Length; i++) {
            WeaponComponent weaponComponent = data.weaponComponent[i];
            WeaponState weaponState = data.weaponState[i];
            WeaponAction weaponAction = data.weaponAction[i];
            Entity entity = data.entities[i];

            LineRenderer lineRenderer = weaponComponent.lineRenderer;
            Transform weaponTransform = weaponComponent.lineRenderer.transform;
            lineRenderer.SetPosition(0, weaponTransform.position);

            weaponState.reloadTimer = math.max(0, weaponState.reloadTimer - Time.deltaTime);
            weaponState.fireTimer = math.max(0, weaponState.fireTimer - Time.deltaTime);
            weaponState.effectTimer = math.max(0, weaponState.effectTimer - Time.deltaTime);

            if(lineRenderer.enabled && weaponState.effectTimer == 0) {
                lineRenderer.enabled = false;
            }

            if (weaponState.reloading && weaponState.reloadTimer == 0) {
                weaponState.magazine = weaponComponent.magazinSize;
                weaponState.reloading = false;
            }

            if (weaponAction.reload && weaponState.magazine < weaponComponent.magazinSize && !weaponState.reloading) {
                weaponState.reloadTimer = weaponComponent.reloadTime;
                weaponState.reloading = true;
            }
            
            if (weaponAction.fire && weaponState.magazine > 0 && weaponState.fireTimer == 0 && !weaponState.reloading) {
                lineRenderer.enabled = true;
                weaponState.fireTimer = weaponComponent.fireRate;
                weaponState.magazine--;

                if (Physics.SphereCast(weaponAction.shootOrigin, 0.5f, weaponAction.shootDir, out RaycastHit hit, weaponComponent.range)) {
                    lineRenderer.SetPosition(1, hit.point);
                    PostUpdateCommands.CreateEntity();
                    GameObjectEntity hittenGameObjectEntity = hit.collider.GetComponent<GameObjectEntity>();
                    if (hittenGameObjectEntity) {
                        PostUpdateCommands.AddComponent(new DamageInfo {
                            source = entity,
                            receiver = hittenGameObjectEntity.Entity,
                            damage = weaponComponent.damage
                        });
                    }
                } else {
                    lineRenderer.SetPosition(1, weaponTransform.position + weaponTransform.forward * weaponComponent.range);
                }
            }

            data.weaponState[i] = weaponState;
        }
    }
}
