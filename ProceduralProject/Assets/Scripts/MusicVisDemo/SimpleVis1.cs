using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LineRenderer))]
class SimpleVis1 : MonoBehaviour
{
    static public SimpleVis1 vis { get; private set; }

    public float ringRadius = 1.0f;
    public float ringHeight = 4.0f;
    public float orbHeight = 10.0f;
    public float orbSize = 20.0f;

    public float avgAmp = 0;

    public int numBands = 512;
    public Orb orbPrefab;

    private AudioSource player;
    private LineRenderer line;
    private List<Orb> orbs = new List<Orb>();

    public PostProcessing ppShader;

    private void Start()
    {
        if (vis != null)
        {
            Destroy(gameObject);
        }
        vis = this;

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

    private void OnDestroy()
    {
        if (vis == this) vis = null;
    }

    private void UpdateFreqBand()
    {
        float[] bands = new float[numBands];
        player.GetSpectrumData(bands, 0, FFTWindow.BlackmanHarris);

        float avgAmp = 0;

        for (int i = 0; i < orbs.Count; i++)
        {
            //float p = (i + 1) / (float)numBands;
            //orbs[i].localScale = orbSize * bands[i] * p * Vector3.one;
            //orbs[i].position = new Vector3(0, i * orbHeight / numBands, 0);

            orbs[i].UpdateAudioData(bands[i] * 100);
            avgAmp += bands[i]; // add to average
        }

        avgAmp /= numBands;
        avgAmp *= 10000;
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

            float x = Mathf.Cos(rads) * ringRadius;
            float z = Mathf.Sin(rads) * ringRadius;

            float y = sample * ringHeight;

            points[i] = new Vector3(x, y, z);
        }

        line.positionCount = points.Length;
        line.SetPositions(points);
    }
}
