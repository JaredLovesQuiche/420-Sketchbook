using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public List<PathfindTile> OpenList = new List<PathfindTile>();
    public List<PathfindTile> ClosedList = new List<PathfindTile>();
    public float TileSize = 1.0f;

    public List<PathfindTile> FindNewPath(Vector3 S, Vector3 T) // S Is the start, T is the target
    {
        OpenList.Clear();
        ClosedList.Clear();
        //Debug.Log(S);
        Vector3 OriginPosition = RoundVector3(S);
        Vector3 TargetPosition = RoundVector3(T);

        RaycastHit testRay;
        Physics.Linecast(transform.position, OriginPosition, (LayerMask)13);
        Debug.DrawLine(OriginPosition + new Vector3(TileSize / 2, TileSize / 2, 0), OriginPosition - new Vector3(TileSize / 2, TileSize / 2, 0), Color.magenta, 0.5f);
        //Debug.Log(OriginPosition);
        if (testRay)
        {
            OriginPosition.x += TileSize;
            Debug.DrawLine(OriginPosition + new Vector3(TileSize / 2, TileSize / 2, 0), OriginPosition - new Vector3(TileSize / 2, TileSize / 2, 0), Color.magenta, 0.5f);
            if (testRay)
            {
                OriginPosition.y += TileSize;
                Debug.DrawLine(OriginPosition + new Vector3(TileSize / 2, TileSize / 2, 0), OriginPosition - new Vector3(TileSize / 2, TileSize / 2, 0), Color.magenta, 0.5f);
                if (testRay)
                {
                    OriginPosition.x -= TileSize;
                    Debug.DrawLine(OriginPosition + new Vector3(TileSize / 2, TileSize / 2, 0), OriginPosition - new Vector3(TileSize / 2, TileSize / 2, 0), Color.magenta, 0.5f);
                    if (testRay)
                    {
                        Debug.LogError("No suitable starting point for pathfind");
                        return null;
                    }
                }
            }
        }
        

        int Iterations = 0;
        float OriginDistanceFromTarget = GetHeuristic(OriginPosition, TargetPosition);

        OpenList.Add(new PathfindTile(null, OriginPosition, OriginDistanceFromTarget, 0, OriginDistanceFromTarget)); //Add starting tile

        while (OpenList.Count > 0)
        {
            Iterations += 1;

            PathfindTile CurrentTile = FindMinimumScoreTileInList(OpenList);
            //Debug.Log($"Pos {CurrentTile.Position} H{CurrentTile.Heuristic} C{CurrentTile.Cost} S{CurrentTile.Score}");

            if (Iterations > 50)
            {
                Debug.LogError("Iterations exceeded 50. Navigating to furthest tile.");
                return CalculatePath(CurrentTile);
            }

            if (CurrentTile.Position == TargetPosition || CurrentTile.Heuristic <= 1)
            {
                //Debug.Log("Found path at " + CurrentTile.Position);
                return CalculatePath(CurrentTile);
            }
                
            //If current tile is not the end, add to closed list
            OpenList.Remove(CurrentTile);
            ClosedList.Add(CurrentTile);

            foreach (PathfindTile NeighborTile in GetNeighborTiles(CurrentTile))
            {
                Debug.DrawLine(new Vector3(NeighborTile.Position.x + TileSize, NeighborTile.Position.y + TileSize), new Vector3(NeighborTile.Position.x - TileSize, NeighborTile.Position.y - TileSize), Color.red, 0.5f);

                if (IsTileAlreadyInList(NeighborTile, ClosedList)) continue; //if already in closed list or contains a dead zone in the tilemap or there is an obstacle in the way

                RaycastHit2D Ray = Physics2D.Linecast(Vector3ToVector2(NeighborTile.Position), Vector3ToVector2(CurrentTile.Position), (LayerMask)13, minDepth:-1.0f, maxDepth:1.0f);
                if (Ray.collider)
                    continue;

                NeighborTile.Heuristic = GetHeuristic(NeighborTile.Position, TargetPosition);
                NeighborTile.Cost = NeighborTile.Parent.Cost * TileSize + Vector2.Distance(CurrentTile.Position, NeighborTile.Position);
                NeighborTile.Score = NeighborTile.Heuristic + NeighborTile.Cost;

                if (!IsTileAlreadyInList(NeighborTile, OpenList))
                    OpenList.Add(NeighborTile);
            }
        }

        Debug.LogError("Out of members in OpenList");

        return null;
    }

    private List<PathfindTile> CalculatePath(PathfindTile EndTile)
    {
        List<PathfindTile> Path = new List<PathfindTile>();
        PathfindTile RefTile = EndTile;

        Path.Add(EndTile);

        while (RefTile.Parent != null)
        {
            Debug.DrawLine(RefTile.Position, RefTile.Parent.Position, Color.green, 0.5f);

            Path.Add(RefTile.Parent);
            RefTile = RefTile.Parent;
        }

        Path.Reverse();
        return Path;
    }

    //Useful Functions

    private bool IsTileAlreadyInList(PathfindTile Tile, List<PathfindTile> List)
    {
        foreach (PathfindTile TileFromList in List)
        {
            if (Tile.Position == TileFromList.Position)
                return true;
        }
        return false;
    }

    private PathfindTile FindMinimumScoreTileInList(List<PathfindTile> TileList)
    {
        float Benchmark = float.MaxValue;
        PathfindTile BestTile = new PathfindTile();

        foreach (PathfindTile Tile in TileList)
        {
            if (Tile.Score < Benchmark)
            {
                Benchmark = Tile.Score;
                BestTile = Tile;
            }
        }

        return BestTile;
    }

    private List<PathfindTile> GetNeighborTiles(PathfindTile CurrentTile)
    {
        List<PathfindTile> NeighborList = new List<PathfindTile>
        {
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3(0.5f, 0,  0)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3(-0.5f, 0, 0)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3(0, 0.5f,  0)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3(0, -0.5f, 0)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3(0.5f, 0.5f, 0)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3(0.5f, -0.5f, 0)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3(-0.5f, 0.5f,  0)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3(-0.5f, -0.5f, 0))
        };

        return NeighborList;
    }

    private float GetHeuristic(Vector3 FromPosition, Vector3 ToPosition)
    {
        return Vector3.Distance(FromPosition, ToPosition);
    }

    private Vector3 RoundVector3(Vector3 vector3)
    {
        float x;
        float y;

        if (vector3.x > 0)
            x = Mathf.Max(vector3.x - vector3.x % TileSize, (vector3.x + TileSize / 2) - (vector3.x + TileSize / 2) % TileSize);
        else x = Mathf.Min(vector3.x - vector3.x % TileSize, (vector3.x - TileSize / 2) - (vector3.x - TileSize / 2) % TileSize);
        if (vector3.y > 0)
            y = Mathf.Max(vector3.y - vector3.y % TileSize, (vector3.y + TileSize / 2) - (vector3.y + TileSize / 2) % TileSize);
        else y = Mathf.Min(vector3.y - vector3.y % TileSize, (vector3.y - TileSize / 2) - (vector3.y - TileSize / 2) % TileSize);
        return new Vector3(x, y, 0);
    }

    private Vector2 Vector3ToVector2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }
}