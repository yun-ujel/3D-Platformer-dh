using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physCollision : MonoBehaviour
{
    public CapsuleCollider capsule;
    public SphereCollider sphere;
    public MeshFilter mf;

    [SerializeField] Mesh sphereMesh;
    [SerializeField] Mesh capsuleMesh;

    void Start()
    {
        
    }

    public void ColliderSphere()
    {
        capsule.enabled = false;
        sphere.enabled = true;

        mf.mesh = sphereMesh;

    }

    public void ColliderCapsule()
    {
        capsule.enabled = true;
        sphere.enabled = false;

        mf.mesh = capsuleMesh;

    }
}
