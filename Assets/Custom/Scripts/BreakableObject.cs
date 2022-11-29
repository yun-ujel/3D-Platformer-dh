using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    GameObject player;
    PlayerController pc;
    Rigidbody rb;

    private void Start()
    {
        player = GameObject.Find("player");
        pc = player.GetComponent<PlayerController>();
        rb = player.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision cl)
    {
        if(cl.gameObject == player && pc.hardLandingCounter > 0f)
        {
            BoxCollider bc = GetComponent<BoxCollider>();

            bc.enabled = false;

            rb.AddForce(Vector3.down * 30f, ForceMode.Impulse);

            Destroy(this.gameObject);
        }
    }
}
