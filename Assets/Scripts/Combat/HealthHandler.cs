using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

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
    public bool dead, despawned, isClient;
    [Header("UI")]
    public TextMeshProUGUI damageText;
    public Image paintImage; 
    public void Awake()
    {
        resetHealth();
        deathmatchScript = FindObjectOfType<DeathmatchScript>();
        UpdateHealth(0,0,gameObject);
    }
    public void resetHealth()
    {
        currentHealth = baseHealth;
        currentPaint = 0;
        dead = false;
    }
    public void UpdateHealth(float damageAmmount, float paintAmmount, GameObject sender)
    {
        currentHealth = Mathf.Clamp(currentHealth - damageAmmount, 0, baseHealth);
        currentPaint = Mathf.Clamp(currentPaint + paintAmmount, 0, maxPaint);
        if (isClient)
        { 
            damageText.text = currentHealth.ToString();
            paintImage.fillAmount = currentPaint/ maxPaint; 
        }
        DeathCheck(sender);
    }
    private void DeathCheck(GameObject sender)
    {
        if(currentPaint >= maxPaint)
        {
            //Debug.Log(gameObject.name + " has been killed by " + sender.name);
            dead = true;
            if(sender.GetComponent<TeamManager>().teamColor != GetComponent<TeamManager>().teamColor)
            {
                deathmatchScript.updatePlayerScore(sender);
            }
            deathmatchScript.deSpawnMe(gameObject, true, 5f);
        }
        if(currentHealth <= 0)
        {
            //Debug.Log(gameObject.name + " has been knocked out by " + sender.name);
        }
    }

}
