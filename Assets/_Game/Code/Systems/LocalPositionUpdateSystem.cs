using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class LocalPositionUpdateSystem : ComponentSystem {
    struct Data {
        public readonly int Length;
        public ComponentDataArray<Velocity> velocities;
        public ComponentDataArray<Position> positions;
        public ComponentDataArray<Heading> headings;
        public ComponentArray<CharacterController> characterControllers;
    }
    
    [Inject] Data data;

    protected override void OnUpdate() {

        for (int i = 0; i < data.Length; i++) {
            CharacterController characterController = data.characterControllers[i];
            Transform transform = characterController.transform;
            Vector3 velocity = data.velocities[i].Value;
            Vector3 position = data.positions[i].Value;
            Vector3 heading = data.headings[i].Value;
            if (heading != Vector3.zero) {
                transform.rotation = Quaternion.LookRotation(heading);
            }

            position += velocity;
            characterController.Move(velocity);
            data.positions[i] = new Position { Value = characterController.transform.position };
        }
    }
}
