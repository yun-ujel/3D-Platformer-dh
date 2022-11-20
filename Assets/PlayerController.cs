using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftShift;
    public KeyCode diveKey = KeyCode.E;

    [Header("Speed Values")]

    [SerializeField] private float walkSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float rollSpeed;

    private float currentMoveSpeed = 30f;
    [SerializeField] private float jumpForce = 300f;
    Vector3 moveDir;

    [Header("Air Movement")]
    public bool onGround;
    [SerializeField, Range(0f, 10f)] private float airMultiplier;
    [SerializeField, Range(0f, 10f)] private float upwardMovementMultiplier = 1f;
    [SerializeField, Range(0f, 10f)] private float downwardMovementMultiplier = 4f;
    [SerializeField, Range(0f, 20f)] private float slamMovementMultiplier = 4f;
    private float defaultGravityScale = 1f;
    bool isRising;

    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    public float diveForce = 16f;

    [Header("Crouch")]

    [SerializeField, Range(0f, 1f)] private float slamBufferTime;
    private float slamBufferCounter;

    [SerializeField, Range(0f, 1f)] private float hardLandingTime;
    private float hardLandingCounter;

    [SerializeField, Range(0f, 1f)] private float rollHopCooldown;
    public float rollHopForce = 4f;

    public bool inCrouch;
    public bool inRoll;

    bool isSlam;

    [Header("References")]

    public Transform orient;
    public cam cam;
    public physCollision phys;
    public LayerMask groundLayer;
    Rigidbody rb;
    gravity grav;
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

    float timeSinceLastAction;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grav = GetComponent<gravity>();
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
        if (Physics.CheckBox(new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), new Vector3(0.3f, 0.7f, 0.3f), Quaternion.identity, groundLayer))
        {
            if (isSlam)
            {
                isSlam = false;
                
                SlamLanding();
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

        }
        else
        {
            onGround = false;
        }

        // Air movement after Dive
        if(timeSinceLastAction < -0.5f && state == MovementState.dive)
        {
            Debug.Log("Exit Dive");

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
        if (Input.GetKeyDown(jumpKey))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        if (jumpBufferCounter > 0f && onGround && state == MovementState.none) // "If able to jump"
        {
            Jump();
        }

        // Variable Jump Height
        if (isRising && rb.velocity.y > 0f && state == MovementState.none && !onGround && !Input.GetKey(jumpKey))
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
        if (Input.GetKeyDown(diveKey) && !onGround && state == MovementState.none)
        {
            Dive();
        }



        // CROUCHING
        if (onGround) 
        { 
            // ENTER CROUCH
            if (Input.GetKeyDown(crouchKey) && state != MovementState.roll)
            {
                EnterCrouch();
                Debug.Log("Enter Crouch");
            }

            // ENTER ROLL
            if (Input.GetKeyDown(diveKey))
            {
                if (state == MovementState.crouch)
                {
                    StartRoll();
                }
                else if (state == MovementState.roll && timeSinceLastAction < -rollHopCooldown)
                {
                    RollHop();
                }
            }
        }
        // SLAM
        else if (state != MovementState.dive || state != MovementState.slam)
        {
            if (Input.GetKeyDown(crouchKey))
            {
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
            if (Input.GetKeyUp(crouchKey) || Input.GetKeyDown(jumpKey))
            {
                ExitCrouch();
                Debug.Log("Exit Crouch");
            }
            else if (hardLandingCounter < 0f && !Input.GetKey(crouchKey) && state == MovementState.crouch)
            {
                ExitCrouch();
                Debug.Log("Exit Crouch");
            }
        }
        // EXIT ROLL
        if (timeSinceLastAction < -0.3f && rb.velocity.magnitude < 2f && state == MovementState.roll && cam.inputDir == Vector3.zero)
        {
            Debug.Log("Exit Roll");

            EndRoll();
            EnterCrouch();
        }


        ///|| (hardLandingCounter < 0f && !Input.GetKey(crouchKey))



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
    void SlamLanding()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        hardLandingCounter = hardLandingTime;
        rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;

        EnterCrouch();
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
    }
    void Jump()
    {
        timeSinceLastAction = 0f;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * (jumpForce), ForceMode.Impulse);
        grav.gravityScale = upwardMovementMultiplier;

        jumpBufferCounter = 0f;

        isRising = true;

        Debug.Log("Jump Triggered");
    }


    // Ground States
    void StartRoll()
    {
        state = MovementState.roll;

        phys.meshSphere();

        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        currentMoveSpeed = rollSpeed * 0.05f;

        RollHop();
    }
    void EndRoll()
    {
        phys.meshCapsule();
    }
    void RollHop()
    {
        timeSinceLastAction = 0f;

        rb.AddForce(cam.PlayerPhysical.forward.normalized * rollHopForce, ForceMode.Impulse);

        rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
    }

    void EnterCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
        currentMoveSpeed = crouchSpeed;

        phys.ColliderSphere();

        state = MovementState.crouch;
    }
    void ExitCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        currentMoveSpeed = walkSpeed;

        phys.ColliderCapsule();

        if (state == MovementState.roll)
        {
            EndRoll();
            state = MovementState.none;
        }
        else if (state == MovementState.crouch)
        {
            state = MovementState.none;
        }
    }
}
