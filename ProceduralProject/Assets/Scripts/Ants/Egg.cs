using UnityEngine;

public class Egg : MonoBehaviour
{
    public Colony parentColony;

    private Rigidbody2D rb;

    public float timeToHatch = 10.0f;
    public Ant.AntType type;

    public void Init()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        GetComponent<SpriteRenderer>().color = parentColony.GetColor() - new Color(0.1f, 0.1f, 0.1f, 0.0f);
    }

    public void Run()
    {
        timeToHatch -= Time.deltaTime;
        if (parentColony != null)
        {
            Vector2 dirToColony = (parentColony.transform.position - transform.position).normalized;
            rb.AddForce(dirToColony * 0.002f);
        }
    }

    public bool ShouldHatch()
    {
        return timeToHatch <= 0;
    }
}
