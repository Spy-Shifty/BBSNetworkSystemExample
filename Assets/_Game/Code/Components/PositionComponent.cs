using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[NetSync]
public struct Position : IComponentData {
    [NetSyncMember(lerpDamp: 0.9f, jumpThreshold: 0)]
    [NetSyncSubMember("x")]
    [NetSyncSubMember("y")]
    [NetSyncSubMember("z")]
    public Vector3 Value;
}

public class PositionComponent : ComponentDataWrapper<Position> { }