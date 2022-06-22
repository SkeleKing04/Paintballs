using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Positions")]
    public Transform fireTransform;
    public Transform trailStartTransform;
    private int layerMasks;
    [Header("Tracer")]
    public TrailRenderer tracer;
    public LineRenderer lineRenderer;
    public float trailTime;

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
    private List<KeyCode> inputBuffer;
    // Start is called before the first frame update
    void Start()
    {
        layerMasks = 1 << 8;
        layerMasks = ~layerMasks;
    }

    // Update is called once per frame
    void Update()
    {
        stateCheck();
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
            //Switch to next weapon
        }
    }
    public void stateCheck()
    {
        
    }
    public void readyWeapon()
    {
        state = gunState.ready;
    }
    public void Shoot()
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
        }
        else
        {
            Debug.DrawRay(startPos, fireTransform.forward * 50f, Color.red, 0.1f);
            Debug.Log("Missed object");
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
        line.SetPosition(0, trailStartTransform.position);
        line.SetPosition(1, endPoint);
        // Sets how long the trail is visible for
        //this needs to be a different variable
        float time = trailTime;
        while (time > 0)
        {
            // the colour here should corespond to the colour of the player's team/paint
            line.startColor = new Color(255,0,0,time);
            time -= Time.deltaTime;
            yield return null;
        }
        Destroy(line.gameObject);
    }
}
