using UnityEngine;

public class Settings : MonoBehaviour
{
    public static bool isPaused = true;

    public static Color colonyPlacementColor = Color.white;

    public static int generationSeed = 0;
    public static int generationWidth = 40;
    public static int generationHeight = 40;
    public static float generationZoom = 10.0f;
    
    public static void ToggleIsPaused()
    {
        isPaused = !isPaused;
    }

    public static void SetColonyColorR(string v)
    {
        float r = float.Parse(v);
        colonyPlacementColor.r = r / 255.0f;
    }

    public static void SetColonyColorG(string v)
    {
        float g = float.Parse(v);
        colonyPlacementColor.g = g / 255.0f;
    }

    public static void SetColonyColorB(string v)
    {
        float b = float.Parse(v);
        colonyPlacementColor.b = b / 255.0f;
    }

    public static void SetGenerationWidth(string v)
    {
        int w = int.Parse(v);
        generationWidth = w;
    }

    public static void SetGenerationHeight(string v)
    {
        int h = int.Parse(v);
        generationHeight = h;
    }

    public static void SetGenerationZoom(string v)
    {
        float z = float.Parse(v);
        generationZoom = z;
    }

    public static void SetGenerationSeed(string v)
    {
        int s = int.Parse(v);
        generationSeed = s;
    }
}