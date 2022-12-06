using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropShadow : MonoBehaviour
{
    GameObject player;
    Transform playerTransform;
    public LayerMask groundLayer;
    void Start()
    {
        player = GameObject.Find("player");
        playerTransform = player.GetComponent<Transform>();
        groundLayer = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit raycastHit;

        Physics.Raycast(new Vector3(transform.position.x, playerTransform.position.y, transform.position.z), Vector3.down, out raycastHit, 210f, groundLayer);
        transform.position = new Vector3(playerTransform.position.x, raycastHit.point.y + 0.05f, playerTransform.position.z);
    }
}
