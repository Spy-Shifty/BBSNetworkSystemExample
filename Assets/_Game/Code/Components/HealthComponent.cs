using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[NetSync]
[Serializable]
public struct Health : IComponentData {

    [NetSyncMember]
    public int value;
}
public class HealthComponent : ComponentDataWrapper<Health> { }