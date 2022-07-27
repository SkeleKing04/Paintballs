using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPointer : MonoBehaviour
{
    public Transform rotatorPoint;
    // Update is called once per frame
    public void pointGun(Transform origin)
    {
        RaycastHit hit;
        if(Physics.Raycast(origin.position, origin.forward, out hit, Mathf.Infinity))
        {
            rotatorPoint.LookAt(hit.point);
        }
    }
}
