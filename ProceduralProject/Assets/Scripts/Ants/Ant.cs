using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public Main main;
    public Colony parentColony;

    public GameObject battleTarget;
    public GameObject foodPellet;

    private Rigidbody2D rb;

    public Vector2 targetDir = Vector2.zero;
    public Vector2 actualDir = Vector2.zero;

    public float defaultUpdateTimer = 0.25f;
    private float updateTimer = 0.25f;
    public float defaultPheromoneTimer = 0.25f;
    private float pheromoneTimer = 0.25f;

    public float speed = 1.0f;
    public float health = 10.0f;

    public bool seenEnemy = false;
    public bool fleeing = false;

    public AntState state = AntState.NOFOOD;
    public AntType type = AntType.WORKER;

    public void Init()
    {
        if (type == AntType.SOLDIER)
        {
            transform.localScale *= 1.5f;
            state = AntState.LOOKINGFORFIGHT;
        }
        actualDir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        rb = GetComponent<Rigidbody2D>();
        GetComponent<SpriteRenderer>().color = parentColony.GetColor() - new Color(0.1f, 0.1f, 0.1f, 0.0f);
    }

    public enum AntState
    {
        NOFOOD,
        HASFOOD,
        FIGHTING,
        LOOKINGFORFIGHT,
    }

    public enum AntType
    {
        WORKER,
        SOLDIER,
    }

    public void Run()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }
        updateTimer -= Time.deltaTime;
        pheromoneTimer -= Time.deltaTime;

        UpdateDirection();

        if (state == AntState.FIGHTING)
        {
            if (battleTarget != null)
                targetDir = (battleTarget.transform.position - transform.position).normalized;
            else
                state = AntState.LOOKINGFORFIGHT;
        }
        else if (updateTimer <= 0)
        {
            UpdateTargetDirection();
            updateTimer = defaultUpdateTimer;
        }
        
        if (pheromoneTimer <= 0)
        {
            ReleasePheromones();
            pheromoneTimer = defaultPheromoneTimer;
        }
        UpdateVelocity();
    }

    public void DrawDebug()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3)actualDir, Color.green);
        Debug.DrawLine(transform.position, transform.position + (Vector3)targetDir, Color.red);
    }

    private void UpdateVelocity()
    {
        rb.velocity = actualDir * speed;
        transform.localRotation = Quaternion.Euler(0, 0, Angle(actualDir) * 180 / Mathf.PI + 90);
    }

    private void UpdateDirection()
    {
        actualDir = Vector2.Lerp(actualDir, targetDir, 0.02f).normalized;
    }

    private void UpdateTargetDirection()
    {
        Vector2Int roundedPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        PheromoneData pheromone = parentColony.pheromoneData[roundedPos.x, roundedPos.y];
        Vector2 newDir = Vector2.zero;

        //Debug.Log(state + " " + tile.toHome);

        if (fleeing)
        {
            if (pheromone.toHome != Vector2.zero)
                targetDir = pheromone.toHome;
        }
        else
            switch (state)
            {
                case AntState.NOFOOD:
                    if (pheromone.toFood.magnitude > 0.5f)
                        newDir = Vector2.Lerp(targetDir, pheromone.toFood.normalized, pheromone.toFood.magnitude);
                    break;
                case AntState.HASFOOD:
                    if (pheromone.toHome.magnitude > 0.5f)
                        newDir = Vector2.Lerp(targetDir, pheromone.toHome.normalized, pheromone.toHome.magnitude);
                    break;
                case AntState.LOOKINGFORFIGHT:
                    if (pheromone.toFight.magnitude > 0.2f)
                        newDir = Vector2.Lerp(targetDir, pheromone.toFight.normalized, pheromone.toFight.magnitude);
                    break;
            }

        if (newDir != Vector2.zero)
        {
            targetDir = newDir;
        }
        else
        {
            // if the tile doesn't have pheromones, wander
            targetDir = RotateAngle(actualDir, Random.Range(-10.0f, 10.0f));
        }
    }

    private void ReleasePheromones()
    {
        Vector2Int roundedPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        PheromoneData data = parentColony.pheromoneData[roundedPos.x, roundedPos.y];

        //Debug.Log("pos: " + data.worldPos + "\nToHome: " + data.toHome + "\nToFood: " + data.toFood);

        if (fleeing)
        {
            data.toFight += -actualDir * 0.2f;
            data.toFight.Normalize();
        }

        switch (state)
        {
            case AntState.NOFOOD:
                // leave a trail of pheromones to get home
                data.toHome += -actualDir * 0.2f;
                data.toHome.Normalize();
                break;
            case AntState.HASFOOD:
                data.toFood += -actualDir * 0.2f;
                data.toFood.Normalize();
                break;
            case AntState.FIGHTING:
                data.toFight += actualDir * 0.2f;
                data.toFight.Normalize();
                break;
        }
        parentColony.pheromoneData[roundedPos.x, roundedPos.y] = data;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Colony otherColony;
        Ant otherAnt;

        if (other.gameObject.name.Equals("Tilemap"))
        {
            Vector2 contactPoint = other.GetContact(0).point;
            Vector2 dirToContact = (contactPoint - (Vector2)transform.position).normalized;
            Vector2 tilePos = (Vector2)transform.position + dirToContact / 2;
            Vector3Int tilePosRounded = new Vector3Int(Mathf.RoundToInt(tilePos.x), Mathf.RoundToInt(tilePos.y), 0);
            if (TerrainManager.tilemap.GetTile(tilePosRounded).name.Equals("foodTile"))
            {
                if (state == AntState.NOFOOD)
                {
                    state = AntState.HASFOOD;
                    foodPellet.SetActive(true);
                    parentColony.pheromoneData[tilePosRounded.x, tilePosRounded.y].food--;
                    if (parentColony.pheromoneData[tilePosRounded.x, tilePosRounded.y].food == 0)
                    {
                        main.SetTileToEmpty((Vector2Int)tilePosRounded);
                        parentColony.pheromoneData[tilePosRounded.x, tilePosRounded.y].food = -1;
                    }
                }
            }

            Vector2 n = other.GetContact(0).normal;

            actualDir = Reflect(actualDir, n);
            targetDir = Reflect(targetDir, n);
        }
        else if (other.gameObject.TryGetComponent(out otherColony))
        {
            if (otherColony == parentColony)
            {
                if (fleeing)
                {
                    parentColony.danger++;
                    fleeing = false;
                }
                if (state == AntState.HASFOOD)
                {
                    state = AntState.NOFOOD;
                    foodPellet.SetActive(false);
                    parentColony.ReceiveFood(1);
                }
            }
            else if (type == AntType.SOLDIER)
            {
                otherColony.health--;
            }
        }
        else if (other.gameObject.TryGetComponent(out otherAnt))
        {
            if (type == AntType.SOLDIER && otherAnt.parentColony != parentColony)
            {
                otherAnt.health--;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (battleTarget != null) return;
        Ant otherAnt;
        Colony otherColony;
        if (other.gameObject.TryGetComponent(out otherAnt))
        {
            if (otherAnt.parentColony != parentColony)
            {
                if (type == AntType.WORKER)
                {
                    fleeing = true;
                }
                else if (type == AntType.SOLDIER)
                {
                    state = AntState.FIGHTING;
                    battleTarget = otherAnt.gameObject;
                }
            }
        }
        else if (other.gameObject.TryGetComponent(out otherColony))
        {
            //print("hit colony");
            if (otherColony != parentColony)
            {
                if (type == AntType.WORKER)
                {
                    fleeing = true;
                }
                else if (type == AntType.SOLDIER)
                {
                    state = AntState.FIGHTING;
                    battleTarget = otherColony.gameObject;
                }
            }
        }
    }

    private Vector2 RotateAngle(Vector2 v, float angle)
    {
        float ca = Mathf.Cos(angle);
        float sa = Mathf.Sin(angle);
        return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
    }

    private Vector2 Reflect(Vector2 v, Vector2 n)
    {
        return v - n * (Vector2.Dot(v, n) * 2.0f);
    }

    private float Angle(Vector2 v)
    {
        return -Mathf.Atan2(-v.y, v.x);
    }
}
