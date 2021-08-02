using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{

    #region variables
    public int subdivisions = 2;
    public float planetRadius;
    [Space(10)]
    [Header("Noise Parameters")]
    public float maxHeight;
    public int seed;
    public float scale = 1.0f;
    public int octaves = 1;
    [Range(0,1)]
    public float persistance = 1f;
    public float lacunarity = 1f;
    public float warpAmplitude = 0f;
    [Space(10)]
    [Header("Craters Parameters")]
    [Tooltip("number of craters")]
    public int craterDensity = 0;
    [Tooltip("width of the outer rim of the crater")]
    public float rimWidth = .5f;
    [Tooltip("level of the flat floor of the crater")]
    public float rimHeight = -.8f;
    [Tooltip("steepness of the crater/rim junction")]
    public float rimSteepness = .2f;
    [Tooltip("maximal radius of the crater")]
    public float maxRadius = 1f;

    public GameObject renderObject;

    public bool autoUpdate;
    #endregion

    #region customMethods

    public void GenerateSphere() {
        System.Diagnostics.Stopwatch time = new System.Diagnostics.Stopwatch ();
        time.Start ();
        SphereMeshData meshData = PlanetMeshGenerator.GenerateSphere(subdivisions, planetRadius);

        MeshFilter mesh = renderObject.GetComponent<MeshFilter>();
        Mesh planetMesh = meshData.CreateMesh();
        planetMesh = PlanetLandscapeGenerator.generateNoise(planetMesh, maxHeight, seed, scale, lacunarity, persistance, octaves, warpAmplitude);
        planetMesh = PlanetLandscapeGenerator.generateCraters(planetMesh, craterDensity, rimWidth, rimHeight, rimSteepness, maxRadius);

        mesh.sharedMesh = planetMesh;
        time.Stop ();
        print("carried out " + time.Elapsed.TotalSeconds + " second");
    }

    #endregion

    #region builtins
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate() {
        if (subdivisions > 6 || subdivisions < 0) {
            subdivisions = 0;
        }

        scale = scale < 0 ? 0 : scale;

        octaves = octaves < 1 ? 1 : octaves;

        lacunarity = lacunarity < 1 ? 1 : lacunarity;
    }
    
    #endregion
}
