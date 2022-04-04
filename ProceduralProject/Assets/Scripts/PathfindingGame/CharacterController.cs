using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    private Rigidbody rb;

    public float speed = 3;
    public float maxSpeed = 2;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        DoInput();
    }

    private void DoInput()
    {
        bool pressedKey = false;
        Vector2 dir = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            pressedKey = true;
            dir += new Vector2(0, 1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            pressedKey = true;
            dir += new Vector2(-1, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            pressedKey = true;
            dir += new Vector2(0, -1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            pressedKey = true;
            dir += new Vector2(1, 0);
        }

        if (!pressedKey) 
            DampMovement();
        else
            MoveDirection(dir);
    }

    private void MoveDirection(Vector2 dir)
    {
        if (dir == Vector2.zero) return;
       
        rb.AddForce(new Vector3(dir.x, 0, dir.y) * speed);
        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            float damp = maxSpeed / rb.velocity.magnitude;
            rb.velocity *= damp;
        }
    }

    private void DampMovement()
    {
        rb.velocity /= 1.01f;
    }
}
