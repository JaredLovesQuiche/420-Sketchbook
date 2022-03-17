using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    delegate Pathfinder.Node LookupDelegate(int x, int y);

    public TerrainCube cubePrefab;

    private TerrainCube[,] cubes;

    private void Start()
    {
        MakeGrid();
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

    void MakeNodes()
    {
        Pathfinder.Node[,] nodes = new Pathfinder.Node[cubes.GetLength(0), cubes.GetLength(1)];

        for (int x = 0; x < cubes.GetLength(0); x++)
        {
            for (int z = 0; z < cubes.GetLength(1); z++)
            {
                Pathfinder.Node n = new Pathfinder.Node();

                n.position = cubes[x, z].transform.position;
                n.moveCost = cubes[x, z].isSolid ? 9999 : 1;

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
                Pathfinder.Node neighbor2 = lookup(x + 1, z);
                Pathfinder.Node neighbor3 = lookup(x + 1, z);
                Pathfinder.Node neighbor4 = lookup(x + 1, z);

                if (neighbor1 != null) n.neighbors.Add(neighbor1);
                if (neighbor2 != null) n.neighbors.Add(neighbor2);
                if (neighbor3 != null) n.neighbors.Add(neighbor3);
                if (neighbor4 != null) n.neighbors.Add(neighbor4);
            }
        }
    }
}
