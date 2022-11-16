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
    bool isJumping;

    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("Slam")]
    [SerializeField, Range(0f, 1f)] private float slamBufferTime;
    bool isSlam;


    [Header("References")]

    public Transform orient;
    public cam cam;
    public LayerMask groundLayer;
    

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
        dive
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

        if (state == MovementState.slam || state == MovementState.slamBuffer || state == MovementState.dive)
        {
            cam.freezeRotation = true;
        }
        else
        {
            cam.freezeRotation = false;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void StateHandler()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1f + 0.2f, groundLayer))
        {
            state = MovementState.ground;
            rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;


            if (isSlam)
            {
                isSlam = false;
                transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
            }

        }
        else if(!(state == MovementState.slam || state == MovementState.slamBuffer || state == MovementState.dive))
        {
            state = MovementState.air;
        }
        

    }



    private void Jump()
    {
        rb.AddForce(transform.up * (jumpForce * 0.01f), ForceMode.Impulse);
        grav.gravityScale = upwardMovementMultiplier;

        isJumping |= true;
        
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
        else if((rb.velocity.y < 0) && state != MovementState.ground)
        {
            grav.gravityScale = downwardMovementMultiplier; // Falling Gravity
        }
        else
        {
            grav.gravityScale = defaultGravityScale;  // Default Gravity
        }


        // Variable Jump Height

        if (isJumping && state == MovementState.ground)
        {
            isJumping = false;
        }
        else if (isJumping && rb.velocity.y > 0f && state == MovementState.air && !Input.GetKey(jumpKey)) 
        {
            rb.AddForce(Vector3.down * (rb.velocity.y * 0.4f), ForceMode.Impulse); // Cancel out vertical momentum
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



        // CROUCHING
        if (state == MovementState.ground) 
        { 

            if (Input.GetKeyDown(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
                rb.AddForce(Vector3.down * 7f, ForceMode.Impulse);

                currentMoveSpeed = crouchSpeed;
            }
            if (Input.GetKeyUp(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);

                currentMoveSpeed = walkSpeed;
            }

        }
        else
        {
            if (Input.GetKeyDown(crouchKey))
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;

                state = MovementState.slamBuffer;
                isSlam = true;

                

                Invoke(nameof(Slam), slamBufferTime);

            }

        }

        // DIVING
        if (Input.GetKeyDown(KeyCode.E))
        {
            Dive();
        }



    }

    void Slam()
    {
        rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        rb.AddForce(Vector3.down * 7f, ForceMode.Impulse);
        transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);


        state = MovementState.slam; 

    }

    void Dive()
    {
        state = MovementState.dive;
        rb.AddForce(cam.PlayerPhysical.forward.normalized * 16f, ForceMode.Impulse);
    }

}
