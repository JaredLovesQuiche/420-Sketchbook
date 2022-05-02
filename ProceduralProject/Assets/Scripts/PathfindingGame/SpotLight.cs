using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLight : MonoBehaviour
{
    public Transform spotlightCylinder;
    public Transform focusPoint;
    public Vector3 center;
    public Light light;

    public Color[] colors;
    private int activeColor = 0;

    public float speed = 1;
    public float angle = 40;
    public float movementMulti = 1.0f;
    private float colorSwitchTimer = 0.7f;
    private float defaultColorSwitchTimer = 0.7f;

    private void Start()
    {
        speed = Random.Range(0.5f, 3.0f);
        angle = Random.Range(20.0f, 35.0f);
        movementMulti = Random.Range(1.0f, 4.0f);
        light.spotAngle = angle;
    }

    private void Update()
    {
        focusPoint.position = new Vector3(Mathf.Cos(Time.time * speed) * movementMulti, 0, Mathf.Sin(Time.time * speed) * movementMulti) + center;
        spotlightCylinder.LookAt(focusPoint, new Vector3(0, 0, 1));

        if (MusicVis.beat > 0.25f && colorSwitchTimer <= 0)
        {
            colorSwitchTimer = defaultColorSwitchTimer;
            SwitchColors();
        }

        colorSwitchTimer -= Time.deltaTime;
    }

    private void SwitchColors()
    {
        activeColor++;
        if (activeColor > colors.Length - 1) activeColor = 0;
        light.color = colors[activeColor];
    }
}
