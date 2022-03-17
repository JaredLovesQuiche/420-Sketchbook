using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCube : MonoBehaviour
{
    public Transform wall;
    BoxCollider box;
    public bool isSolid = true;

    private void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    private void OnMouseDown()
    {
        isSolid = !isSolid;
        UpdateArt();
    }

    void UpdateArt()
    {
        if (wall)
        {
            wall.gameObject.SetActive(isSolid);

            float y = isSolid ? 0.44f : 0.0f;
            float h = isSolid ? 1.1f : 0.2f;
        }
    }
}
