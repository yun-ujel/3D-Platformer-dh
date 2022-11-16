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

    public float rotationspeed;

    public bool freezeRotation;
    public Vector3 inputDir;

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

        if (!freezeRotation)
        {
            RotatePlayer();
        }
    }

    void RotatePlayer()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = orient.forward * verticalInput + orient.right * horizontalInput;

        if (inputDir != Vector3.zero)
        {
            PlayerPhysical.forward = Vector3.Slerp(PlayerPhysical.forward, inputDir.normalized, Time.deltaTime * rotationspeed);
        }
    }
}
