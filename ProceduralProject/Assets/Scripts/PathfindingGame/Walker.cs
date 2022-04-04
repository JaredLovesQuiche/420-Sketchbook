using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Walker : MonoBehaviour
{
    public float speed = 4;
    public float maxSpeed = 4;

    public Transform Target;

    private Rigidbody rb;
    private WalkerAI AI;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        AI = GetComponent<WalkerAI>();
    }

    private void Update()
    {
        MoveDirection(AI.RayPath(Target));
    }

    private void MoveDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        rb.AddForce(dir * speed);
        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            float damp = maxSpeed / rb.velocity.magnitude;
            rb.velocity *= damp;
        }
    }
}
