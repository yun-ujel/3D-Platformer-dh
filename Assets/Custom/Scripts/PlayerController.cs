using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Speed Values")]

    [SerializeField] private float walkSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float rollSpeed;

    private float currentMoveSpeed = 30f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float slamJumpForce = 16f;
    Vector3 moveDir;

    [Header("Air Movement")]
    [SerializeField, Range(0f, 10f)] private float airMultiplier;
    [SerializeField, Range(0f, 10f)] private float upwardMovementMultiplier = 1f;
    [SerializeField, Range(0f, 10f)] private float downwardMovementMultiplier = 4f;
    [SerializeField, Range(0f, 20f)] private float slamMovementMultiplier = 4f;
    private float defaultGravityScale = 1f;
    bool diveUsed;

    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float diveForce = 16f;

    [Header("Crouch")]

    [SerializeField, Range(0f, 1f)] private float slamBufferTime;
    private float slamBufferCounter;
    [SerializeField, Range(0f, 1f)] private float hardLandingTime;
    [HideInInspector]
    public float hardLandingCounter;
    [SerializeField, Range(0f, 1f)] private float rollHopCooldown;
    [SerializeField] private float rollHopForce = 4f;
    [SerializeField] private float hardRollForce = 36f;
    bool isSlam;

    [Header("References")]

    [SerializeField] private Transform orient;
    [SerializeField] private cam cam;
    [SerializeField] private LayerMask groundLayer;
    Rigidbody rb;
    gravity grav;
    [HideInInspector]
    public PlayerInput PI;

    [Header("Read Only")]

    public MovementState state;
    public float currentVelocity;
    public enum MovementState
    {
        slam,
        slamBuffer,
        dive,

        none,

        crouch,
        roll
    }

    public bool onGround;
    [HideInInspector]
    public bool inCrouch;
    [HideInInspector]
    public bool inRoll;
    [HideInInspector]
    public float timeSinceLastAction;
    [HideInInspector]
    public float jumpBufferCounter;
    //[HideInInspector]
    public bool isRising;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grav = GetComponent<gravity>();

        PI = GetComponent<PlayerInput>();
    }
    private void Update()
    {
        MyInput();
        StateHandler();

        currentVelocity = rb.velocity.magnitude;

        timeSinceLastAction -= Time.deltaTime;
    }
    void FixedUpdate()
    {
        MovePlayer();
    }
    private void StateHandler()
    {
        // Ground Check & Landing
        if (Physics.CheckBox(new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), new Vector3(0.2f, 0.7f, 0.2f), Quaternion.identity, groundLayer))
        {
            if (isSlam)
            {
                isSlam = false;

                BroadcastMessage("HardLanding");
            }
            else if (!onGround && !isRising)
            {
                Debug.Log("Landed");
            }

            if (!(state == MovementState.crouch || state == MovementState.roll))
            {
                state = MovementState.none;
            }

            onGround = true;
            diveUsed = false;

        }
        else
        {
            onGround = false;
        }

        // Air movement after Dive
        if(timeSinceLastAction < -0.8f && state == MovementState.dive)
        {
            Debug.Log("Exit Dive");

            state = MovementState.none;
        }
        else if(timeSinceLastAction < -1f && rb.velocity.y <= 1f && state == MovementState.slam)
        {
            state = MovementState.none;
        }

        // Hard Landing
        hardLandingCounter -= Time.deltaTime;


        // Rotation Management
        if(state == MovementState.none)
        {
            // Freeze Rotation, Unfreeze position
            rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
        }
        if (!(state == MovementState.none || state == MovementState.roll || state == MovementState.crouch))
        {
            cam.rotationMode = cam.rMode.frozen;
        }
        else if (state == MovementState.roll)
        {
            cam.rotationMode = cam.rMode.roll;
        }
        else
        {
            cam.rotationMode = cam.rMode.plain;
        }




    }
    private void MyInput()
    {
        // JUMPING
        if (PI.JumpPressed())
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        if (jumpBufferCounter > 0f && onGround) // "If able to jump"
        {
            if (hardLandingCounter > 0f)
            {
                BroadcastMessage("slamJump");
            }
            else if (state != MovementState.slam && hardLandingCounter < 0f)
            {
                BroadcastMessage("Jump");
            }
        }

        // Variable Jump Height
        if (isRising && rb.velocity.y > 0f && state == MovementState.none && !onGround && !PI.JumpHeld())
        {
            rb.AddForce(Vector3.down * (rb.velocity.y * 0.8f), ForceMode.Impulse); // Cancel out vertical momentum
            Debug.Log("Jump Cancelled");
            isRising = false;

        }
        else if (rb.velocity.y < 0f && isRising)
        {
            isRising = false;
        }

        // DIVING
        if (PI.DivePressed() && !onGround && state == MovementState.none && !diveUsed)
        {
            BroadcastMessage("Dive");
        }



        // CROUCHING
        if (onGround) 
        { 
            // ENTER CROUCH
            if (PI.CrouchPressed() && state != MovementState.roll)
            {
                BroadcastMessage("EnterCrouch");
            }

            // ENTER ROLL
            if (state == MovementState.roll && timeSinceLastAction < -rollHopCooldown && PI.DivePressed())
            {
                BroadcastMessage("RollHop");
            }
            else if (state == MovementState.crouch && PI.DiveHeld())
            {
                BroadcastMessage("StartRoll");
            }
        }
        // SLAM
        else if (!(state == MovementState.slam))
        {
            if (PI.CrouchPressed() && state != MovementState.slamBuffer)
            {
                BroadcastMessage("EndRoll");
                BroadcastMessage("ExitCrouch");

                rb.constraints = RigidbodyConstraints.FreezeAll;
                state = MovementState.slamBuffer;
                isSlam = true;

                //Invoke(nameof(Slam), slamBufferTime);
                slamBufferCounter = slamBufferTime;
            }
            else if (state == MovementState.slamBuffer)
            {
                slamBufferCounter -= Time.deltaTime;
            }

            if (slamBufferCounter < 0f && state == MovementState.slamBuffer)
            {
                Slam();
            }
        }
        // EXIT CROUCH
        if (state == MovementState.roll || state == MovementState.crouch)
        {
            if (PI.CrouchReleased() && hardLandingCounter < 0f)
            {
                BroadcastMessage("ExitCrouch");
            }
            else if (hardLandingCounter < 0f && !PI.CrouchHeld() && (state == MovementState.crouch))
            {
                BroadcastMessage("ExitCrouch");
            }
            else if (hardLandingCounter < -1f && !PI.CrouchHeld() && (state == MovementState.roll))
            {
                BroadcastMessage("ExitCrouch");
            }
        }
        // EXIT ROLL
        if (timeSinceLastAction < -0.3f && rb.velocity.magnitude < 2f && state == MovementState.roll && cam.inputDir == Vector3.zero)
        {
            Debug.Log("Exit Roll");

            BroadcastMessage("EndRoll");
            BroadcastMessage("EnterCrouch");
        }


        



    }
    private void MovePlayer()
    {

        moveDir = (orient.forward * Input.GetAxisRaw("Vertical")) + (orient.right * Input.GetAxisRaw("Horizontal"));


        if ((state == MovementState.none || state == MovementState.crouch) && onGround)
        {
            rb.AddForce(moveDir.normalized * currentMoveSpeed, ForceMode.Force);
        }
        else if (state == MovementState.none && !onGround)
        {
            rb.AddForce(moveDir.normalized * currentMoveSpeed * airMultiplier, ForceMode.Force);
        }
        else if (state == MovementState.roll)
        {
            rb.AddForce(moveDir.normalized * currentMoveSpeed, ForceMode.Impulse);
        }


        ApplyDynamicGravity();
    }
    void ApplyDynamicGravity()
    {
        // DYNAMIC GRAVITY
        if (state == MovementState.slam)
        {
            grav.gravityScale = slamMovementMultiplier; // Slam Gravity
        }
        else if ((rb.velocity.y > 0) && !onGround)
        {
            grav.gravityScale = upwardMovementMultiplier; // Rising Gravity
        }
        else if ((rb.velocity.y < 0) && !onGround)
        {
            grav.gravityScale = downwardMovementMultiplier; // Falling Gravity
        }
        else
        {
            grav.gravityScale = defaultGravityScale;  // Default Gravity
        }
    }






    // --- ACTION METHODS --- 

    // Air States
    void Slam()
    {
        timeSinceLastAction = 0f;

        rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        rb.AddForce(Vector3.down * 7f, ForceMode.Impulse);
        //transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);

        slamBufferCounter = slamBufferTime;

        state = MovementState.slam;
    }
    void HardLanding()
    {
        Debug.Log("Triggered Hard Landing");

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        hardLandingCounter = hardLandingTime;
        rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;

        BroadcastMessage("EnterCrouch");
    }

    void Dive()
    {
        timeSinceLastAction = 0f;

        state = MovementState.dive;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (cam.inputDir != Vector3.zero)
        {
            rb.AddForce(cam.orient.forward * diveForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(cam.PlayerPhysical.forward.normalized * diveForce, ForceMode.Impulse);
        }

        diveUsed = true;
    }
    void Jump()
    {
        BroadcastMessage("ExitCrouch");

        timeSinceLastAction = 0f;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * (jumpForce), ForceMode.Impulse);
        grav.gravityScale = upwardMovementMultiplier;

        jumpBufferCounter = 0f;

        isRising = true;

        Debug.Log("Jump Triggered");
    }
    void slamJump()
    {
        BroadcastMessage("ExitCrouch");

        timeSinceLastAction = 0f;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * (slamJumpForce), ForceMode.Impulse);
        grav.gravityScale = upwardMovementMultiplier;

        jumpBufferCounter = 0f;

        isRising = true;

        Debug.Log("Jump Triggered");
    }


    // Ground States
    void StartRoll()
    {
        state = MovementState.roll;

        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        currentMoveSpeed = rollSpeed * 0.05f;

        BroadcastMessage("RollHop");
    }
    void RollHop()
    {
        timeSinceLastAction = 0f;

        if (hardLandingCounter < 0f)
        {
            rb.AddForce(cam.PlayerPhysical.forward.normalized * rollHopForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(cam.PlayerPhysical.forward.normalized * hardRollForce, ForceMode.Impulse);
        }

        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
    }

    void EnterCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
        currentMoveSpeed = crouchSpeed;

        state = MovementState.crouch;

        Debug.Log("Enter Crouch");
    }
    void ExitCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        currentMoveSpeed = walkSpeed;

        if (state == MovementState.roll)
        {
            BroadcastMessage("EndRoll");
            state = MovementState.none;
        }
        else if (state == MovementState.crouch)
        {
            state = MovementState.none;
        }

        Debug.Log("Exit Crouch");
    }

    void Bounce()
    {
        if (isSlam)
        {
            isSlam = false;

            BroadcastMessage("HardLanding");
        }
    }
}
