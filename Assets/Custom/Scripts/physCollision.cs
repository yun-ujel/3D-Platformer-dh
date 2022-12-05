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

    [SerializeField] float rollRotationSpeed;

    Rigidbody rbody;
    [SerializeField] Transform zRotationObject;

    bool isRolling;

    private void Start()
    {
        capsule = GetComponentInChildren<CapsuleCollider>();
        sphere = GetComponentInChildren<SphereCollider>();
        rbody = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        if (isRolling)
        {
            zRotationObject.Rotate(Vector3.right, rbody.velocity.magnitude * Time.deltaTime * rollRotationSpeed);
        }
        else if (!isRolling && zRotationObject.rotation.z != 0)
        {
            zRotationObject.Rotate(Vector3.right, Vector3.Angle(zRotationObject.up, transform.up));
        }
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
        isRolling = true;
    }
    public void EndRoll()
    {
        mf.mesh = capsuleMesh;
        isRolling = false;
    }
}
