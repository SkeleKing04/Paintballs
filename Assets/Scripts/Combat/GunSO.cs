using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "New Gun")]
public class GunSO : ScriptableObject
{
    public float range, rateOfFire, reloadSpeed, damage, paintDamage;
    public int ammo;
    public bool explosive;
    public float explosiveForce, explosiveRange;
    public GameObject modelPrefab;
    public AudioClip fireNoise;
}
