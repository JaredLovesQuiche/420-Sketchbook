using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct PheromoneData
{
    public PheromoneData(int food)
    {
        this.food = food;
        toHome = Vector2.zero;
        toFood = Vector2.zero;
        toFight = Vector2.zero;
    }

    public enum Type
    {
        TOHOME,
        TOFOOD,
        TOFIGHT,
    }

    public int food;
    public Vector2 toHome;
    public Vector2 toFood;
    public Vector2 toFight;
}

public class Main : MonoBehaviour
{
    public GameObject colonyObject;
    public GameObject eggObject;
    public GameObject antObject;

    public Tile emptyTile;
    public Tile wallTile;
    public Tile foodTile;

    public TerrainManager terrainManager;

    public static PheromoneData[,] pheromoneDataTemplate;
    public static List<Colony> colonies = new List<Colony>();
    public static List<Egg> eggs = new List<Egg>();
    public static List<Ant> ants = new List<Ant>();


    private void Start()
    {
        terrainManager.Build();
        //SpawnDebugTest();
    }

    private void Update()
    {
        if (Settings.isPaused) return;
        foreach (Colony c in colonies)
        {
            c.Run();
            if (c.ShouldLayEgg())
            {
                if (c.danger > 0)
                {
                    LayEgg(c, Ant.AntType.SOLDIER);
                    c.danger -= 2;
                }
                else
                    LayEgg(c, Ant.AntType.WORKER);
            }
        }

        foreach (Egg e in eggs)
        {
            if (e == null) continue;
            e.Run();
            if (e.ShouldHatch()) Hatch(e);
        }

        foreach (Ant a in ants)
        {
            if (a == null) continue;
            a.Run();
            a.DrawDebug();
        }
        //if (ants.Count > 0) print(ants[0].state);
    }

    public static void ClearSimulation()
    {
        for (int i = colonies.Count - 1; i >= 0; i--)
        {
            Destroy(colonies[i].gameObject);
        }
        for (int i = eggs.Count - 1; i >= 0; i--)
        {
            Destroy(eggs[i].gameObject);
        }
        for (int i = ants.Count - 1; i >= 0; i--)
        {
            Destroy(ants[i].gameObject);
        }

        colonies.Clear();
        eggs.Clear();
        ants.Clear();
    }

    public void SetTileToEmpty(Vector2Int gridPos)
    {
        TerrainManager.tilemap.SetTile((Vector3Int)gridPos, emptyTile);
        // update the global tilemap data
        terrainManager.SendTileInfo(gridPos, -1);
    }

    //public void SetTileToWall(Vector2Int gridPos)
    //{
    //    TerrainManager.tilemap.SetTile((Vector3Int)gridPos, foodTile);
    //    // update the global tilemap data
    //    terrainManager.SendTileInfo(gridPos);

    //    foreach (Colony c in colonies)
    //    {
    //        // send info to colony copies
    //        c.tileInfo[gridPos.x, gridPos.y].worldPos = gridPos;
    //    }
    //}

    public void LayEgg(Colony parent, Ant.AntType type)
    {
        GameObject e = Instantiate(eggObject);
        e.transform.position = parent.transform.position;
        e.GetComponent<Egg>().parentColony = parent;
        e.GetComponent<Egg>().timeToHatch = parent.eggTimeTohatch;
        e.GetComponent<Egg>().type = type;
        e.GetComponent<Egg>().Init();
        eggs.Add(e.GetComponent<Egg>());
    }

    public void Hatch(Egg egg)
    {
        GameObject ant = Instantiate(antObject);
        ant.transform.position = egg.transform.position;
        ant.GetComponent<Ant>().main = this;
        ant.GetComponent<Ant>().parentColony = egg.parentColony;
        ant.GetComponent<Ant>().type = egg.type;
        ant.GetComponent<Ant>().Init();
        ants.Add(ant.GetComponent<Ant>());
        Destroy(egg.gameObject);
    }

    public Colony SpawnColony(Vector2 position)
    {
        GameObject newColony = Instantiate(colonyObject);
        newColony.transform.position = position;
        newColony.GetComponent<Colony>().pheromoneData = pheromoneDataTemplate;
        newColony.GetComponent<Colony>().SetColor(Settings.colonyPlacementColor);
        colonies.Add(newColony.GetComponent<Colony>());
        return newColony.GetComponent<Colony>();
    }

    public void SpawnRandomColony()
    {
        Vector2 colonyPos = FindRandomPositionOnGrid();

        GameObject newColony = Instantiate(colonyObject);
        newColony.transform.position = colonyPos;
        newColony.GetComponent<Colony>().pheromoneData = pheromoneDataTemplate;
        newColony.GetComponent<Colony>().SetColor(Settings.colonyPlacementColor);
        colonies.Add(newColony.GetComponent<Colony>());
    }

    private Vector2Int FindRandomPositionOnGrid()
    {
        int maxLimit = 500;
        int counter = 0;
        while (true)
        {
            Vector2Int randPos = new Vector2Int(Random.Range(1, Settings.generationWidth - 1), Random.Range(1, Settings.generationHeight - 1));
            Tile tile = (Tile)TerrainManager.tilemap.GetTile((Vector3Int)randPos);

            if (tile != wallTile && tile != foodTile)
            {
                return randPos;
            }
            else if (counter > maxLimit) return new Vector2Int(0, 0);
            counter++;
        }
    }
}
