using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

//[Serializable]
public struct DamageInfo : IComponentData {
    public Entity source;
    public Entity receiver;
    public int damage;
}

//public class Unity_Entity_Component1  : ComponentDataWrapper<Unity_Entity_Component1> { }
