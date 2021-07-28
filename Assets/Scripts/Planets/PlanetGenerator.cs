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

    public GameObject renderObject;

    public bool autoUpdate;
    #endregion

    #region customMethods

    public void GenerateSphere() {
        SphereMeshData meshData = PlanetMeshGenerator.GenerateSphere(subdivisions, planetRadius);

        MeshFilter mesh = renderObject.GetComponent<MeshFilter>();
        Mesh planetMesh = meshData.CreateMesh();
        mesh.sharedMesh = PlanetLandscapeGenerator.generateNoise(planetMesh, maxHeight, seed, scale, lacunarity, persistance, octaves, warpAmplitude);
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
