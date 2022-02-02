using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject room;

    public int res = 50;

    private int[,] dungeonData;
    private List<GameObject> rooms;

    private void Start()
    {
        dungeonData = new int[res, res];
        rooms = new List<GameObject>();

        DungeonReset();
    }

    private void OnValidate()
    {
        if (!EditorApplication.isPlaying) return;
        dungeonData = new int[res, res];
        DungeonReset();
    }

    private void DungeonReset()
    {
        dungeonData = new int[res, res];

        ClearRooms();
        Generate(2, 3);
        Generate(4, 4);
        Generate(4, 4);
        CreateRoomsFromData();
    }

    void Generate(int startRoomType, int endRoomType)
    {
        int halfW = dungeonData.GetLength(0) / 2;
        int halfH = dungeonData.GetLength(1) / 2;

        int x = Random.Range(0, dungeonData.GetLength(0));
        int z = Random.Range(0, dungeonData.GetLength(1));
        int tx = Random.Range(0, halfW);
        int tz = Random.Range(0, halfH);

        if (x < halfW) tx += halfW;
        if (z < halfH) tz += halfH;
        
        if (dungeonData[x, z] < 2)
            dungeonData[x, z] = startRoomType;
        if (dungeonData[tx, tz] < 2)
            dungeonData[tx, tz] = endRoomType;

        while (x != tx || z != tz)
        {
            int dir = Random.Range(0, 4);
            int dis = Random.Range(1, 4);

            if (Random.Range(0, 10) > 4) // new direction
            {
                int dx = tx - x;
                int dz = tz - z;

                if (Mathf.Abs(dx) < Mathf.Abs(dz)) // pick random direction
                {
                    dir = (dz < 0) ? 0 : 1;
                }
                else
                {
                    dir = (dx < 0) ? 3 : 2;
                }
            }

            for (int i = 0; i < dis; i++) // walk in a direction and add rooms to dungeon data
            {
                switch (dir)
                {
                    case 0:
                        z--; break;
                    case 1:
                        z++; break;
                    case 2:
                        x++; break;
                    case 3:
                        x--; break;
                }
                x = Mathf.Clamp(x, 0, res - 1);
                z = Mathf.Clamp(z, 0, res - 1);
                if (dungeonData[x, z] == 0)
                    dungeonData[x, z] = 1;
            }
        }
    }

    void PunchHoles()
    {
        for (int x = 0; x < dungeonData.GetLength(0); x++)
        {
            for (int z = 0; z < dungeonData.GetLength(1); z++)
            {
                int val = dungeonData[x, z];
                if (val != 1) continue;

                if (Random.Range(0, 12) < 3) continue;

                int[] neighbors = new int[8];
                neighbors[0] = dungeonData[x, z - 1];
                neighbors[1] = dungeonData[x + 1, z - 1];
                neighbors[2] = dungeonData[x + 1, z];
                neighbors[3] = dungeonData[x + 1, z + 1];
                neighbors[4] = dungeonData[x, z + 1];
                neighbors[5] = dungeonData[x - 1, z + 1];
                neighbors[6] = dungeonData[x - 1, z];
                neighbors[7] = dungeonData[x - 1, z - 1];

                int tally = 0;
                foreach (int i in neighbors)
                {
                    if (!System.Convert.ToBoolean(i)) tally++;
                }

                if (tally <= 1)
                    dungeonData[x, z] = 0;
            }
        }
    }

    void CreateRoomsFromData()
    {
        for (int x = 0; x < dungeonData.GetLength(0); x++)
        {
            for (int z = 0; z < dungeonData.GetLength(1); z++)
            {
                if (dungeonData[x, z] > 0)
                    rooms.Add(CreateRoom(x, z, dungeonData[x, z]));
            }
        }
    }

    GameObject CreateRoom(int x, int z, int col)
    {
        GameObject o = Instantiate(room, new Vector3(x, 0, z), Quaternion.identity, transform);
        switch (col)
        {
            case 1: // normal room
                o.GetComponent<Renderer>().material.color = Color.white;
                break;
            case 2: // starting room
                o.GetComponent<Renderer>().material.color = Color.green;
                break;
            case 3: // exit room
                o.GetComponent<Renderer>().material.color = Color.red;
                break;
            case 4: // cool room
                o.GetComponent<Renderer>().material.color = Color.yellow;
                break;
        }
        return o;
    }

    void ClearRooms()
    {
        if (rooms.Count > 0)
            for (int i = rooms.Count - 1; i >= 0; i--)
            {
                Destroy (rooms[i]);
            }
    }
}
