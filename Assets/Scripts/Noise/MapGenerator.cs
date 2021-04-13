using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    #region variables
    [Header("Generator parameters")]
    public int width = 100;
    public int height = 100;
    public int pointsPerUnit = 1;
    public float scale = 1.0f;
    public int octaves = 1;
    [Range(0,1)]
    public float persistance = 1f;
    public float lacunarity = 1f;
    public float redistribution = 1f;
    public float warping1 = 0f;
    public float warping2 = 0f;
    public float minHeight = 0;
    public float maxHeight = 1;
    public int seed = 1;
    [Tooltip("Proportion of water over land of the island")]
    [Range(0,1)]
    public float waterCoefficient;
    [Tooltip("Generate island-ish shapes")]
    public bool islandMode;
    [Space(10)]
    [Header("Biomes and colors")]
    public TerrainType[] biomes;
    [Space(10)]
    [Header("Water")]
    [Tooltip("Automatically fill the spaces below the lowest value with water")]
    public bool addWater;
    public Material waterMaterial;
    public Renderer waterRenderer;
    [Space(10)]
    [Header("Misc")]
    public bool colorMap;
    public bool generateMesh;
    public bool autoUpdate;
    public bool demo;
    public Vector2 offset;
    public Renderer renderObject;
    public Mesh defaultMesh;
    #endregion

    public void GenerateMap() {
        float[,] noisemap = NoiseGenerator.GenerateNoise(width * pointsPerUnit, height * pointsPerUnit, octaves, persistance, lacunarity, scale, offset, redistribution, seed, 
                                                        islandMode, waterCoefficient, warping1, warping2);

        Texture2D texture;
        if (colorMap) {
            texture = TextureGenerator.GenerateColorTexture(noisemap, width * pointsPerUnit, height * pointsPerUnit, biomes);
        } else {
            texture = TextureGenerator.GenerateTexture(noisemap, width * pointsPerUnit, height * pointsPerUnit);
        }

        if (generateMesh) {
            MeshFilter mesh = renderObject.GetComponent<MeshFilter>();
            mesh.sharedMesh = MeshGenerator.GenerateMesh(noisemap, minHeight, maxHeight, width, height, pointsPerUnit);
            MeshFilter waterMesh = waterRenderer.GetComponent<MeshFilter>();
            waterMesh.sharedMesh = MeshGenerator.generateWater(width, height);

            waterRenderer.transform.position = renderObject.transform.position + (Vector3.up * (maxHeight * biomes[0].heightThreshold));
        } else {
            MeshFilter mesh = renderObject.GetComponent<MeshFilter>();
            mesh.sharedMesh = defaultMesh;
        }

        if (addWater) {
            waterRenderer.gameObject.SetActive(true);
        } else {
            waterRenderer.gameObject.SetActive(false);
        }

        renderObject.sharedMaterial.mainTexture = texture;
    }

    //checks for incorrect inspector values
    private void OnValidate() {
        width = width < 1 ? 1 : width;

        height = height < 1 ? 1 : height;

        scale = scale < 1 ? 1 : scale;

        octaves = octaves < 1 ? 1 : octaves;

        lacunarity = lacunarity < 1 ? 1 : lacunarity;

        redistribution = redistribution < 1 ? 1 : redistribution;

        warping1 = warping1 < 0 ? 0 : warping1;

        warping2 = warping2 < 0 ? 0 : warping2;
    }

    private void Update() {
        if (demo) {
            offset.x += .1f * Time.deltaTime;
            offset.y += -0.05f * Time.deltaTime;
            GenerateMap();
        }
    }

    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float heightThreshold;
        public Color color;
    }
}
