using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bouncingPad : MonoBehaviour
{
    GameObject player;
    Rigidbody rb;
    private void Start()
    {
        player = GameObject.Find("player");
        rb = player.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision cl)
    {
        if (cl.gameObject == player)
        {
            rb.AddForce(Vector3.up * 110f, ForceMode.Impulse);

            Debug.Log("bounce");
        }
    }
}
