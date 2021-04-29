using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO tiling -> générer plusieurs tuiles de décor
//TODO mettre en cache des données déjà calculées pour ne pas avoir à les recalculer (par exemple mettre en cache des chunks)

public class MapGenerator : MonoBehaviour
{

    #region variables
    [Header("Generator parameters")]
    public int MapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
    public int pointsPerUnit = 1;
    public float scale = 1.0f;
    public Vector2 offset;
    public int octaves = 1;
    [Range(0,1)]
    public float persistance = 1f;
    public float lacunarity = 1f;
    public float redistribution = 1f;
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
    public bool flattenWaterLevel;
    public AnimationCurve flatWaterlevel;
    public bool addDynamicWater;
    public Material waterMaterial;
    public Renderer waterRenderer;
    [Space(10)]
    [Header("Misc")]
    public bool colorMap;
    public bool generateMesh;
    public bool autoUpdate;
    public bool demo;
    public Renderer renderObject;
    public Mesh defaultMesh;
    #endregion

    public void GenerateMap() {
        float[,] noisemap = NoiseGenerator.GenerateNoise(MapChunkSize * pointsPerUnit, MapChunkSize * pointsPerUnit, octaves, persistance, lacunarity, scale, offset, redistribution, seed, 
                                                        islandMode, waterCoefficient);

        Texture2D texture;
        if (colorMap) {
            texture = TextureGenerator.GenerateColorTexture(noisemap, MapChunkSize * pointsPerUnit, MapChunkSize * pointsPerUnit, biomes);
        } else {
            texture = TextureGenerator.GenerateTexture(noisemap, MapChunkSize * pointsPerUnit, MapChunkSize * pointsPerUnit);
        }

        if (generateMesh) {
            MeshFilter mesh = renderObject.GetComponent<MeshFilter>();
            if (flattenWaterLevel)
                mesh.sharedMesh = MeshGenerator.GenerateMesh(noisemap, maxHeight, MapChunkSize, MapChunkSize, pointsPerUnit, flatWaterlevel, levelOfDetail);
            else
                mesh.sharedMesh = MeshGenerator.GenerateMesh(noisemap, maxHeight, MapChunkSize, MapChunkSize, pointsPerUnit, AnimationCurve.Linear(0,0,1,1), levelOfDetail);
            MeshFilter waterMesh = waterRenderer.GetComponent<MeshFilter>();
            waterMesh.sharedMesh = MeshGenerator.generateWater(MapChunkSize, MapChunkSize);

            waterRenderer.transform.position = renderObject.transform.position + (Vector3.up * (maxHeight * biomes[0].heightThreshold));
        } else {
            MeshFilter mesh = renderObject.GetComponent<MeshFilter>();
            mesh.sharedMesh = defaultMesh;
        }

        if (addDynamicWater) {
            waterRenderer.gameObject.SetActive(true);
        } else {
            waterRenderer.gameObject.SetActive(false);
        }

        renderObject.sharedMaterial.mainTexture = texture;
    }

    //checks for incorrect inspector values
    private void OnValidate() {
        scale = scale < 0 ? 0 : scale;

        octaves = octaves < 1 ? 1 : octaves;

        lacunarity = lacunarity < 1 ? 1 : lacunarity;

        redistribution = redistribution < 1 ? 1 : redistribution;

        if (addDynamicWater) {
            flattenWaterLevel = false;
        }

        if (flattenWaterLevel) {
            addDynamicWater = false;
        }
    }

    private void Update() {
        /*if (demo) {
            offset.x += .1f * Time.deltaTime;
            offset.y += -0.05f * Time.deltaTime;
            GenerateMap();
        }*/
    }

    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float heightThreshold;
        public Color color;
    }
}
