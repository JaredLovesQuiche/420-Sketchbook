using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform debugSphere;

    public List<PathfindTile> OpenList = new List<PathfindTile>();
    public List<PathfindTile> ClosedList = new List<PathfindTile>();
    public float tileSize = 1.0f;
    public float yOffset = 1.0f;

    public List<PathfindTile> FindNewPath(Vector3 S, Vector3 T) // S Is the start, T is the target
    {
        OpenList.Clear();
        ClosedList.Clear();
        //Debug.Log(S);
        Vector3 OriginPosition = RoundVector3(S);
        Vector3 TargetPosition = RoundVector3(T);
        
        int Iterations = 0;
        float OriginDistanceFromTarget = GetHeuristic(OriginPosition, TargetPosition);

        OpenList.Add(new PathfindTile(null, OriginPosition, OriginDistanceFromTarget, 0, OriginDistanceFromTarget)); //Add starting tile

        while (OpenList.Count > 0)
        {
            Iterations += 1;

            PathfindTile CurrentTile = FindMinimumScoreTileInList(OpenList);
            //Debug.Log($"Pos {CurrentTile.Position} H{CurrentTile.Heuristic} C{CurrentTile.Cost} S{CurrentTile.Score}");

            if (Iterations > 500)
            {
                Debug.LogError("Iterations exceeded 500. Returning null");
                return null;
            }

            if (CurrentTile.Position == TargetPosition || CurrentTile.Heuristic <= 1)
            {
                //Debug.Log("Found path at " + CurrentTile.Position);
                return CalculatePath(CurrentTile);
            }
                
            //If current tile is not the end, add to closed list
            OpenList.Remove(CurrentTile);
            ClosedList.Add(CurrentTile);

            foreach (PathfindTile NeighborTile in GetNeighborTiles(CurrentTile, tileSize))
            {
                Debug.DrawLine(new Vector3(NeighborTile.Position.x + tileSize, yOffset, NeighborTile.Position.z + tileSize), new Vector3(NeighborTile.Position.x - tileSize, yOffset, NeighborTile.Position.z - tileSize), Color.red, 0.5f);

                if (IsTileAlreadyInList(NeighborTile, ClosedList)) continue; //if already in closed list or contains a dead zone in the tilemap or there is an obstacle in the way

                //bool ray = Physics.Linecast(NeighborTile.Position, CurrentTile.Position, 1 << 0, QueryTriggerInteraction.Ignore);
                //if (ray)
                //{
                //    print("linecast hit something!");
                //}

                bool raycast = Physics.Raycast(NeighborTile.Position, (CurrentTile.Position - NeighborTile.Position).normalized, 10.0f, 1 << 0);
                if (raycast)
                {
                    continue;
                }

                NeighborTile.Heuristic = GetHeuristic(NeighborTile.Position, TargetPosition);
                NeighborTile.Cost = NeighborTile.Parent.Cost * tileSize + Vector3.Distance(CurrentTile.Position, NeighborTile.Position);
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
            //Instantiate(debugSphere).position = RefTile.Position;

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

    private List<PathfindTile> GetNeighborTiles(PathfindTile CurrentTile, float tileSize)
    {
        List<PathfindTile> NeighborList = new List<PathfindTile>
        {
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3( tileSize, 0,  0       )),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3(-tileSize, 0,  0       )),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3( 0       , 0,  tileSize)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3( 0       , 0, -tileSize)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3( tileSize, 0,  tileSize)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3( tileSize, 0, -tileSize)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3(-tileSize, 0,  tileSize)),
            new PathfindTile(CurrentTile, CurrentTile.Position + new Vector3(-tileSize, 0, -tileSize))
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
        float z;

        if (vector3.x > 0)
            x = Mathf.Max(vector3.x - vector3.x % tileSize, (vector3.x + tileSize / 2) - (vector3.x + tileSize / 2) % tileSize);
        else x = Mathf.Min(vector3.x - vector3.x % tileSize, (vector3.x - tileSize / 2) - (vector3.x - tileSize / 2) % tileSize);
        if (vector3.z > 0)
            z = Mathf.Max(vector3.z - vector3.z % tileSize, (vector3.z + tileSize / 2) - (vector3.z + tileSize / 2) % tileSize);
        else z = Mathf.Min(vector3.z - vector3.z % tileSize, (vector3.z - tileSize / 2) - (vector3.z - tileSize / 2) % tileSize);
        return new Vector3(x, yOffset, z);
    }
}