using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius = 1.0f;
    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask = false;
        public NoiseSettings noiseSettings = new NoiseSettings();
    }
}
