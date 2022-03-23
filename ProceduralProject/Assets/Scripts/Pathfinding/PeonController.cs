using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeonController : MonoBehaviour
{
    public Transform moveTarget;

    private List<Pathfinder.Node> pathToTarget = new List<Pathfinder.Node>();

    private bool shouldCheckAgain = true;

    private float checkAgainIn = 0;

    private LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        checkAgainIn -= Time.deltaTime;

        if (checkAgainIn <= 0)
        {
            shouldCheckAgain = true;
            checkAgainIn = 1;
        }
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (pathToTarget == null || pathToTarget.Count < 2) return;

        // TODO: Grab first item in path and move to node
        transform.position = Vector3.Lerp(transform.position, pathToTarget[1].position, 0.05f);

        Vector3 target = pathToTarget[1].position;
        target.y = 1;

        float d = (pathToTarget[1].position - transform.position).magnitude;
        if (d < 0.25f)
        {
            shouldCheckAgain = true;
        }
    }

    private void FindPath()
    {
        shouldCheckAgain = false;

        if (moveTarget == null || GridController.singleton == null) return;

        Pathfinder.Node start = GridController.singleton.Lookup(transform.position);
        Pathfinder.Node end = GridController.singleton.Lookup(moveTarget.position);

        if (start == null || end == null || start == end)
        {
            pathToTarget.Clear();
            return;
        }

        pathToTarget = Pathfinder.Solve(start, end);

        // render line
        Vector3[] positions = new Vector3[pathToTarget.Count];
        for (int i = 0; i < pathToTarget.Count; i++)
        {
            positions[i] = pathToTarget[i].position + new Vector3(0, 0.5f, 0);
        }
        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }
}
