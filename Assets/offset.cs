using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class offset : MonoBehaviour
{
    MeshRenderer mr;
    Material mat;
    float counterx;
    float countery;

    Vector3 startPos;

    float counter;

    int moveDir = 1;

    float tTDC; // time till direction change

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mat = mr.material;
        startPos = transform.position;
    }
    void Update()
    {
        //mat.mainTextureOffset = new Vector2(mat.mainTextureOffset.x +Time.deltaTime * 0.2f, mat.mainTextureOffset.y +Time.deltaTime * 0.2f);
        //Debug.Log(mat.GetTextureOffset("_DetailAlbedoMap"));

        counterx += Time.deltaTime * Random.Range(0.1f, 0.4f);
        countery += Time.deltaTime * Random.Range(-0.5f, 0.5f);

        mat.SetTextureOffset("_DetailAlbedoMap", new Vector2(counterx, countery));


        if (tTDC < 0f || transform.position.y > startPos.y + 3f || transform.position.y < startPos.y - 1f)
        {
            if (tTDC < 0f)
            {
                if (moveDir == 1)
                {
                    moveDir = -1;
                }
                else
                {
                    moveDir = 1;
                }
                tTDC = Random.Range(0.1f, 5f);
            }
            else if (transform.position.y > startPos.y + 3f)
            {
                moveDir = -1;
                tTDC = Random.Range(0.1f, 5f);
            }
            else if (transform.position.y < startPos.y - 1f)
            {
                moveDir = 1;
                tTDC = Random.Range(0.1f, 5f);
            }
        }
        else
        {
            tTDC -= Time.deltaTime;
        }

        if (moveDir == 1)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * (Random.Range(0.5f, 1.5f)), transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * (Random.Range(0.5f, 1.5f)), transform.position.z);
        }

        //Debug.Log(tTDC);
    }
}
