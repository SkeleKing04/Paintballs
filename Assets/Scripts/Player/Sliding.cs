using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private new Rigidbody rigidbody;
    private MovementScript movementScript;
    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    public float slideYScale;
    public float startYScale;
    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        movementScript = GetComponent<MovementScript>();
        startYScale = playerObj.localScale.y;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    private void FixedUpdate()
    {
        //if (movementScript.state == MovementScript.MovementStates.sliding)
    }
    /*public void StartSlide()
    {
        movementScript.sliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime * movementScript.healthMultiplier;
    }
    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if(!movementScript.OnSlope() || rigidbody.velocity.y > -0.1f)
        {
            rigidbody.AddForce(inputDirection.normalized * slideForce * (movementScript.healthMultiplier / (movementScript.healthMultiplier * 10)), ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else
        {
            rigidbody.AddForce(movementScript.GetSlopeMoveDirection(inputDirection).normalized * slideForce * (movementScript.healthMultiplier / (movementScript.healthMultiplier * 10)), ForceMode.Force);

        }

        if (slideTimer <= 0) StopSlide();
    }
    public void StopSlide()
    {
        movementScript.sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
    }*/
}
