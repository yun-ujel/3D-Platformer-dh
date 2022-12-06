using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectible : MonoBehaviour
{
    MeshRenderer coneRend;
    GameObject cone;
    Light ight;

    bool isDimming = false;
    private void Start()
    {
        ight = GetComponent<Light>();
        ight.intensity = 0f;

        cone = GameObject.Find("Cone");
        coneRend = cone.GetComponentInChildren<MeshRenderer>();

        coneRend.material.SetColor("_BaseColor", Color.black);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collected");

            ight.intensity = 100f;
            coneRend.material.SetColor("_BaseColor", Color.white);

            isDimming = true;
        }
    }

    private void Update()
    {
        if (isDimming)
        {
            float dim;

            dim = Mathf.MoveTowards(ight.intensity, 0f, 200f * Time.deltaTime);

            ight.intensity = dim;

            coneRend.material.SetColor("_BaseColor", new Color(dim * 0.01f, dim * 0.01f, dim * 0.01f));
        }
    }
}
