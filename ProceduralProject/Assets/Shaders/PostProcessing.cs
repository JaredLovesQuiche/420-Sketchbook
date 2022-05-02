using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessing : MonoBehaviour
{
    private Shader shader;
    private Material mat;

    public Texture noiseTexture;

    private void Start()
    {
        mat = new Material(shader);

        mat.SetTexture("_NoiseTex", noiseTexture);
    }

    public float UpdateAmp(float amp)
    {
        return 0;
    }
}
