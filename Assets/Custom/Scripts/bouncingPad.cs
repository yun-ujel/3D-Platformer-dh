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
        if (cl.gameObject == player && Physics.CheckBox(new Vector3(transform.position.x, transform.position.y + (transform.localScale.y * 6f), transform.position.z), new Vector3(transform.localScale.x * 15f, transform.localScale.y * 5f, transform.localScale.z * 16f)))
        {
            rb.AddForce(Vector3.up * 95f, ForceMode.Impulse);

            player.BroadcastMessage("Bounce");
        }
    }
}
