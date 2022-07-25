using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPointer : MonoBehaviour
{
    // Update is called once per frame
    public void pointGun(Transform origin)
    {
        RaycastHit hit;
        if(Physics.Raycast(origin.position, origin.forward, out hit, Mathf.Infinity))
        {
            transform.LookAt(hit.point);
        }
    }
}
