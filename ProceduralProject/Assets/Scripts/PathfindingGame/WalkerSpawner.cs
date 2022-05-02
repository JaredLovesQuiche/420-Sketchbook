using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerSpawner : MonoBehaviour
{
    private float spawnTimer = 2.0f;

    public float defaultSpawnTimer = 1.0f;
    public int maxWalkers = 50;
    public static List<Walker> walkers = new List<Walker>();

    public Walker walkerPrefab;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        InvokeRepeating(nameof(CullWalkers), 0, 3.0f);
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnWalker();
            spawnTimer = defaultSpawnTimer;
        }
    }

    private void SpawnWalker()
    {
        if (walkers.Count < maxWalkers)
        {
            Walker newWalker = Instantiate(walkerPrefab, transform);
            walkers.Add(newWalker);
        }
    }

    private void CullWalkers()
    {
        for (int i = walkers.Count - 1; i >= 0; i--)
        {
            if (walkers[i] == null) continue;
            if (Vector3.Distance(walkers[i].transform.position, player.position) > 30)
            {
                Destroy(walkers[i].gameObject);
                walkers.RemoveAt(i);
            }
        }
    }
}
