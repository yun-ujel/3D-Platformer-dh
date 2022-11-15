using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpForce = 300f;
    [SerializeField, Range(-5f, 5f)] private float groundDrag = 2f;
    [SerializeField, Range(0f, 10f)] private float airMultiplier;

    public Transform orient;

    Rigidbody rb;

    Vector3 moveDir;

    public LayerMask ground;
    bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, 1f + 0.2f, ground);

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if(Input.GetKey(KeyCode.Space) && grounded)
        {
            Jump();
        }
    }


    void FixedUpdate()
    {
        MovePlayer();
    }

    private void Jump()
    {
        rb.AddForce(transform.up * (jumpForce * 0.01f), ForceMode.Impulse);


        
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
       
    }
}
