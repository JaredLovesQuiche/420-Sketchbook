using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAgent : MonoBehaviour
{
    static float G = 5;
    static float MAX_FORCE = 20;

    static List<GravityAgent> agents = new List<GravityAgent>();

    static void FindGravityForce(GravityAgent a, GravityAgent b)
    {
        if (a == b) return;
        if (a.isDone) return;
        if (b.isDone) return;

        Vector3 vectorToB = b.position - a.position;
        float rr = vectorToB.sqrMagnitude;
        float gravity = G * (a.mass * b.mass) / rr;

        if (gravity > MAX_FORCE) gravity = MAX_FORCE;

        vectorToB.Normalize();

        a.AddForce(vectorToB * gravity);
        b.AddForce(-vectorToB * gravity);
    }

    Vector3 position;
    Vector3 force;
    Vector3 velocity;

    Material ballMat;
    Material trailMat;
    Color color;
    float rMax, rMin, gMax, gMin, bMax, bMin;

    float mass;
    float scale;
    bool isDone = false;
    bool collided = false;

    private void Start()
    {
        position = transform.position;
        mass = Random.Range(10, 100);
        scale = Mathf.Sqrt(Mathf.Sqrt(mass / 5));

        int colorBias = Random.Range(0, 3); // to try to reduce the occurence of more boring colors
        switch (colorBias)
        {
            case 0:
                rMax = Random.Range(0.7f, 1.0f);
                gMax = Random.Range(0.4f, 0.6f);
                bMax = Random.Range(0.4f, 0.6f);
                break;
            case 1:
                rMax = Random.Range(0.4f, 0.6f);
                gMax = Random.Range(0.7f, 1.0f);
                bMax = Random.Range(0.4f, 0.6f);
                break;
            case 2:
                rMax = Random.Range(0.4f, 0.6f);
                gMax = Random.Range(0.4f, 0.6f);
                bMax = Random.Range(0.7f, 1.0f);
                break;
        }
        
        rMin = Random.Range(0.4f, rMax);
        gMin = Random.Range(0.4f, gMax);
        bMin = Random.Range(0.4f, bMax);
        ballMat = transform.GetComponent<MeshRenderer>().material;
        trailMat = transform.GetComponent<TrailRenderer>().material;

        agents.Add(this); 
    }

    private void OnDestroy()
    {
        agents.Remove(this);
    }

    private void Update()
    {
        UpdateColors();

        // calc gravity to other agents
        if (collided) return;
        foreach (GravityAgent a in agents)
        {
            FindGravityForce(this, a);
        }
        isDone = true;

        // physics integration step
        Vector3 acceleration = force / mass;

        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;

        transform.position = position;
    }

    private void LateUpdate()
    {
        isDone = false;
        force *= 0;
        transform.localScale = new Vector3(scale, scale, scale);
        foreach (GravityAgent a in agents)
            a.collided = false;
    }

    private void AddForce(Vector3 f)
    {
        force += f;
    }

    float lerpColor = 0.5f;
    int flip = 1;
    private void UpdateColors()
    {
        lerpColor += flip * Time.deltaTime / 1;
        if (lerpColor <= 0.5f) flip = 1;
        else if (lerpColor >= 1) flip = -1;

        color.r = Mathf.Lerp(rMin, rMax, lerpColor);
        color.g = Mathf.Lerp(gMin, gMax, lerpColor);
        color.b = Mathf.Lerp(bMin, bMax, lerpColor);

        ballMat.color = color;
        trailMat.color = color;
    }

    void OnTriggerEnter(Collider other)
    {
        GravityAgent otherSphere;
        if (!other.transform.TryGetComponent(out otherSphere)) return;
        if (otherSphere.collided) return;
        collided = true;
        otherSphere.collided = true;

        float m1 = mass;
        float m2 = otherSphere.mass;
        Vector3 dirToOther = (position - otherSphere.position).normalized;
        Vector3 dirToThis = (otherSphere.position - position).normalized;

        Vector3 v1 = velocity;
        float x1 = Vector3.Dot(dirToOther, v1);
        Vector3 v1x = dirToOther * x1;
        Vector3 v1y = v1 - v1x;

        Vector3 v2 = otherSphere.velocity;
        float x2 = Vector3.Dot(dirToThis, v2);
        Vector3 v2x = dirToThis * x2;
        Vector3 v2y = v2 - v2x;

        v1 = v1x * (m1 - m2) / (m1 + m2) + v2x * (2 * m2) / (m1 + m2) + v1y;
        v2 = v1x * (2 * m1) / (m1 + m2) + v2x * (m2 - m1) / (m1 + m2) + v2y;

        velocity = v1;
        otherSphere.velocity = v2;
    }
    // f = m / a
}
