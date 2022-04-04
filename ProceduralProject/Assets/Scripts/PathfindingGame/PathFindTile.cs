using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindTile
{
    public PathfindTile Parent;
    public Vector3 Position;
    public float Heuristic;
    public float Cost;
    public float Score;

    public PathfindTile(PathfindTile _Parent, Vector3 _Position, float _Heuristic, float _Cost, float _Score)
    {
        this.Parent = _Parent;
        this.Position = _Position;
        this.Heuristic = _Heuristic;
        this.Cost = _Cost;
        this.Score = _Score;
    }

    public PathfindTile(PathfindTile _Parent, Vector3 _Position)
    {
        this.Parent = _Parent;
        this.Position = _Position;
    }

    public PathfindTile() { }
}
