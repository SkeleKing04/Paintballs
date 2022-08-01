using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{    
    private float moveSpeed;
    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float groundDrag;
    private HealthHandler playerHealth;
    public float healthMultiplier;
    [Header("Smooth Speed")]
    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;
    //private int jumpLeeway;
    //public float jumpLeewayTimer;
    //private bool jumpLeewayStart;
    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    public Transform playerOrient;
    [Header("General")]
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    [Header("General")]
    public new Rigidbody rigidbody;
    public new CameraController camera;
    public Transform playerObj;

    public MovementStates state;
    public enum MovementStates
    {
        walking,
        sprinting,
        crouching,
        sliding,
        air
    }
    public bool sliding;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerHealth = GetComponent<HealthHandler>();
        rigidbody.freezeRotation = true;
        readyToJump = true;
        startYScale = playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        userInput();
        SpeedControl();
        StateHandler();
        if(grounded)
        {
            rigidbody.drag = groundDrag;
            //jumpLeeway = 0;
        }
        else
        {
            rigidbody.drag = 0;
            //if(jumpLeeway == 0 && readyToJump)
            //{
            //   jumpLeeway = 1;
            //    Invoke(nameof(RemoveLeeway), jumpLeewayTimer);
            //}
        }
    }
    void FixedUpdate()
    {
        healthMultiplier = Mathf.Clamp(Mathf.Pow(1.6f, playerHealth.currentHealth / 100) -0.6f, 0.5f, 1);
        movePlayer();
    }
    private void userInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            //jumpLeeway = 2;
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        if(Input.GetKeyDown(crouchKey))
        {
            playerObj.localScale = new Vector3(playerObj.localScale.x, crouchYScale, playerObj.localScale.z);
            rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if(Input.GetKeyUp(crouchKey))
        {
            playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
        }
    }
    private void StateHandler()
    {
        if (sliding)
        {
            state = MovementStates.sliding;
            if (OnSlope() && rigidbody.velocity.y < 0.1f) desiredMoveSpeed = slideSpeed;
            else desiredMoveSpeed = sprintSpeed;
        }
        else if(Input.GetKey(crouchKey))
        {
            state = MovementStates.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementStates.sprinting;
            desiredMoveSpeed = sprintSpeed;
            camera.DoFov(75f);
        }
        else if (grounded)
        {
            state = MovementStates.walking;
            desiredMoveSpeed = walkSpeed;
            camera.DoFov(60f);
        }
        else
        {
            state = MovementStates.air;
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
        moveDirection = playerOrient.forward * verticalInput + playerOrient.right * horizontalInput;
        if(OnSlope() && !exitingSlope)
        {
            rigidbody.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f * healthMultiplier, ForceMode.Force);
            if(rigidbody.velocity.y > 0)
            {
                rigidbody.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        if(grounded)
        {
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f * healthMultiplier, ForceMode.Force);
        }
        else if(!grounded)
        {
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f * (healthMultiplier / 5) * airMultiplier, ForceMode.Force);
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
    /*private void RemoveLeeway()
    {
        jumpLeeway = 2;
    }*/
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
