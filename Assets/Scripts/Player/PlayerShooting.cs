using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(GunHandler))]
public class PlayerShooting : MonoBehaviour
{
    [Header("Positions")]
    public Transform trueFireTransform;
    public Transform falseFireTransform;
    private LayerMask layerMasks;
    private GunPointer gunPointer;
    [Header("Tracer")]
    //public TrailRenderer tracer;
    public bool doTracer;
    public LineRenderer lineRenderer;
    public float trailTime;
    public float trailWidth;
    private Color teamColor;
    public Animator animator;

    public enum gunState
    {
        ready,
        firing,
        reloading,
        switchOut,
        swtichIn
    };
    [Header("State Management")]
    public gunState state;
    [Header("Keybinds")]
    public KeyCode fireKey;
    public KeyCode secondaryActionKey;
    public KeyCode reloadKey;
    public KeyCode switchWeaponKey;
    [Header("Extras")]
    public bool isClientOBJ;
    private GunHandler HeldGun;
    public Image hitMarker;
    public AudioClip reloadSound;
    private AudioSource audio;
    // Start is called before the first frame update
    void Awake()
    {
        layerMasks = 1 << 8;
        layerMasks = ~layerMasks;
        try
        {
            teamColor = GetComponent<TeamManager>().teamColor;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to find team color in team manager! Did you forget to attach the component to this object? (" + gameObject.name + ")\nThe error is " + e);
        }

        if (isClientOBJ) gunPointer = GetComponent<GunPointer>();
        HeldGun = GetComponent<GunHandler>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isClientOBJ)
        {
            gunPointer.pointGun(trueFireTransform);
            if(Input.GetKey(fireKey) && state == gunState.ready && HeldGun.ammoInWeapon[HeldGun.gunIndex] > 0)
            {
                Shoot();
            }
            else if(HeldGun.ammoInWeapon[HeldGun.gunIndex] <= 0 && state == gunState.ready)
            {
                Reload();
            }
            if(Input.GetKey(secondaryActionKey))
            {
                //Not used ATM
            }
            if(Input.GetKeyDown(reloadKey) && state == gunState.ready)
            {
                Reload();
            }
            if(Input.GetKeyDown(switchWeaponKey) && state == gunState.ready)
            {
                //UI.updateTextBox(new int[] {2}, new string[] {"Gun: " + HeldGun.gun.name});
                //Switch to next weapon
            }
        }

    }
    public void readyWeapon()
    {
        if(state == gunState.reloading) audio.Stop();
        if(isClientOBJ) animator.SetTrigger("ReadyWeapon");
        state = gunState.ready;
    }
    public void Shoot()
    {
        //Debug.Log(gameObject.name + " is shooting");
        //Variables
        RaycastHit hit;
        Vector3 startPos = trueFireTransform.position;
        Vector3 endPoint = trueFireTransform.forward;
        //Creates the tracer line  
        LineRenderer line = new LineRenderer();
        if(doTracer) line = Instantiate(lineRenderer, falseFireTransform.position, Quaternion.identity);
        teamColor = GetComponent<TeamManager>().teamColor;
        // Raycast to see if object hit in range
        // The "50f" needs to be changed to the weapons range
        if(Physics.Raycast(startPos, endPoint * HeldGun.gunList[HeldGun.gunIndex].range, out hit, HeldGun.gunList[HeldGun.gunIndex].range, layerMasks))
        {   
            Debug.DrawRay(startPos, trueFireTransform.forward * hit.distance, Color.green, HeldGun.gunList[HeldGun.gunIndex].rateOfFire);
            Debug.DrawRay(startPos, trueFireTransform.forward * HeldGun.gunList[HeldGun.gunIndex].range, Color.blue, HeldGun.gunList[HeldGun.gunIndex].rateOfFire);
            if(hit.collider == null)
            {
                Ray ray = new Ray(startPos, trueFireTransform.forward * HeldGun.gunList[HeldGun.gunIndex].range);
                hit.point = (ray.GetPoint(HeldGun.gunList[HeldGun.gunIndex].range));
            }
            //Debug.Log("Hit object" + hit.collider.name);
            //Begins to set up the tracer
            if(doTracer) StartCoroutine(line.GetComponent<BulletTracer>().setLine(falseFireTransform.position, hit.point, GetComponent<TeamManager>().teamColor, HeldGun.gunList[HeldGun.gunIndex].rateOfFire, trailWidth, gameObject));
            try
            {
                hit.collider.gameObject.GetComponentInParent<HealthHandler>().UpdateHealth(HeldGun.gunList[HeldGun.gunIndex].damage, HeldGun.gunList[HeldGun.gunIndex].paintDamage, gameObject);
                if(isClientOBJ)
                {
                    hitMarker.GetComponent<Animator>().SetTrigger("hit");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to Update Health of " + hit.collider.gameObject.name + ". Are you missing the component?\nThe error is " + e);
            }
        }
        else
        {
            Debug.DrawRay(startPos, trueFireTransform.forward * HeldGun.gunList[HeldGun.gunIndex].range, Color.red, 0.1f);
            //Debug.Log("Missed object.");
            //Begins to set up the tracer
            if(doTracer) StartCoroutine(line.GetComponent<BulletTracer>().setLine(falseFireTransform.position, falseFireTransform.forward * HeldGun.gunList[HeldGun.gunIndex].range, GetComponent<TeamManager>().teamColor, trailTime, trailWidth, gameObject));
        }
        // stops the gun from firing stupidly
        state = gunState.firing;
        //animator.SetFloat("ReadySpeed",1 - HeldGun.gunList[HeldGun.gunIndex].rateOfFire);
        if(isClientOBJ) animator.SetTrigger("FireWeapon");
        HeldGun.shootWeapon();
        Invoke(nameof(readyWeapon), HeldGun.gunList[HeldGun.gunIndex].rateOfFire);
    }
    public void Reload()
    {
        if(isClientOBJ)animator.SetTrigger("ReloadSpin");
        audio.clip = reloadSound;
        audio.PlayDelayed(0.5f);
        state = gunState.reloading;
        HeldGun.Invoke(nameof(HeldGun.reloadWeapon),HeldGun.gunList[HeldGun.gunIndex].reloadSpeed);
        Invoke(nameof(readyWeapon), HeldGun.gunList[HeldGun.gunIndex].reloadSpeed);
    }
}
