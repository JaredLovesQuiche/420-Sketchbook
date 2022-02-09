using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BranchingType
{
    Random,
    Opposite,
    Alternate180,
    AlternateFib,
    WhorledTwo,
    WhorledThree,
}

public class InstanceCollection
{
    public List<CombineInstance> branchInstances = new List<CombineInstance>();
    public List<CombineInstance> leafInstances = new List<CombineInstance>();

    public void AddBranch(Mesh branchMesh, Matrix4x4 xform)
    {
        branchInstances.Add(new CombineInstance() { mesh = branchMesh, transform = xform });
    }
    public void AddLeaf(Mesh leafMesh, Matrix4x4 xform)
    {
        branchInstances.Add(new CombineInstance() { mesh = leafMesh, transform = xform });
    }
    public Mesh MakeMultiMesh()
    {
        Mesh branchMesh = new Mesh();
        branchMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        branchMesh.CombineMeshes(branchInstances.ToArray());

        Mesh leafMesh = new Mesh();
        leafMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        leafMesh.CombineMeshes(leafInstances.ToArray());

        Mesh finalMesh = new Mesh();
        finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        finalMesh.CombineMeshes();
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlantDemo2 : MonoBehaviour
{
    [Range(0, 100000000)]
    public int seed;

    public BranchingType branchType;

    [Range(1, 20)]
    public int iterations = 5;
    [Range(-30, 30)]
    public float turnDegrees = 10.0f;
    [Range(-45, 45)]
    public float twistDegrees = 10.0f;
    [Range(0, 1)]
    public float alignWithParent = 0.5f;
    [Range(1, 10)]
    public int branchNodeDist = 2;
    [Range(0, 10)]
    public int branchNodeTrunk = 1;

    private System.Random randGenerator;

    private float Rand()
    {
        return (float)randGenerator.NextDouble();
    }
    private float Rand(float min, float max)
    {
        return Rand() * (max - min) + min;
    }
    private float RandBell(float min, float max)
    {
        min /= 2;
        max /= 2;

        return Rand(min, max) + Rand(min, max);
    }
    private bool RandBool()
    {
        return (Rand() >= 0.5f);
    }

    private void Start()
    {
        Build();
    }

    private void OnValidate()
    {
        Build();
    }

    void Build()
    {
        randGenerator = new System.Random(seed);

        // make storage
        InstanceCollection instances = new InstanceCollection();

        // create mesh instances
        Grow(instances, Vector3.zero, Quaternion.identity, new Vector3(0.25f, 1, 0.25f), iterations);

        // combine instances together to make final mesh
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(instances.ToArray());

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter) meshFilter.mesh = mesh;
    }

    void Grow(InstanceCollection instances, Vector3 pos, Quaternion rot, Vector3 scale, int max, int num = 0, float nodeSpin = 0)
    {
        if (num >= max) return; // stop recursion

        // make a cube mesh, add to list
        Matrix4x4 xform = new Matrix4x4(pos, rot, scale);
        instances.AddBranch(MeshTools.MakeCube(), xform);

        // recursion
        float percentAtEnd = num++ / (float)max;
        Vector3 endpoint = xform.MultiplyPoint(Vector3.up);

        if ((pos - endpoint).sqrMagnitude < 0.1f) return; // too small, stop recursion

        bool hasNode = num >= branchNodeTrunk && ((num - branchNodeTrunk - 1) % branchNodeDist == 0);

        if (hasNode)
        {
            if (branchType == BranchingType.Alternate180) nodeSpin += 180.0f;
            if (branchType == BranchingType.AlternateFib) nodeSpin += 137.5f;
        }

        {
            Quaternion randRot = rot * Quaternion.Euler(turnDegrees, twistDegrees, 0);
            Quaternion upRot = Quaternion.RotateTowards(rot, Quaternion.identity, 45);

            Quaternion newRot = Quaternion.Lerp(randRot, upRot, percentAtEnd);

            Grow(instances, endpoint, newRot, scale * 0.9f, max, num, nodeSpin);
        }

        if (num > 1)
        {
            if (num % 2 == 1)
            {
                int howMany = 0;
                float degreesBetweenNodes = 0.0f;

                switch (branchType)
                {
                    case BranchingType.Random:
                        howMany = 1;
                        break;
                    case BranchingType.Opposite:
                        howMany = 2;
                        degreesBetweenNodes = 180.0f;
                        break;
                    case BranchingType.Alternate180:
                        howMany = 1;
                        degreesBetweenNodes = 180.0f;
                        break;
                    case BranchingType.AlternateFib:
                        howMany = 1;
                        degreesBetweenNodes = 137.5f;
                        break;
                    case BranchingType.WhorledTwo:
                        howMany = 2;
                        degreesBetweenNodes = 180.0f;
                        break;
                    case BranchingType.WhorledThree:
                        howMany = 3;
                        degreesBetweenNodes = 120.0f;
                        break;
                }

                float lean = Mathf.Lerp(90, 0, alignWithParent);

                for (int i = 0; i < howMany; i++)
                {
                    float spin = nodeSpin + degreesBetweenNodes * i;
                    Quaternion newRot = rot * Quaternion.Euler(lean, spin, 0);

                    float s = RandBell(0.5f, 0.95f);

                    Grow(instances, endpoint, newRot, scale * s, max, num, 90);
                }
            }
        }
    }
}