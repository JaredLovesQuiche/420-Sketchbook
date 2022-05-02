using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
public class Orb : MonoBehaviour
{
    private SimpleVis1 vis;
    private Rigidbody rb;

    private void Start()
    {
        vis = SimpleVis1.vis;
        GetComponent<MeshRenderer>().material.SetFloat("_TimeOffset", Random.Range(0, 2 * Mathf.PI));
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 ToVis = vis.transform.position - transform.position;
        Vector3 dirToVis = ToVis.normalized;

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.1f);

        rb.AddForce(100 * Time.deltaTime * dirToVis);
    }

    public void UpdateAudioData(float value)
    {
        transform.localScale = Vector3.one * (transform.localScale.x + value);
    }
}
