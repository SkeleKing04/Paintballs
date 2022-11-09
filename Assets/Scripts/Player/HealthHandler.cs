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
    public AudioClip deathSound;
    private AudioSource source;
    [Header("UI")]
    public TextMeshProUGUI damageText, paintText;
    public Image damageImage, paintImage; 
    [Header("DeathJump")]
    public float jumpForce;
    public float positionOffset, range, vertical;

    public void Awake()
    {
        resetHealth();
        deathmatchScript = FindObjectOfType<DeathmatchScript>();
        UpdateHealth(-baseHealth,0,gameObject);
        if(gameObject.GetComponent<EnemyAI>()) gameObject.GetComponent<EnemyAI>().enabled = true;
        if(isClient)
        {
            CameraController camControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
            camControl.originOffset = new Vector3(0,0,0);
            camControl.doMouseMovement = true;
            camControl.lookAtTarget = false;
            camControl.useWorldY = false;
            //paintText.GetComponentInParent<GameObject>().GetComponentInChildren<Image>().color = gameObject.GetComponent<TeamManager>().teamColor;
        }
        gameObject.GetComponent<MovementScript>().enabled = true;
        gameObject.GetComponent<Transform>().rotation = new Quaternion(0,0,0,0);
        gameObject.GetComponent<Rigidbody>().freezeRotation = true;
        source = gameObject.GetComponent<AudioSource>();
    }
    public void resetHealth()
    {
        currentHealth = baseHealth;
        currentPaint = 0;
        dead = false;
    }
    public void UpdateHealth(float damageAmmount, float paintAmmount, GameObject sender)
    {
        if(!dead)
        {
        currentHealth = Mathf.Clamp(currentHealth - damageAmmount, 0, baseHealth);
        currentPaint = Mathf.Clamp(currentPaint + paintAmmount, 0, maxPaint);
        if (isClient)
        { 
            paintText.text = currentPaint.ToString();
            paintImage.fillAmount = currentPaint/maxPaint; 
            damageText.text = currentHealth.ToString();
            damageImage.fillAmount = currentHealth / baseHealth;
        }
        DeathCheck(sender);
        }
    }
    private void DeathCheck(GameObject sender)
    {
        if(currentPaint >= maxPaint && !dead)
        {
            Debug.Log(gameObject.name + " has been killed by " + sender.name);
            source.clip = deathSound;
            source.Play();
            if(sender.GetComponent<TeamManager>().teamColor != GetComponent<TeamManager>().teamColor && !dead)
            {
                deathmatchScript.updatePlayerScore(sender);
            }
            deathmatchScript.deSpawnMe(gameObject, true, 5f);
            if(gameObject.GetComponent<EnemyAI>()) gameObject.GetComponent<EnemyAI>().enabled = false;
            gameObject.GetComponent<MovementScript>().enabled = false;
            gameObject.GetComponent<Rigidbody>().freezeRotation = false;
            if(!dead)
            {
                gameObject.GetComponent<Rigidbody>().AddExplosionForce(jumpForce,transform.position - (transform.position - sender.transform.position),(transform.position - sender.transform.position).magnitude,vertical,ForceMode.Impulse);
            }
            /*if(isClient && !dead)
            {
                CameraController camControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
                camControl.originOffset = new Vector3(0,5,-5);
                camControl.useWorldY = true;
                camControl.doMouseMovement = false;
                camControl.lookAtTarget = true;
            }*/
            dead = true;
        }
        if(currentHealth <= 0)
        {
            //Debug.Log(gameObject.name + " has been knocked out by " + sender.name);
        }
    }

}
