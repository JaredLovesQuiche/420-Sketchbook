using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCube : MonoBehaviour
{
    public Transform wall;
    public Transform slime;
    BoxCollider box;

    public TerrainType type = TerrainType.Open;
    public float moveCost
    {
        get
        {
            if (type == TerrainType.Open) return 1;
            if (type == TerrainType.Wall) return 9999;
            if (type == TerrainType.Slime) return 10;
            return 1;
        }
    }

    public enum TerrainType
    {
        Open,
        Wall,
        Slime,
    }

    private void Start()
    {
        box = GetComponent<BoxCollider>();
        UpdateArt();
    }

    private void OnMouseDown()
    {
        type += 1;
        if ((int)type > 2) type = TerrainType.Open;
        UpdateArt();
    }

    void UpdateArt()
    {
        print((int)type + " " + type.ToString());
        bool isShowingWall = (type == TerrainType.Wall);
        bool isShowingSlime = (type == TerrainType.Slime);

        float y, h;
        if (isShowingWall)
        {
            y = 0.46f;
            h = 1.1f;
        }
        else if (isShowingSlime)
        {
            y = 0.46f;
            h = 1.1f;
        }
        else
        {
            y = 0.0f;
            h = 0.2f;
        }

        box.size = new Vector3(1, h, 1);
        box.center = new Vector3(0, y, 0);

        if (wall) wall.gameObject.SetActive(isShowingWall);
        if (slime) slime.gameObject.SetActive(isShowingSlime);
    }
}
