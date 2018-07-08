using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[NetSync]
public struct Heading : IComponentData {
    [NetSyncMember(lerpDamp:0.9f)]
    [NetSyncSubMember("x")]
    [NetSyncSubMember("z")]
    public Vector3 Value;

    public Heading(Vector3 heading) {
        Value = heading;
    }
}

public class HeadingComponent : ComponentDataWrapper<Heading> { }