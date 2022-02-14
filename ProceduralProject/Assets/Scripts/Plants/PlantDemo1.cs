using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PlantDemo1 : MonoBehaviour
{
    public GameObject leaves;

    [Range(1, 20)]
    public int iterations;
    [Range(5, 30)]
    public float spreadDegrees;

    public void Build(Vector3 location)
    {
        if (!Application.isPlaying) return;
        // make storage
        List<CombineInstance> instances = new List<CombineInstance>();

        // create mesh instances
        Grow(instances, Vector3.zero, Quaternion.identity, new Vector3(0.25f, 1, 0.25f), iterations, location);

        // combine instances together to make final mesh
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(instances.ToArray());

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter) meshFilter.mesh = mesh;
    }

    void Grow(List<CombineInstance> instances, Vector3 pos, Quaternion rot, Vector3 scale, int max, Vector3 location, int num = 0)
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

        if ((pos - endpoint).sqrMagnitude < 0.1f)
        {
            GameObject leaf = Instantiate(leaves, this.transform);
            leaf.transform.position = endpoint + location;
            leaf.transform.localScale = Vector3.one * UnityEngine.Random.Range(1.0f, 3.0f);
            leaf.GetComponent<MeshRenderer>().material.color = new Color(UnityEngine.Random.Range(0.2f, 0.5f), UnityEngine.Random.Range(0.5f, 1.0f), UnityEngine.Random.Range(0.2f, 0.5f));
            return;
        }
        {
            Quaternion randRot = rot * Quaternion.Euler(spreadDegrees, Random.Range(-90, 90), 0);
            Quaternion upRot = Quaternion.RotateTowards(rot, Quaternion.identity, 45);

            Quaternion newRot = Quaternion.Lerp(randRot, upRot, percentAtEnd);

            Grow(instances, endpoint, newRot, scale * 0.9f, max, location, num);
        }

        if (num > 1)
        {
            if (num % 2 == 1)
            {
                float degrees = Random.Range(-1, 2) * 90;
                Quaternion newRot = Quaternion.LookRotation(endpoint - pos) * Quaternion.Euler(0, 0, degrees);
                Grow(instances, endpoint, newRot, scale, max, location, num);
            }
        }
    }
}