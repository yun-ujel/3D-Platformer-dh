using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physCollision : MonoBehaviour
{
    private CapsuleCollider capsule;
    private SphereCollider sphere;
    public MeshFilter mf;

    [SerializeField] Mesh sphereMesh;
    [SerializeField] Mesh capsuleMesh;

    private void Start()
    {
        capsule = GetComponentInChildren<CapsuleCollider>();
        sphere = GetComponentInChildren<SphereCollider>();
    }



    public void EnterCrouch()
    {
        capsule.enabled = false;
        sphere.enabled = true;
    }

    public void ExitCrouch()
    {
        capsule.enabled = true;
        sphere.enabled = false;
    }

    public void StartRoll()
    {
        mf.mesh = sphereMesh;
    }
    public void EndRoll()
    {
        mf.mesh = capsuleMesh;
    }
}
