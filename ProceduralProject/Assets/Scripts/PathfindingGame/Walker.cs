using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Pathfinding))]
public class Walker : MonoBehaviour
{
    public float speed = 4;
    public float maxSpeed = 4;
    public float nodeDistanceSatisfaction = 0.2f;
    public float raypathUpdateTick = 0.1f;

    public Vector3 target;
    public Vector3 start;
    public Vector3 end;

    private GameObject[] leftExits;
    private GameObject[] rightExits;

    private Rigidbody rb;
    private WalkerAI AI;
    private Pathfinding pathfind;
    private Transform player;

    private List<PathfindTile> path;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        AI = GetComponent<WalkerAI>();
        pathfind = GetComponent<Pathfinding>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        leftExits = GameObject.FindGameObjectsWithTag("ExitLeft");
        rightExits = GameObject.FindGameObjectsWithTag("ExitRight");

        InvokeRepeating(nameof(UpdateRayPathing), 0.5f, raypathUpdateTick);

        FindPathStartEnd();
        path = pathfind.FindNewPath(start, end);
        if (path == null) ReachedEndOfPath();
        transform.position = start;
    }

    private void Update()
    {
        MoveTo(target);
    }

    private void UpdateRayPathing()
    {
        if (path == null) return;
        target = AI.RayPath(FindNextNodeInPath());
    }

    private void MoveTo(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        rb.AddForce(dir * speed);
        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            float damp = maxSpeed / rb.velocity.magnitude;
            rb.velocity *= damp;
        }
    }

    private Vector3 FindNextNodeInPath()
    {
        if (path == null) return new Vector3();
        if (path.Count <= 0)
        {
            ReachedEndOfPath();
        }
        if (Vector3.Distance(path[0].Position, transform.position) < nodeDistanceSatisfaction)
        {
            path.RemoveAt(0);
        }
        if (path.Count > 0)
            return path[0].Position;
        else
            ReachedEndOfPath();
        return new Vector3();
    }

    private void FindPathStartEnd()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            start = leftExits[Random.Range(0, leftExits.Length)].transform.position;
            end = rightExits[Random.Range(0, rightExits.Length)].transform.position;
            while (Vector3.Distance(start, player.position) > 50 || start.z < player.position.z - 5)
            {
                start = leftExits[Random.Range(0, leftExits.Length)].transform.position;
            }
            while (Vector3.Distance(start, end) > 30)
            {
                end = rightExits[Random.Range(0, rightExits.Length)].transform.position;
            }
        }
        else
        {
            start = rightExits[Random.Range(0, rightExits.Length)].transform.position;
            end = leftExits[Random.Range(0, leftExits.Length)].transform.position;
            while (Vector3.Distance(start, player.position) > 50 || start.z < player.position.z - 5)
            {
                start = rightExits[Random.Range(0, rightExits.Length)].transform.position;
            }
            while (Vector3.Distance(start, end) > 30)
            {
                end = leftExits[Random.Range(0, leftExits.Length)].transform.position;
            }
        }
    }

    private void ReachedEndOfPath()
    {
        StopAllCoroutines();
        WalkerSpawner.walkers.Remove(this);
        Destroy(gameObject);
    }
}
