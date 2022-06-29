using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    // health that object starts with
    public float baseHealth;
    // health used throughout the gameplay
    public float currentHealth;
    public float maxPaint;
    public float currentPaint;
    public enum damageType
    {
        normal,
        paint,
        healing,
        overheal,
        removepaint
    };
    //private damageType type;
    public void Awake()
    {
        currentHealth = baseHealth;
    }
    public void UpdateHealth(float pointAmmount, GameObject sender, damageType type)
    {
        switch(type)
        {
            case damageType.normal:
                currentHealth -= Mathf.Clamp(pointAmmount, 0, baseHealth);
                break;
            case damageType.paint:
                currentPaint += Mathf.Clamp(pointAmmount, 0, maxPaint);
                break;
            case damageType.healing:
                currentHealth += Mathf.Clamp(pointAmmount, 0, baseHealth);
                break;
            case damageType.overheal:
                currentHealth += Mathf.Clamp(pointAmmount, 0, baseHealth * 2);
                break;
            case damageType.removepaint:
                currentPaint -= Mathf.Clamp(pointAmmount, 0, maxPaint);
                break;
        }
        DeathCheck(sender);
    }
    private void DeathCheck(GameObject sender)
    {
        if(currentPaint >= maxPaint)
        {
            Debug.Log(gameObject.name + " has been killed by " + sender.name);
        }
    }
}
