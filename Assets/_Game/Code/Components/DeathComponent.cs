using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

//[Serializable]
[NetSync]
public struct DeathComponent : IComponentData {
    [NetSyncMember(initOnly: true)]
    public float timer;
}

//public class DeathComponent  : ComponentDataWrapper<DeathComponent> { }
