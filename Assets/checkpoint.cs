using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    GameObject player;
    Transform pt;

    bool hasPassedCheckpoint = false;
    void Start()
    {
        player = GameObject.Find("player");
        pt = player.GetComponent<Transform>();
    }
    void Update()
    {
        if(pt.position.y < -17f && !hasPassedCheckpoint)
        {
            pt.position = new Vector3(0f, 2f, -10f);
        }
        else if(pt.position.y < -17f && hasPassedCheckpoint)
        {
            pt.position = new Vector3(-8f, 37.5f, 145f);
        }

        if(pt.position.y > 36f && !hasPassedCheckpoint)
        {
            hasPassedCheckpoint = true;
        }
    }
}
