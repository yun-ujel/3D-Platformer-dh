using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField]private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftShift;



    [Header("Speed Values")]

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpForce = 300f;

    [Header("Air Movement")]

    [SerializeField, Range(0f, 10f)] private float airMultiplier;
    [SerializeField, Range(0f, 10f)] private float upwardMovementMultiplier = 1f;
    [SerializeField, Range(0f, 10f)] private float downwardMovementMultiplier = 1f;
    private float defaultGravityScale = 1f;
    bool isJumping;

    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("References")]

    public Transform orient;

    public LayerMask groundLayer;
    bool grounded;

    Rigidbody rb;

    Vector3 moveDir;

    gravity grav;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grav = GetComponent<gravity>();
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, 1f + 0.2f, groundLayer);

        MyInput();
    }

    void FixedUpdate()
    {
        MovePlayer();
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
        if (grounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDir.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }


        ApplyDynamicGravity();
    }

    void ApplyDynamicGravity()
    {
        // DYNAMIC GRAVITY

        if ((rb.velocity.y > 0) && !grounded)
        {
            grav.gravityScale = upwardMovementMultiplier; // Rising Gravity
        }
        else if((rb.velocity.y < 0) && !grounded)
        {
            grav.gravityScale = downwardMovementMultiplier; // Falling Gravity
        }
        else
        {
            grav.gravityScale = defaultGravityScale;  // Default Gravity
        }


        // Variable Jump Height

        if (isJumping && grounded)
        {
            isJumping = false;
        }
        else if (isJumping && rb.velocity.y > 0f && !grounded  && !Input.GetKey(jumpKey)) 
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

        if (jumpBufferCounter > 0f && grounded) // "If able to jump"
        {
            Jump();
        }



        // CROUCHING




    }



}
