using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainManager : MonoBehaviour
{
    public static Tilemap tilemap;
    private SimplexNoise noise;

    public Tile emptyTile;
    public Tile wallTile;
    public Tile foodTile;

    private Vector2 noiseOffset = new Vector2(0, 0);

    private void Awake()
    {
        tilemap = FindObjectOfType<Tilemap>();
    }

    private void Start()
    {
        noise = new SimplexNoise();
        noiseOffset = new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000));
    }

    public void Build()
    {
        Clear();
        Main.ClearSimulation();
        if (noise == null)
            noise = new SimplexNoise();
        noiseOffset = new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000));
        Main.pheromoneDataTemplate = new PheromoneData[Settings.generationWidth, Settings.generationHeight];

        Vector3Int[] positions = new Vector3Int[Settings.generationWidth * Settings.generationHeight];
        TileBase[] tiles = new Tile[Settings.generationWidth * Settings.generationHeight];

        int index = 0;
        for (int y = 0; y < Settings.generationHeight; y++)
        {
            for (int x = 0; x < Settings.generationWidth; x++)
            {
                if (x == 0 || y == 0 || x == Settings.generationWidth - 1 || y == Settings.generationHeight - 1)
                {
                    positions[index] = new Vector3Int(x, y);
                    tiles[index] = wallTile;
                    index++;
                    continue;
                }
                // evaluate noise at position and do different things.
                float v = noise.Evaluate(new Vector3((x + noiseOffset.x) / Settings.generationZoom, (y + noiseOffset.y) / Settings.generationZoom, 0));
                
                if (v >= 0.5)
                {
                    positions[index] = new Vector3Int(x, y);
                    tiles[index] = wallTile;
                }
                else if ((v >= 0.3 && v < 0.5) || (v > -1.0f && v < -0.8f)) // 0.3 <= v < 0.5 OR -0.7 < v < -0.5
                {
                    positions[index] = new Vector3Int(x, y);
                    SendTileInfo(new Vector2Int(x, y), 10);
                    tiles[index] = foodTile;
                }
                else
                {
                    positions[index] = new Vector3Int(x, y);
                    SendTileInfo(new Vector2Int(x, y), -1);
                    tiles[index] = emptyTile;
                }

                index++;
            }
        }

        tilemap.SetTiles(positions, tiles);
        tilemap.RefreshAllTiles();
    }

    public void SendTileInfo(Vector2Int gridPos, int food)
    {
        Main.pheromoneDataTemplate[gridPos.x, gridPos.y] = new PheromoneData(food);
    }

    static void Clear()
    {
        tilemap.ClearAllTiles();
    }
}
