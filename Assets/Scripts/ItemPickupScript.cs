using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemPickupScript : MonoBehaviour
{
    public enum pickupType
    {
        gun,
        ammo,
        heal,
    };
    public pickupType type;
    
    public GunSO gunType;
    public float ammoValue;
    public Vector2 damagePaintValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " hit " + other.name);
        switch(type)
        {
            case pickupType.gun:
                try {other.GetComponentInParent<GunHandler>().giveGun(gunType,true);} catch (Exception e) {}
                break;
            case pickupType.ammo:
                break;
            case pickupType.heal:
                try {other.GetComponentInParent<HealthHandler>().UpdateHealth(damagePaintValue.x,damagePaintValue.y,gameObject);} catch (Exception e) {}
                break;
        }
        Destroy(gameObject);
    }
    
}
