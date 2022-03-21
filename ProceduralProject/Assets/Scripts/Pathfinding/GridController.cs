using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(LineRenderer))]
public class GridController : MonoBehaviour
{
    delegate Pathfinder.Node LookupDelegate(int x, int y);

    public TerrainCube cubePrefab;
    public Transform helperStart;
    public Transform helperEnd;

    private TerrainCube[,] cubes;
    private LineRenderer line;

    private void Start()
    {
        MakeGrid();
        line = GetComponent<LineRenderer>();
    }

    void MakeGrid()
    {
        int size = 19;
        cubes = new TerrainCube[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                cubes[x,z] = Instantiate(cubePrefab, new Vector3(x, 0, z), Quaternion.identity);
            }
        }
    }

    public void MakeNodes()
    {
        Pathfinder.Node[,] nodes = new Pathfinder.Node[cubes.GetLength(0), cubes.GetLength(1)];

        for (int x = 0; x < cubes.GetLength(0); x++)
        {
            for (int z = 0; z < cubes.GetLength(1); z++)
            {
                Pathfinder.Node n = new Pathfinder.Node();

                n.position = cubes[x, z].transform.position;

                nodes[x, z] = n;
            }
        }

        LookupDelegate lookup = (x, y) => 
        {
            if (x < 0) return null;
            if (y < 0) return null;
            if (x >= nodes.GetLength(0)) return null;
            if (y >= nodes.GetLength(1)) return null;
            return nodes[x, y];
        };

        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int z = 0; z < nodes.GetLength(1); z++)
            {
                Pathfinder.Node n = nodes[x, z];

                Pathfinder.Node neighbor1 = lookup(x + 1, z);
                Pathfinder.Node neighbor2 = lookup(x, z + 1);
                Pathfinder.Node neighbor3 = lookup(x - 1, z);
                Pathfinder.Node neighbor4 = lookup(x, z - 1);

                if (neighbor1 != null) n.neighbors.Add(neighbor1);
                if (neighbor2 != null) n.neighbors.Add(neighbor2);
                if (neighbor3 != null) n.neighbors.Add(neighbor3);
                if (neighbor4 != null) n.neighbors.Add(neighbor4);
            }
        }

        Pathfinder.Node start = Lookup(helperStart.position, nodes);
        Pathfinder.Node end = Lookup(helperEnd.position, nodes);

        List<Pathfinder.Node> path = Pathfinder.Solve(start, end);

        Vector3[] positions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            positions[i] = path[i].position + new Vector3(0, 0.5f, 0);
        }
        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }

    public Pathfinder.Node Lookup(Vector3 pos, Pathfinder.Node[,] nodes)
    {
        float w = 1;
        float h = 1;

        int x = (int)(pos.x / w);
        int y = (int)(pos.z / h);

        if (x < 0 || y < 0 || x >= nodes.GetLength(0) || y >= nodes.GetLength(1)) return null;

        return nodes[x, y];
    }
}

[CustomEditor(typeof(GridController))]
class GridControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("find a path"))
        {
            (target as GridController).MakeNodes();
        }
    }
}
