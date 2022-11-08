using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam : MonoBehaviour
{
    [Header("references")]
    public Transform orient;
    public Transform Player;
    public Transform Player_obj;
    public Rigidbody rbody;

    public float rotationspeed;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //rotate orientation
        Vector3 veiwDir = Player.position - new Vector3(transform.position.x, Player.position.y, transform.position.z);
        orient.forward = veiwDir.normalized;

        //rotate player
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = orient.forward * verticalInput + orient.right * horizontalInput;

        if(inputDir!= Vector3.zero)
        {
            Player_obj.forward = Vector3.Slerp(Player_obj.forward, inputDir.normalized, Time.deltaTime * rotationspeed);
        }
    }
}
