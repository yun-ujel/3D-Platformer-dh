using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField]private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftShift;


    [Header("Speed Values")]

    [SerializeField] private float walkSpeed;
    [SerializeField] private float crouchSpeed;
    private float currentMoveSpeed = 30f;
    [SerializeField] private float jumpForce = 300f;

    [Header("Air Movement")]

    [SerializeField, Range(0f, 10f)] private float airMultiplier;
    [SerializeField, Range(0f, 10f)] private float upwardMovementMultiplier = 1f;
    [SerializeField, Range(0f, 10f)] private float downwardMovementMultiplier = 4f;
    [SerializeField, Range(0f, 20f)] private float slamMovementMultiplier = 4f;
    private float defaultGravityScale = 1f;
    bool isRising;

    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;


    [Header("Slam")]
    [SerializeField, Range(0f, 1f)] private float slamBufferTime;
    private float slamBufferCounter;
    bool isSlam;


    [Header("References")]

    public Transform orient;
    public cam cam;
    public LayerMask groundLayer;


    public physCollision phys;


    Rigidbody rb;
    gravity grav;

    Vector3 moveDir;

    public MovementState state;
    public enum MovementState
    {
        ground,
        air,
        slam,
        slamBuffer,
        dive,
        rolling
    }



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grav = GetComponent<gravity>();
    }

    private void Update()
    {
        MyInput();
        StateHandler();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void StateHandler()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1f + 0.2f, groundLayer) && (!Input.GetKey(KeyCode.C)))
        {

            if (isSlam)
            {
                isSlam = false;
                //transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
                state = MovementState.ground;
            }
            else if (state == MovementState.air && !isRising)
            {
                state = MovementState.ground;

                Debug.Log("Landed");
            }
            else if (state != MovementState.ground)
            {
                state = MovementState.ground;
            }



            

        }
        else if(!(state == MovementState.slam || state == MovementState.slamBuffer || state == MovementState.dive || state  == MovementState.rolling))
        {
            state = MovementState.air;
        }
        // Freeze Rotation, Unfreeze position
        if(state == MovementState.air || state == MovementState.ground)
        {
            rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
        }


        
        if (!(state == MovementState.air || state == MovementState.ground || state == MovementState.rolling))
        {
            cam.rotationMode = cam.rMode.frozen;
        }
        else if (state == MovementState.rolling)
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

        if (jumpBufferCounter > 0f && state == MovementState.ground) // "If able to jump"
        {
            Jump();
        }

        // Variable Jump Height

        if (isRising && rb.velocity.y > 0f && state == MovementState.air && !Input.GetKey(jumpKey))
        {
            rb.AddForce(Vector3.down * (rb.velocity.y * 0.8f), ForceMode.Impulse); // Cancel out vertical momentum
            Debug.Log("Jump Cancelled");
            isRising = false;

        }
        else if (rb.velocity.y < 0f && isRising)
        {
            isRising = false;
        }

        



        // CROUCHING
        if (state == MovementState.ground) 
        { 

            if (Input.GetKeyDown(crouchKey))
            {
                //transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
                rb.AddForce(Vector3.down * 7f, ForceMode.Impulse);

                currentMoveSpeed = crouchSpeed;
            }
            if (Input.GetKeyUp(crouchKey))
            {
                //transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);

                currentMoveSpeed = walkSpeed;
            }

        }
        else // if midair
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

        // DIVING
        if (Input.GetKeyDown(KeyCode.E) && state == MovementState.air)
        {
            Dive();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            state = MovementState.rolling;

            StartRoll();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            EndRoll();
        }



    }

    private void MovePlayer()
    {

        moveDir = (orient.forward * Input.GetAxisRaw("Vertical")) + (orient.right * Input.GetAxisRaw("Horizontal"));
        if (state == MovementState.ground)
        {
            rb.AddForce(moveDir.normalized * currentMoveSpeed, ForceMode.Force);
        }
        else if (state == MovementState.air)
        {
            rb.AddForce(moveDir.normalized * currentMoveSpeed * airMultiplier, ForceMode.Force);
        }
        else if (state == MovementState.rolling)
        {
            rb.AddForce(Vector3.Normalize(orient.forward * Input.GetAxisRaw("Vertical")) * currentMoveSpeed, ForceMode.Force);
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
        else if ((rb.velocity.y > 0) && state != MovementState.ground)
        {
            grav.gravityScale = upwardMovementMultiplier; // Rising Gravity
        }
        else if ((rb.velocity.y < 0) && state != MovementState.ground)
        {
            grav.gravityScale = downwardMovementMultiplier; // Falling Gravity
        }
        else
        {
            grav.gravityScale = defaultGravityScale;  // Default Gravity
        }



    }


    // Action Methods
    void Slam()
    {
        rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        rb.AddForce(Vector3.down * 7f, ForceMode.Impulse);
        //transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);

        slamBufferCounter = slamBufferTime;

        state = MovementState.slam; 

    }

    void Dive()
    {
        state = MovementState.dive;
        //rb.AddForce(cam.PlayerPhysical.forward.normalized * 16f, ForceMode.Impulse);

        if(cam.inputDir != Vector3.zero)
        {
            rb.AddForce(cam.orient.forward * 16f, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(cam.PlayerPhysical.forward.normalized * 16f, ForceMode.Impulse);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * (jumpForce), ForceMode.Impulse);
        grav.gravityScale = upwardMovementMultiplier;

        jumpBufferCounter = 0f;

        isRising = true;

        Debug.Log("Jump Triggered");
    }


    // Roll Methods
    void StartRoll()
    {
        phys.ColliderSphere();
        rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotationZ;
        
    }
    void EndRoll()
    {
        phys.ColliderCapsule();
        transform.rotation = Quaternion.identity;
        
    }
}
