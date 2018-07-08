using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
[NetSync]
public struct WeaponState : IComponentData {
    [HideInInspector] public float fireTimer;
    [HideInInspector] public float reloadTimer;
    [HideInInspector] public float effectTimer;
    [HideInInspector] public boolean reloading;

    [NetSyncMember]
    public int magazine;
}

public class WeaponStateComponent : ComponentDataWrapper<WeaponState> { }
