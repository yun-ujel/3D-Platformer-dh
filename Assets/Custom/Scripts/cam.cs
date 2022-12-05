using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam : MonoBehaviour
{
    [Header("References")]
    public Transform orient;
    public Transform Player;
    public Transform PlayerPhysical;
    public Rigidbody rbody;
    //public Transform PlayerPhysical_Z;
    public PlayerController pc;

    [Header("Speed Values")]
    public float rotationSpeed;
    //public float rollRotationSpeed;

    [Header("Read Only")]
    public bool freezeRotation;
    public Vector3 inputDir;

    public rMode rotationMode;
    public enum rMode
    {
        plain,
        frozen,
        roll

    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //rotate orientation
        Vector3 viewDir = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        orient.forward = viewDir.normalized;

        if (rotationMode != rMode.frozen)
        {
            RotatePlayer();
        }
    }

    void RotatePlayer()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = orient.forward * verticalInput + orient.right * horizontalInput;

        if (inputDir != Vector3.zero && rotationMode == rMode.plain)
        {
            PlayerPhysical.forward = Vector3.Slerp(PlayerPhysical.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
        else if (rotationMode == rMode.roll)
        {
            PlayerPhysical.forward = Vector3.Slerp(PlayerPhysical.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            //PlayerPhysical_Z.Rotate(Vector3.right, rbody.velocity.magnitude * Time.deltaTime * rollRotationSpeed);
        }
        //if (rotationMode != rMode.roll && PlayerPhysical_Z.rotation.z != 0)
        //{
           // PlayerPhysical_Z.Rotate(Vector3.right, Vector3.Angle(PlayerPhysical_Z.up, PlayerPhysical.up));
        //}
        
    }
}
