using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

//[Serializable]
[NetSync]
public struct WeaponAction : IComponentData {
    [NetSyncMember]
    public boolean fire;
    [NetSyncMember]
    public boolean reload;

    [NetSyncMember(lerpDamp: 0)]
    [NetSyncSubMember("x"), NetSyncSubMember("y"), NetSyncSubMember("z")]
    public Vector3 shootOrigin;

    [NetSyncMember(lerpDamp:0)]
    [NetSyncSubMember("x"), NetSyncSubMember("y"), NetSyncSubMember("z")]
    public Vector3 shootDir;

}

public class WeaponActionComponent : ComponentDataWrapper<WeaponAction> { }
