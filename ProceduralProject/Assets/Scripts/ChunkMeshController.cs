using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ChunkMeshController : MonoBehaviour
{
    public enum BlendMode
    {
        AVERAGE,
        ADD,
        MULTIPLY,
        DIVIDE,
        SUBTRACT,
    }

    [System.Serializable]
    public class NoiseField
    {
        public bool enabled = true;
        public BlendMode blendMode = BlendMode.ADD;
        public Vector3 offset;
        public float zoom = 20;
        public float flattenAmount;
        public float flattenOffset;
    }

    public int resolution = 10;
    public float densityThreshold = 0.5f;

    public NoiseField[] fields;

    private MeshFilter meshFilter;
    private MeshCollider meshCol;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCol = GetComponent<MeshCollider>();

        BuildMesh();
    }

    private void OnValidate()
    {
        BuildMesh();
    }

    void BuildMesh()
    {
        bool[,,] voxels = new bool[resolution, resolution, resolution];

        for (int x = 0; x < voxels.GetLength(0); x++)
        {
            for (int y = 0; y < voxels.GetLength(1); y++)
            {
                for (int z = 0; z < voxels.GetLength(2); z++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    float density = 0;

                    foreach (NoiseField field in fields)
                    {
                        if (!enabled) continue;
                        Vector3 noisePos = (pos + transform.position) / field.zoom + field.offset;

                        float d = Noise.perlin3D(noisePos);
                        d -= ((y + field.flattenOffset) / 100.0f) * field.flattenAmount;

                        switch (field.blendMode)
                        {
                            case BlendMode.AVERAGE:
                                density = (density + d) / 2;
                                break;
                            case BlendMode.ADD:
                                density += d;
                                break;
                            case BlendMode.MULTIPLY:
                                density *= d;
                                break;
                            case BlendMode.DIVIDE:
                                density /= d;
                                break;
                            case BlendMode.SUBTRACT:
                                density -= d;
                                break;
                        }
                    }

                    voxels[x, y, z] = (density > densityThreshold);
                }
            }
        }

        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> UVs = new List<Vector2>();
        List<int> indices = new List<int>();

        for (int x = 0; x < voxels.GetLength(0); x++)
        {
            for (int y = 0; y < voxels.GetLength(1); y++)
            {
                for (int z = 0; z < voxels.GetLength(2); z++)
                {
                    if (voxels[x, y, z])
                    {
                        byte sides = 0;

                        if (!Lookup(voxels, x, y + 1, z)) sides |= 01;
                        if (!Lookup(voxels, x, y - 1, z)) sides |= 02;
                        if (!Lookup(voxels, x + 1, y, z)) sides |= 04;
                        if (!Lookup(voxels, x - 1, y, z)) sides |= 08;
                        if (!Lookup(voxels, x, y, z + 1)) sides |= 16;
                        if (!Lookup(voxels, x, y, z - 1)) sides |= 32;

                        AddCube(new Vector3(x, y, z), sides, verts, norms, UVs, indices);
                    }
                }
            }
        }

        Mesh mesh = new Mesh();

        mesh.vertices = verts.ToArray();
        mesh.normals = norms.ToArray();
        mesh.uv = UVs.ToArray();
        mesh.triangles = indices.ToArray();
        if (!meshFilter) meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        if (!meshCol) meshCol = GetComponent<MeshCollider>();
        meshCol.sharedMesh = mesh;
    }

    bool Lookup(bool[,,] arr, int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0) return false;
        if (x >= arr.GetLength(0) || y >= arr.GetLength(1) || z >= arr.GetLength(2)) return false;

        return arr[x, y, z];
    }

    void AddCube(Vector3 pos, byte sides, List<Vector3> verts, List<Vector3> norms, List<Vector2> UVs, List<int> indices)
    {
        if ((sides & 01) > 0) AddQuad(pos + new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 0), verts, norms, UVs, indices);
        if ((sides & 02) > 0) AddQuad(pos - new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 180), verts, norms, UVs, indices);
        if ((sides & 04) > 0) AddQuad(pos + new Vector3(0.5f, 0, 0), Quaternion.Euler(90, 0, -90), verts, norms, UVs, indices);
        if ((sides & 08) > 0) AddQuad(pos - new Vector3(0.5f, 0, 0), Quaternion.Euler(90, 0, 90), verts, norms, UVs, indices);
        if ((sides & 16) > 0) AddQuad(pos + new Vector3(0, 0, 0.5f), Quaternion.Euler(90, 0, 0), verts, norms, UVs, indices);
        if ((sides & 32) > 0) AddQuad(pos - new Vector3(0, 0, 0.5f), Quaternion.Euler(-90, 0, 0), verts, norms, UVs, indices);
    }

    void AddQuad(Vector3 pos, Quaternion rot, List<Vector3> verts, List<Vector3> norms, List<Vector2> UVs, List<int> indices)
    {
        int num = verts.Count;

        verts.Add(rot * new Vector3( 0.5f, 0,  0.5f) + pos);
        verts.Add(rot * new Vector3( 0.5f, 0, -0.5f) + pos);
        verts.Add(rot * new Vector3(-0.5f, 0, -0.5f) + pos);
        verts.Add(rot * new Vector3(-0.5f, 0,  0.5f) + pos);

        indices.Add(num + 0);
        indices.Add(num + 1);
        indices.Add(num + 3);
        indices.Add(num + 1);
        indices.Add(num + 2);
        indices.Add(num + 3);

        norms.Add(rot * new Vector3(0, 1, 0));
        norms.Add(rot * new Vector3(0, 1, 0));
        norms.Add(rot * new Vector3(0, 1, 0));
        norms.Add(rot * new Vector3(0, 1, 0));

        UVs.Add(new Vector2(1, 0));
        UVs.Add(new Vector2(1, 1));
        UVs.Add(new Vector2(0, 1));
        UVs.Add(new Vector2(0, 0));
    }

    void BuildMeshQuad()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = new Vector3[]
        {
            new Vector3( 0.5f, 0.0f,  0.5f),
            new Vector3( 0.5f, 0.0f, -0.5f),
            new Vector3(-0.5f, 0.0f, -0.5f),
            new Vector3(-0.5f, 0.0f, -0.5f),
        };

        mesh.triangles = new int[]
        {
            3, 1, 0,
            2, 1, 3,
        };

        mesh.normals = new Vector3[]
        {
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
        };

        mesh.uv = new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        meshFilter.mesh = mesh;
    }
}
