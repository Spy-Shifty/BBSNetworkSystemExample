using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInputSystem : ComponentSystem {

    private struct LivingPlayer {
        public readonly int Length;
        [WriteOnly] public ComponentDataArray<PlayerInput> playerInput;
        [ReadOnly] public SubtractiveComponent<DeathComponent> death;
    }

    private struct DeadPlayer {
        public readonly int Length;
        [WriteOnly] public ComponentDataArray<PlayerInput> playerInput;
        [ReadOnly] public ComponentDataArray<DeathComponent> death;
    }

    [Inject] private LivingPlayer livingPlayers;
    [Inject] private DeadPlayer deadPlayers;

    protected override void OnUpdate() {
        var newPlayerInput = new PlayerInput {
            move = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
            lookRaw = new float2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")),
            jump = Input.GetButtonDown("Jump"),
            fire = Input.GetButton("Fire1"),
            reload = Input.GetButton("Reload"),
        };

        float mouseSmoothness = GameSettings.Instance.MouseSmoothness;
        for (int i = 0; i < livingPlayers.Length; i++) {
            PlayerInput currentPlayerInput = livingPlayers.playerInput[i];
            newPlayerInput.look = math.lerp(currentPlayerInput.look, newPlayerInput.lookRaw, Time.deltaTime * mouseSmoothness);
            livingPlayers.playerInput[i] = newPlayerInput;
        }

        for (int i = 0; i < deadPlayers.Length; i++) {
            PlayerInput currentPlayerInput = deadPlayers.playerInput[i];
            currentPlayerInput.jump = false;
            currentPlayerInput.fire = false;
            currentPlayerInput.reload = false;
            currentPlayerInput.move = new float2(0,0);
            currentPlayerInput.lookRaw = new float2(0,0);
            currentPlayerInput.look = new float2(0,0);
            deadPlayers.playerInput[i] = currentPlayerInput;
        }
    }
}
