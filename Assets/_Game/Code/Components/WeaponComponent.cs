using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(WeaponStateComponent))]
[RequireComponent(typeof(WeaponActionComponent))]
public class WeaponComponent : MonoBehaviour {
    public LineRenderer lineRenderer;
    public int magazinSize;
    public float fireRate;
    public float reloadTime;
    public float effectTime;
    public int damage;
    public float range;
}
