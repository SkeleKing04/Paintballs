using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestDummyScript : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        try
        {
            other.gameObject.GetComponent<HealthHandler>().UpdateHealth(10, gameObject, 0);
            Debug.Log("health - " + other.gameObject.GetComponent<HealthHandler>().currentHealth.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError("Whatever just collided doesn't have a HealthHandler");
        }
    }
}
