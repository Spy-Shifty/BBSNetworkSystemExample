using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class HUDSystem : ComponentSystem {

    struct PlayerData {
        public readonly int Length;
        //[ReadOnly]
        //[WriteOnly]
        public ComponentDataArray<PlayerInput> playerInput;
        public ComponentDataArray<WeaponState> weaponState;
        public ComponentArray<WeaponComponent> weapon;		
        public ComponentDataArray<Health> health;
        //public EntityArray entities;
    }

    struct HudData {
        public readonly int Length;
        //[ReadOnly]
        //[WriteOnly]
        public ComponentArray<HUDComponent> hud;
        //public EntityArray entities;
    }

    [Inject] PlayerData playerData;
    [Inject] HudData hudData;



    protected override void OnUpdate() {
        for (int i = 0; i < hudData.Length; i++) {
            HUDComponent hud = hudData.hud[i];
            for (int j = 0; j < playerData.Length; j++) {
                WeaponState weaponState = playerData.weaponState[j];
                WeaponComponent weapon = playerData.weapon[j];
                Health health = playerData.health[j];

                hud.ammo.text = weaponState.magazine.ToString();

                float reloadProgress = 1 - weaponState.reloadTimer / weapon.reloadTime;
                hud.reloadBar.size = reloadProgress;
                hud.healthBar.size = health.value / 100f;
                hud.gameOver.SetActive(health.value == 0);
            }
        }
    }
}
