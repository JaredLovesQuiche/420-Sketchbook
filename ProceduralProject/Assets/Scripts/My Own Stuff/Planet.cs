using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public GameObject[] rocks;
    public GameObject seed;
    [SerializeField]
    GameObject[] faceObjects;

    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColorGenerator colorGenerator = new ColorGenerator();

    private void Start()
    {
        GenerateRandomValues(shapeSettings, colorSettings);
        GeneratePlanet();
    }

    public void ResetPlanet()
    {
        DestroyPlanet();
        GenerateRandomValues(shapeSettings, colorSettings);
        GeneratePlanet();
    }

    private void DestroyPlanet()
    {
        if (transform.childCount <= 6) return;
        GameObject[] children = new GameObject[transform.childCount];

        for (int i = 6; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }
        foreach (GameObject child in children)
        {
            Destroy(child);
        }
    }

    private void GenerateRandomValues(ShapeSettings shapeSettings, ColorSettings colorSettings)
    {
        shapeSettings.planetRadius = UnityEngine.Random.Range(40, 100);

        shapeSettings.noiseLayers = new ShapeSettings.NoiseLayer[2];

        shapeSettings.noiseLayers[0] = new ShapeSettings.NoiseLayer();
        shapeSettings.noiseLayers[0].enabled = true;
        shapeSettings.noiseLayers[0].useFirstLayerAsMask = false;
        shapeSettings.noiseLayers[0].noiseSettings.strength = UnityEngine.Random.Range(-0.2f, 0.4f);
        shapeSettings.noiseLayers[0].noiseSettings.numLayers = 4;
        shapeSettings.noiseLayers[0].noiseSettings.baseRoughness = UnityEngine.Random.Range(0.5f, 0.8f);
        shapeSettings.noiseLayers[0].noiseSettings.roughness = UnityEngine.Random.Range(1.0f, 3.0f);
        shapeSettings.noiseLayers[0].noiseSettings.persistence = UnityEngine.Random.Range(0.3f, 0.7f);
        shapeSettings.noiseLayers[0].noiseSettings.center = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
        shapeSettings.noiseLayers[0].noiseSettings.minValue = UnityEngine.Random.Range(0.0f, 1.0f);

        shapeSettings.noiseLayers[1] = new ShapeSettings.NoiseLayer();
        shapeSettings.noiseLayers[1].enabled = true;
        shapeSettings.noiseLayers[1].useFirstLayerAsMask = true;
        shapeSettings.noiseLayers[1].noiseSettings.strength = UnityEngine.Random.Range(-2.0f, 6.0f);
        shapeSettings.noiseLayers[1].noiseSettings.numLayers = 4;
        shapeSettings.noiseLayers[1].noiseSettings.baseRoughness = UnityEngine.Random.Range(0.5f, 4.0f);
        shapeSettings.noiseLayers[1].noiseSettings.roughness = UnityEngine.Random.Range(0.5f, 2.0f);
        shapeSettings.noiseLayers[1].noiseSettings.persistence = UnityEngine.Random.Range(0.3f, 0.7f);
        shapeSettings.noiseLayers[1].noiseSettings.center = new Vector3(UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100), UnityEngine.Random.Range(-100, 100));
        shapeSettings.noiseLayers[1].noiseSettings.minValue = UnityEngine.Random.Range(1.4f, 2.0f);

        GradientColorKey[] keys = new GradientColorKey[]
        {
            new GradientColorKey(new Color(UnityEngine.Random.Range(0.0f, 0.5f), UnityEngine.Random.Range(0.0f, 0.5f), UnityEngine.Random.Range(0.8f, 1.0f)), 0),
            new GradientColorKey(new Color(UnityEngine.Random.Range(0.5f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)), 3.5f),
            new GradientColorKey(new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.5f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)), UnityEngine.Random.Range(11.0f, 28.0f)),
            new GradientColorKey(new Color(UnityEngine.Random.Range(0.5f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.5f, 1.0f)), UnityEngine.Random.Range(37.0f, 54.0f)),
            new GradientColorKey(new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.5f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)), UnityEngine.Random.Range(62.0f, 82.0f)),
            new GradientColorKey(new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.5f, 1.0f)), 90.0f),
        };
        GradientAlphaKey[] aKeys = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1.0f, 0.0f),
            new GradientAlphaKey(1.0f, 100.0f),
        };

        colorSettings.gradient.SetKeys(keys, aKeys);
    }

    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);

        if (meshFilters == null || meshFilters.Length == 0)
            meshFilters = new MeshFilter[6];
        if (faceObjects == null || faceObjects.Length == 0)
            faceObjects = new GameObject[6];
        terrainFaces = new TerrainFace[6];
        

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (faceObjects[i] == null)
            {
                faceObjects[i] = new GameObject("mesh");
                faceObjects[i].transform.parent = transform;

                faceObjects[i].AddComponent<MeshRenderer>();
                faceObjects[i].AddComponent<MeshCollider>();
                meshFilters[i] = faceObjects[i].AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }

    void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            faceObjects[i].GetComponent<MeshCollider>().sharedMesh = terrainFaces[i].ConstructMesh();

            // generate interesting things
            foreach (Vector3 vert in faceObjects[i].GetComponent<MeshCollider>().sharedMesh.vertices)
            {
                if (UnityEngine.Random.Range(1, 100) > 98 && Application.isPlaying)
                {
                    int num = UnityEngine.Random.Range(0, 10);
                    GameObject rock = Instantiate(rocks[num], transform);
                    Vector3 planetNorm = vert.normalized;
                    rock.transform.position = vert - planetNorm * 0.5f;
                    rock.transform.LookAt(planetNorm + vert);
                    rock.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(UnityEngine.Random.Range(0.2f, 0.7f), UnityEngine.Random.Range(0.2f, 0.7f), UnityEngine.Random.Range(0.2f, 0.7f));
                    //rock.transform.Rotate(180, 0, 0);
                }
                else if (UnityEngine.Random.Range(1, 200) > 198 && Application.isPlaying)
                {
                    GameObject childSeed = Instantiate(seed, transform);
                    PlantDemo1 plant = childSeed.GetComponent<PlantDemo1>();
                    plant.iterations = 14;
                    plant.Build(childSeed.transform.position);
                    Vector3 planetNorm = vert.normalized;
                    childSeed.transform.position = vert - planetNorm * 0.5f;
                    childSeed.transform.LookAt(planetNorm);
                    childSeed.transform.Rotate(-90, 0, 0);
                }
            }
        }

        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    void GenerateColors()
    {
        colorGenerator.UpdateColors();
    }
}