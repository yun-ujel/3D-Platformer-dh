using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakable : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] PlayerController pc;

    private void OnCollisionEnter(Collision cl)
    {
        if(cl.gameObject == player && pc.hardLandingCounter > 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
