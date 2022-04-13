using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LineRenderer))]
class SimpleVis1 : MonoBehaviour
{
    public float radius = 1.0f;
    public float amp = 4.0f;
    public float orbHeight = 10.0f;
    public float orbSize = 20.0f;

    public int numBands = 512;
    public Transform orbPrefab;

    private AudioSource player;
    private LineRenderer line;
    private List<Transform> orbs = new List<Transform>();

    private void Start()
    {
        player = GetComponent<AudioSource>();
        line = GetComponent<LineRenderer>();

        // spawn orb for every frequency band:
        Quaternion q = Quaternion.identity;
        for (int i = 0; i < numBands; i++)
        {
            Vector3 p = new Vector3(0, i * orbHeight / numBands, 0);
            orbs.Add(Instantiate(orbPrefab, p, q, transform));
        }
    }

    private void Update()
    {
        UpdateWaveForm();
        UpdateFreqBand();
    }

    private void UpdateFreqBand()
    {
        float[] bands = new float[numBands];
        player.GetSpectrumData(bands, 0, FFTWindow.BlackmanHarris);

        for (int i = 0; i < orbs.Count; i++)
        {
            float p = (i + 1) / (float)numBands;
            orbs[i].localScale = orbSize * bands[i] * p * Vector3.one;
        }
    }

    private void UpdateWaveForm()
    {
        int samples = 1024;
        float[] data = new float[samples];
        player.GetOutputData(data, 0);

        Vector3[] points = new Vector3[samples];

        for (int i = 0; i < data.Length; i++)
        {
            float sample = data[i];

            float rads = Mathf.PI * 2 * i / samples;

            float x = Mathf.Cos(rads) * radius;
            float z = Mathf.Sin(rads) * radius;

            float y = sample * amp;

            points[i] = new Vector3(x, y, z);
        }

        line.positionCount = points.Length;
        line.SetPositions(points);
    }
}
