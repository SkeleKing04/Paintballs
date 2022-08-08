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
    //private damageType type;
    private DeathmatchScript deathmatchScript;
    public void Awake()
    {
        currentHealth = baseHealth;
        deathmatchScript = FindObjectOfType<DeathmatchScript>();
    }
    public void UpdateHealth(float pointAmmount, GameObject sender, bool doPaint, bool doOverheal)
    {
        if(doPaint)
        {
            Debug.Log(gameObject.name + " got " + pointAmmount.ToString() + " points of paint damage from " + sender.name);
            currentPaint = Mathf.Clamp(currentPaint + pointAmmount, 0, maxPaint);
        }
        else if(!doPaint)
        {
            Debug.Log(gameObject.name + " got " + pointAmmount.ToString() + " points of damage from " + sender.name);
            if(doOverheal)
            {
                currentHealth = Mathf.Clamp(currentHealth + pointAmmount, 0, Mathf.Infinity);
            }
            else if (!doOverheal)
            {
                currentHealth = Mathf.Clamp(currentHealth + pointAmmount, 0, baseHealth);

            }
        }

        DeathCheck(sender);
    }
    private void DeathCheck(GameObject sender)
    {
        if(currentPaint >= maxPaint)
        {
            Debug.Log(gameObject.name + " has been killed by " + sender.name);
            deathmatchScript.updatePlayerScore(sender);
        }
        if(currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " has been knocked out by " + sender.name);
        }
    }
}
