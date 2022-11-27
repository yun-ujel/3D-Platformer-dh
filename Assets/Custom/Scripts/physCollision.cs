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

    public void ColliderSphere()
    {
        capsule.enabled = false;
        sphere.enabled = true;
    }

    public void ColliderCapsule()
    {
        capsule.enabled = true;
        sphere.enabled = false;
    }

    public void meshSphere()
    {
        mf.mesh = sphereMesh;
    }
    public void meshCapsule()
    {
        mf.mesh = capsuleMesh;
    }
}
