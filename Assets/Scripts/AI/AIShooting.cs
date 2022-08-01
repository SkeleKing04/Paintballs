using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AIShooting : MonoBehaviour
{
    [Header("Positions")]
    private int layerMasks;
    [Header("Tracer")]
    //public TrailRenderer tracer;
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
    //public KeyCode fireKey;
    //public KeyCode secondaryActionKey;
    //public KeyCode reloadKey;
    //public KeyCode switchWeaponKey;
    private List<KeyCode> inputBuffer;
    // Start is called before the first frame update
    void Start()
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
    }
    public void readyWeapon()
    {
        state = gunState.ready;
    }
    public void Shoot(Transform fireTransform, Transform trailStartTransform)
    {
        //Variables
        RaycastHit hit;
        Vector3 startPos = fireTransform.position;
        Vector3 endPoint = fireTransform.forward;
        //Creates the tracer line  
        LineRenderer line = Instantiate(lineRenderer, trailStartTransform.position, Quaternion.identity);
        // Raycast to see if object hit in range
        // The "50f" needs to be changed to the weapons range
        if(Physics.Raycast(startPos, endPoint, out hit, 50f, layerMasks))
        {   
            Debug.DrawRay(startPos, fireTransform.forward * hit.distance, Color.green, 0.1f);
            Debug.Log("Hit object" + hit.collider.name);
            //Begins to set up the tracer
            StartCoroutine(setLine(line, trailStartTransform.position, hit.point));
            try
            {
                hit.collider.gameObject.GetComponent<HealthHandler>().UpdateHealth(10, gameObject, true, false);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to Update Health of " + hit.collider.gameObject.name + ". Are you missing the component?\nThe error is " + e);
            }
        }
        else
        {
            Debug.DrawRay(startPos, fireTransform.forward * 50f, Color.red, 0.1f);
            Debug.Log("Missed object.");
            //Begins to set up the tracer
            StartCoroutine(setLine(line, trailStartTransform.position, fireTransform.forward * 50f));
        }
        // stops the gun from firing stupidly
        state = gunState.firing;
        Invoke(nameof(readyWeapon), 0.1f);
    }
    private IEnumerator setLine(LineRenderer line, Vector3 startPos, Vector3 endPoint)
    {
        //Sets the start and end positions of the tracers
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPoint);
        // Sets how long the trail is visible for
        //this needs to be a different variable
        // the colour here should corespond to the colour of the player's team/paint
        try
        {
            teamColor = GetComponent<TeamManager>().teamColor;
            Debug.Log(gameObject.name + "'s team colour is " + teamColor.ToString());
            line.startColor = new Color(teamColor.r, teamColor.g, teamColor.b, 1);
            line.endColor = new Color(teamColor.r, teamColor.g, teamColor.b, 1);
        }
        catch (Exception e)
        {
            Debug.Log("Failed to update the colour of the line. Does the sender have team manager attached?\nThe error is " + e);
        }

        float time = trailTime;
        while (time > 0)
        {
            line.startWidth = trailWidth * time;
            line.endWidth = line.startWidth;
            time -= Time.deltaTime;
            yield return null;
        }
        Destroy(line.gameObject);
    }
}
