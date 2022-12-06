using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectible : MonoBehaviour
{
    GameObject player;

    private void Start()
    {
        player = GameObject.Find("player");
    }

    private void OnTriggerEnter(Collider cl)
    {
        if (cl.gameObject == player)
        {
            Destroy(this.gameObject);
        }
    }
}
