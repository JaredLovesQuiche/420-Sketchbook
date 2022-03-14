using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colony : MonoBehaviour
{
    public Main main;

    public PheromoneData[,] pheromoneData;

    public int food = 0;
    public int health = 100;
    public int danger = 0;

    public int eggFoodCost = 3;
    public float defaultEggCooldown = 7.0f;
    private float eggCooldown = 3.0f;
    public float eggTimeTohatch = 10.0f;
    public float defaultPheromoneDecayTimer = 15.0f;
    private float pheromoneDecayTimer = 15.0f;

    public void Run()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }
        eggCooldown -= Time.deltaTime;
        pheromoneDecayTimer -= Time.deltaTime;
        if (pheromoneDecayTimer <= 0)
        {
            for (int x = 0; x <= pheromoneData.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= pheromoneData.GetUpperBound(1); y++)
                {
                    pheromoneData[x, y].toHome /= 2;
                    pheromoneData[x, y].toFood /= 2;
                    pheromoneData[x, y].toFight /= 2;
                }
            }
            pheromoneDecayTimer = defaultPheromoneDecayTimer;
        }
    }

    public bool ShouldLayEgg()
    {
        if (eggCooldown > 0) return false;
        bool shouldLayEgg = false;
        if (food >= eggFoodCost)
        {
            food -= eggFoodCost;
            shouldLayEgg = true;
        }
        eggCooldown = defaultEggCooldown;

        return shouldLayEgg;
    }

    public void ReceiveFood(int amount)
    {
        food += amount;
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public Color GetColor()
    {
        return GetComponent<SpriteRenderer>().color;
    }
}