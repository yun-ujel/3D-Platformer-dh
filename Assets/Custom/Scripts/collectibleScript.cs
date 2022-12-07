using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectibleScript : MonoBehaviour
{
    MeshRenderer coneRend;
    MeshRenderer myRend;
    GameObject coneChild;
    Light ight;
    bool isDimming = false;
    float dim = 1f;

    public HUD hUD;
    public Transform lookAt;
    private void Start()
    {
        ight = GetComponent<Light>();
        ight.intensity = 0f;
        SphereCollider sphere = GetComponentInChildren<SphereCollider>();
        coneChild = sphere.gameObject;
        coneRend = coneChild.GetComponent<MeshRenderer>();
        coneRend.material.SetColor("_BaseColor", Color.black);
        myRend = GetComponent<MeshRenderer>();

        GameObject gamemanger = GameObject.Find("Game Manager");
        hUD = gamemanger.GetComponent<HUD>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDimming)
        {
            Debug.Log("Collected");

            transform.LookAt(lookAt);

            ight.intensity = 100f;
            coneRend.material.SetColor("_BaseColor", Color.white);

            isDimming = true;
            myRend.enabled = false;

            hUD.ScoreAdd();
        }
    }

    private void Update()
    {
        if (isDimming && dim > 0f)
        {
            dim = Mathf.MoveTowards(ight.intensity, 0f, 200f * Time.deltaTime);
            
            ight.intensity = dim;

            coneRend.material.SetColor("_BaseColor", new Color(dim * 0.01f, dim * 0.01f, dim * 0.01f));
        }
        else if (dim <= 0f)
        {
            Destroy(coneChild);
            Destroy(this.gameObject);
        }
        else
        {
            transform.Rotate(Vector3.up, 50f * Time.deltaTime);
        }
    }
}
