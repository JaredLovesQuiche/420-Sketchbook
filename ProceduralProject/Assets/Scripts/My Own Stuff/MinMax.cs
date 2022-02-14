using UnityEngine;

public class MinMax
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public MinMax()
    {
        Min = float.MaxValue;
        Max = float.MinValue;
    }

    public void AddValue(float val)
    {
        if (val > Max)
            Max = val;
        if (val < Min)
            Min = val;
    }
}
