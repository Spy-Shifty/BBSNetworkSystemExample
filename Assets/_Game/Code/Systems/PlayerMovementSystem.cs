using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovementSystem : ComponentSystem {
    struct PlayerData {
        public readonly int Length;
        public ComponentDataArray<PlayerInput> input;
        public ComponentArray<CharacterController> characterControllers;
        public ComponentDataArray<Velocity> velocities;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Heading> headings;
        public EntityArray entity;
    }
    
    [Inject] PlayerData playerData;

    protected override void OnUpdate() {

        var walkSpeed = GameSettings.Instance.WalkSpeed;
        var horizontalLookSpeed = GameSettings.Instance.HorizontalLookSpeed;
        var jumpPower = GameSettings.Instance.JumpPower;
        var gravityScale = GameSettings.Instance.GravityScale;

        var dt = Time.deltaTime;
        for (int i = 0; i < playerData.Length; i++) {
            PlayerInput input = playerData.input[i];
            CharacterController characterController = playerData.characterControllers[i];
            Transform transform = characterController.transform;
            Vector3 velocity = playerData.velocities[i].Value;
            Vector3 heading = playerData.headings[i].Value;
            if(heading == Vector3.zero) {
                heading = transform.forward;
            }


            float speed = walkSpeed;
            Vector3 movement = transform.right * input.move.x + transform.forward * input.move.y;
            movement = movement.normalized * speed * dt;

            velocity = new Vector3(movement.x, velocity.y, movement.z);
            heading = Quaternion.Euler(new Vector3(0, input.look.x, 0) * horizontalLookSpeed * dt) * heading;


            velocity.y += Physics.gravity.y * gravityScale * dt;
            if (characterController.isGrounded) {
                if (input.jump) {
                    velocity.y = jumpPower;
                }
            }
            playerData.velocities[i] = new Velocity { Value = velocity };
            playerData.headings[i] = new Heading(heading);
        }
    }
}
