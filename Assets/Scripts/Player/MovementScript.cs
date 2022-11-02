using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HealthHandler))]
public class MovementScript : MonoBehaviour
{    
    [Header("Movement Values")]
    public float walkSpeed;
    //public float sprintSpeed;
    //public float slideSpeed;
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float groundDrag;
    public float airDrag;
    [Header("Health")]
    public float healthMultiplier;
    private HealthHandler playerHealth;
    [Header("Smooth Speed")]
    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;
    [Header("Dashing")]
    public float dashForce;
    public int dashCount;
    public float dashCooldown;
    /*[Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    private bool sliding;
    private Vector3 slideDirection;*/
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    //public KeyCode crouchKey = KeyCode.LeftControl;
    [Header("Ground Check")]
    public float playerHeight;
    public Vector2 legSize;
    public LayerMask whatIsGround;
    public bool grounded;
    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    [Header("Input")]
    public bool isClient = false;
    public float horizontalInput;
    public float verticalInput;
    Vector3 moveDirection;
    [Header("General")]
    public new Rigidbody rigidbody;
    public Transform playerObj;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerHealth = GetComponent<HealthHandler>();
        rigidbody.freezeRotation = true;
        readyToJump = true;
        //startYScale = playerObj.localScale.y;
    }

    void Update()
    {
        Vector3 startPos = playerObj.position + playerObj.forward * legSize.x;
        Vector3 rayDir = Vector3.down * playerHeight * 0.5f;
        RaycastHit ray;
        grounded = Physics.Raycast(playerObj.position + playerObj.forward * legSize.x, rayDir, out ray, playerHeight * 0.5f, whatIsGround);
        grounded = Physics.Raycast(playerObj.position - playerObj.forward * legSize.x, rayDir, out ray, playerHeight * 0.5f, whatIsGround);
        grounded = Physics.Raycast(playerObj.position + playerObj.right * legSize.y, rayDir, out ray, playerHeight * 0.5f, whatIsGround);
        grounded = Physics.Raycast(playerObj.position - playerObj.right * legSize.y, rayDir, out ray, playerHeight * 0.5f, whatIsGround);
        Debug.DrawRay(playerObj.position + playerObj.forward * legSize.x, rayDir, Color.blue, 0.01f);
        Debug.DrawRay(playerObj.position - playerObj.forward * legSize.x, rayDir, Color.blue, 0.01f);
        Debug.DrawRay(playerObj.position + playerObj.right * legSize.y, rayDir, Color.blue, 0.01f);
        Debug.DrawRay(playerObj.position - playerObj.right * legSize.y, rayDir, Color.blue, 0.01f);
        if(isClient) userInput();
        SpeedControl();
        StateHandler();
        if(grounded)
        {
            rigidbody.drag = groundDrag;
        }
        else
        {
            rigidbody.drag = airDrag;
        }
    }
    void FixedUpdate()
    {
        if(playerHealth != null)
        {
            healthMultiplier = Mathf.Clamp(Mathf.Pow(1.6f, playerHealth.currentHealth / playerHealth.baseHealth) -0.6f, 0.5f, 1);
        }
        movePlayer();
    }
    // I would like to move this to its own script, so that the movement script can be applied to the AI
    private void userInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            doJump();
        }
        if(Input.GetKeyDown(dashKey) && dashCount > 0)
        {
            doDash();
        }
    }
    public void doJump()
    {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);

    }
    public void doDash()
    {
            dashCount--;
            Dash();
            Invoke(nameof(ResetDash), dashCooldown);
    }
    private void StateHandler()
    {
        if (grounded)
        {
            //Debug.Log("Walking");
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            //Debug.Log("Air");
            //In the air
        }
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;
    }
    private IEnumerator SmoothSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            if(OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }
        moveSpeed = desiredMoveSpeed;
    }
    private void movePlayer()
    {
        moveDirection = rigidbody.transform.forward * verticalInput + rigidbody.transform.right * horizontalInput;
        if(OnSlope() && !exitingSlope)
        {
            rigidbody.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * healthMultiplier, ForceMode.Force);
            if(rigidbody.velocity.y > 0)
            {
                rigidbody.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        if(grounded)
        {
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * healthMultiplier, ForceMode.Force);
        }
        else if(!grounded)
        {
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * (healthMultiplier / 5) * airMultiplier, ForceMode.Force);
        }
    }
    private void SpeedControl()
    {
        if(OnSlope() && !exitingSlope)
        {
            if(rigidbody.velocity.magnitude > moveSpeed)
            {
                rigidbody.velocity = rigidbody.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rigidbody.velocity = new Vector3(limitedVel.x, rigidbody.velocity.y, limitedVel.z);
            }
        }

    }
    private void Jump()
    {
        exitingSlope = true;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
        rigidbody.AddForce(transform.up * jumpForce * healthMultiplier, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }
    private void Dash()
    {
        exitingSlope = true;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
        //desiredMoveSpeed += dashForce * healthMultiplier;
        rigidbody.AddForce(moveDirection * dashForce * healthMultiplier, ForceMode.Impulse);
    }
    private void ResetDash()
    {
        dashCount++;
        exitingSlope =false;
    }
    public bool OnSlope()
    {
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.3f), Color.cyan, 0.01f);
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
