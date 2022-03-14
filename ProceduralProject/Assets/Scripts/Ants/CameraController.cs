using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera camera;

    Vector2 storedMousePos;

    private bool firstClick = false;

    private void Start()
    {
        storedMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
    }

    void Update()
    {
        Vector2 currentPos = Input.mousePosition;

        if (Input.GetMouseButtonDown(1)) firstClick = true;
        else firstClick = false;

        //Debug.Log(camera.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetMouseButton(1) && !firstClick)
        {   
            Vector2 worldOffset = camera.ScreenToWorldPoint(currentPos) - camera.ScreenToWorldPoint(storedMousePos);

            camera.transform.position -= new Vector3(worldOffset.x, worldOffset.y, 0);
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            camera.orthographicSize -= Input.mouseScrollDelta.y;
            if (camera.orthographicSize > 20) camera.orthographicSize = 20;
            else if (camera.orthographicSize < 1) camera.orthographicSize = 1;
        }
        
        storedMousePos = currentPos;
    }
}
