using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam : MonoBehaviour
{
    [Header("references")]
    public Transform orient;
    public Transform Player;
    public Transform PlayerPhysical;
    public Rigidbody rbody;

    public float rotationSpeed;
    public float rollRotationSpeed;

    public bool freezeRotation;
    public Vector3 inputDir;

    public float viewAngle;

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

        Debug.DrawRay(PlayerPhysical.position, PlayerPhysical.forward, Color.red, 10f);
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

            viewAngle = Vector3.Angle(orient.forward, PlayerPhysical.forward);


            
            //rbody.MoveRotation(rbody.rotation * Quaternion.Euler(new Vector3(0f, viewAngle, 0f) * Time.fixedDeltaTime));
        }
    }
}
