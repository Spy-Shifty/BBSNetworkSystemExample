using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct Velocity : IComponentData {
    public Vector3 Value;
}

public class VelocityComponent : ComponentDataWrapper<Velocity> { }