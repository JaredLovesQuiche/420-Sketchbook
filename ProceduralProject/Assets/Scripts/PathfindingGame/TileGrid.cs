using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindTile
{
    public Color color;
    public float height;

    public enum Type
    {

    }
}

public class TileGrid : MonoBehaviour
{
    public Vector3 WorldOffset { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    private PathfindTile[,] grid;

    public TileGrid(int width, int height)
    {
        this.Width = width;
        this.Height = height;
        this.WorldOffset = new Vector3(0, 0, 0);
        Init();
    }

    public TileGrid(int width, int height, Vector3 worldOffset)
    {
        this.Width = width;
        this.Height = height;
        this.WorldOffset = worldOffset;
        Init();
    }

    private void Init()
    {
        Build();
    }

    private void Build()
    {

    }

    public void SetSize(int width, int height, bool erase = false)
    {
        this.Width = width;
        this.Height = height;
        Build();
    }

    public void SetWorldOffset(Vector3 worldOffset)
    {
        this.WorldOffset = worldOffset;
        transform.position = worldOffset;
    }
}
