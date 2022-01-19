using UnityEngine;

public class Noise : MonoBehaviour
{
    static public float perlin3D(float x, float y, float z)
    {
        float val = 0;

        val += Mathf.PerlinNoise(x, y);
        val += Mathf.PerlinNoise(-z, x);
        val += Mathf.PerlinNoise(-y, z);

        return val / 3.0f;
    }

    static public float perlin3D(Vector3 pos)
    {
        return perlin3D(pos.x, pos.y, pos.z);
    }
}
