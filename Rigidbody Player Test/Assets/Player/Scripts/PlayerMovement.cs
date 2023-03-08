using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    public float actualSpeed;
    public float runningSpeed;
    public float sprintSpeed;

    [SerializeField] private float runningVar;

    public float groundDrag;

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
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;


    public Transform orientation;

    [SerializeField] float horizontalInput;
    [SerializeField] float verticalInput;
    public bool stopMovement;

    Vector3 moveDirection;

    Rigidbody rb;

    public AnimController AnimController;
    [SerializeField] private bool landNext = true;
    [SerializeField] private bool jumpStart = true;

    public MovementState state;
    public enum MovementState
    {
        idle,
        running,
        sprinting,
        crouching_idle,
        crouching_walking,
        jump_start,
        jump_end,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        AnimController.AnimState(state);
        
        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }


    private void FixedUpdate()
    {
        actualSpeed = rb.velocity.magnitude;
        //Debug.Log(actualSpeed);
        MovePlayer();
        StateHandler();

        if (AnimController.animctr.GetCurrentAnimatorStateInfo(0).IsName("Jumping_Down_2") == true)
        {
            stopMovement = true; verticalInput = 0; horizontalInput = 0;
        }
        else stopMovement = false;
    }

    private void MyInput()
    {
        if (stopMovement == false)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            jumpStart = true;

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            //transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            //rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            //transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
            // Mode - Crouching Idle
            if (Input.GetKey(crouchKey) && grounded && horizontalInput == 0 && verticalInput == 0 && landNext == false && jumpStart == false)
            {
                state = MovementState.crouching_idle;
                moveSpeed = crouchSpeed;
            }

            // Mode - Crouching Walking
            else if (Input.GetKey(crouchKey) && grounded && (horizontalInput != 0 || verticalInput != 0) && landNext == false && jumpStart == false)
            {
                state = MovementState.crouching_walking;
                moveSpeed = crouchSpeed;
            }

            // Mode - Sprinting
            else if (grounded && (horizontalInput != 0 || verticalInput != 0) && Input.GetKey(sprintKey) && (!Input.GetKey(crouchKey)) && landNext == false && jumpStart == false)
            {
                state = MovementState.sprinting;
                moveSpeed = sprintSpeed;
            }

            // Mode - Running
            else if (grounded && (horizontalInput != 0 || verticalInput != 0) && (!Input.GetKey(crouchKey)) && landNext == false && jumpStart == false)
            {
                state = MovementState.running;
                moveSpeed = runningSpeed;
            }
            // Mode - Idle
            else if (grounded && horizontalInput == 0 && verticalInput == 0 && (!Input.GetKey(crouchKey)) && landNext == false && jumpStart == false)
            {
                state = MovementState.idle;
                moveSpeed = runningSpeed;
            }

            // Mode - Jump Start
            else if (jumpStart == true && grounded && landNext == false)
            {
                state = MovementState.jump_start;
            }

            // Mode - Jump End
            else if(landNext == true && grounded && jumpStart == false)
            {
                state = MovementState.jump_end;
                landNext = false;
                moveSpeed = 0;
            }

            // Mode - Air
            else
            {
                state = MovementState.air;
                landNext = true;
                jumpStart = false;
            }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    //will bee called from animation event
    public void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}