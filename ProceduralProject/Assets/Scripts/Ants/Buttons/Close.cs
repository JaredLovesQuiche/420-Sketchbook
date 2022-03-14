using System.Collections;
using UnityEngine;

public class Close : MonoBehaviour
{
    public RectTransform panel;
    public Vector2 closePosition;
    public Vector2 openPosition;

    bool closed = false;

    public void SlideCanvas()
    {
        print(panel.transform.position);
        if (closed)
        {
            panel.transform.position = new Vector3(openPosition.x, openPosition.y, panel.transform.position.z);
        }
        else
            panel.transform.position = new Vector3(closePosition.x, closePosition.y, panel.transform.position.z);

        closed = !closed;
    }
}
