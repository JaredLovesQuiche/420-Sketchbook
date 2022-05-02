using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicVis : MonoBehaviour
{
    public GameObject[] ticks;

    public Material inactive;
    public Material active;
    public Material activeTop;

    public float upperBound = 3.3f;
    public float pump = 0.0f;

    public static float beat = 0.0f;

    float max = 0;
    float min = 10000;

    private void Update()
    {
        float[] spectrum = new float[256];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        beat = spectrum[1];
        if (beat > pump) pump = beat;

        //if (beat > max) max = beat;
        //if (beat < min) min = beat;
        //print(max);
        //print(min);
        UpdateVisual(pump);
        pump -= 0.008f;
        if (pump < 0) pump = 0;
    }

    private void UpdateVisual(float v)
    {
        float ratio = v / upperBound;
        //print(ratio);
        for (int i = 0; i < ticks.Length; i++)
        {
            if (i / (ticks.Length - 1.0f) < ratio)
            {
                if (i == ticks.Length - 1)
                    ticks[i].GetComponent<MeshRenderer>().sharedMaterial = activeTop;
                else
                    ticks[i].GetComponent<MeshRenderer>().sharedMaterial = active;
            }
            else
                ticks[i].GetComponent<MeshRenderer>().sharedMaterial = inactive;
        }
    }
}
