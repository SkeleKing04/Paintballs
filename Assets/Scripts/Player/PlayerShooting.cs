using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private UIHandler UI;
    // Start is called before the first frame update
    void Start()
    {
        UI = FindObjectOfType<UIHandler>();
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
    }

    // Update is called once per frame
    void Update()
    {
        if(isClientOBJ)
        {
            gunPointer.pointGun(trueFireTransform);
            if(Input.GetKey(fireKey) && state == gunState.ready)
            {
                Shoot();
            }
            if(Input.GetKey(secondaryActionKey))
            {
                //Not used ATM
            }
            if(Input.GetKeyDown(reloadKey) && state == gunState.ready)
            {
                //Reload();
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
        state = gunState.ready;
    }
    public void Shoot()
    {
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
        if(Physics.Raycast(startPos, endPoint * HeldGun.gun.range, out hit, HeldGun.gun.range, layerMasks))
        {   
            Debug.DrawRay(startPos, trueFireTransform.forward * hit.distance, Color.green, HeldGun.gun.rateOfFire);
            //Debug.Log("Hit object" + hit.collider.name);
            //Begins to set up the tracer
            if(doTracer) StartCoroutine(line.GetComponent<BulletTracer>().setLine(falseFireTransform.position, hit.point, GetComponent<TeamManager>().teamColor, trailTime, trailWidth, gameObject));
            try
            {
                hit.collider.gameObject.GetComponentInParent<HealthHandler>().UpdateHealth(HeldGun.gun.damage, HeldGun.gun.paintDamage, gameObject);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to Update Health of " + hit.collider.gameObject.name + ". Are you missing the component?\nThe error is " + e);
            }
        }
        else
        {
            Debug.DrawRay(startPos, trueFireTransform.forward * HeldGun.gun.range, Color.red, 0.1f);
            //Debug.Log("Missed object.");
            //Begins to set up the tracer
            if(doTracer) StartCoroutine(line.GetComponent<BulletTracer>().setLine(falseFireTransform.position, falseFireTransform.forward * HeldGun.gun.range, GetComponent<TeamManager>().teamColor, trailTime, trailWidth, gameObject));
        }
        // stops the gun from firing stupidly
        state = gunState.firing;
        Invoke(nameof(readyWeapon), HeldGun.gun.rateOfFire);
    }
}
