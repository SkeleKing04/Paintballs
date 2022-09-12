using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "New Gun")]
public class GunSO : ScriptableObject
{
    public float range, rateOfFire, reloadSpeed, damage, paintDamage;
    public GameObject modelPrefab;
}
