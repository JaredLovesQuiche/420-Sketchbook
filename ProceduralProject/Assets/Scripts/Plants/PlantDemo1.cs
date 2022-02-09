using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlantDemo1 : MonoBehaviour
{
    [Range(1, 20)]
    public int iterations = 5;
    [Range(5, 30)]
    public float spreadDegrees = 10.0f;

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
        // make storage
        List<CombineInstance> instances = new List<CombineInstance>();

        // create mesh instances
        Grow(instances, Vector3.zero, Quaternion.identity, new Vector3(0.25f, 1, 0.25f), iterations);

        // combine instances together to make final mesh
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(instances.ToArray());

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter) meshFilter.mesh = mesh;
    }

    void Grow(List<CombineInstance> instances, Vector3 pos, Quaternion rot, Vector3 scale, int max, int num = 0)
    {
        if (num >= max) return; // stop recursion

        // make a cube mesh, add to list

        CombineInstance inst = new CombineInstance();
        inst.mesh = MeshTools.MakeCube();
        inst.transform = Matrix4x4.TRS(pos, rot, scale);
        instances.Add(inst);

        // recursion
        float percentAtEnd = num++ / (float)max;
        Vector3 endpoint = inst.transform.MultiplyPoint(Vector3.up);

        if ((pos - endpoint).sqrMagnitude < 0.01f) return; // too small, stop recursion

        {
            Quaternion randRot = rot * Quaternion.Euler(spreadDegrees, Random.Range(-90, 90), 0);
            Quaternion upRot = Quaternion.RotateTowards(rot, Quaternion.identity, 45);

            Quaternion newRot = Quaternion.Lerp(randRot, upRot, percentAtEnd);

            Grow(instances, endpoint, newRot, scale * 0.9f, max, num);
        }

        if (num > 1)
        {
            if (num % 2 == 1)
            {
                float degrees = Random.Range(-1, 2) * 90;
                Quaternion newRot = Quaternion.LookRotation(endpoint - pos) * Quaternion.Euler(0, 0, degrees);
                Grow(instances, endpoint, newRot, scale, max, num);
            }
        }
    }
}