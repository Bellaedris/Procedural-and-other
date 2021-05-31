using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

//TODO mettre en cache des données déjà calculées pour ne pas avoir à les recalculer (par exemple mettre en cache des chunks)

public class MapGenerator : MonoBehaviour
{

    #region variables
    [Header("Generator parameters")]
    public const int MapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
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

    private ConcurrentQueue<MapThreadInfo<MapData>> MapDataThreadInfoQueue = new ConcurrentQueue<MapThreadInfo<MapData>>();
    private ConcurrentQueue<MapThreadInfo<MeshData>> MeshDataThreadInfoQueue = new ConcurrentQueue<MapThreadInfo<MeshData>>();
    #endregion

    #region customMethods
    public void GenerateMeshInEditor() {
        MapData data = GenerateMap();

        if (generateMesh) {
            MeshFilter mesh = renderObject.GetComponent<MeshFilter>();
            MeshData meshData;
            if (flattenWaterLevel)
                meshData = MeshGenerator.GenerateMesh(data.noisemap, maxHeight, MapChunkSize, MapChunkSize, flatWaterlevel, levelOfDetail);
            else
                meshData = MeshGenerator.GenerateMesh(data.noisemap, maxHeight, MapChunkSize, MapChunkSize, AnimationCurve.Linear(0,0,1,1), levelOfDetail);
            MeshFilter waterMesh = waterRenderer.GetComponent<MeshFilter>();
            waterMesh.sharedMesh = MeshGenerator.generateWater(MapChunkSize, MapChunkSize);
            mesh.sharedMesh = meshData.CreateMesh();

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

        renderObject.sharedMaterial.mainTexture = TextureGenerator.TextureFromColorMap(data.colormap, MapChunkSize, MapChunkSize);
    }

    private MapData GenerateMap() {
        float[,] noisemap = NoiseGenerator.GenerateNoise(MapChunkSize, MapChunkSize, octaves, persistance, lacunarity, scale, offset, redistribution, seed, 
                                                        islandMode, waterCoefficient);

        Color[] colourmap = new Color[MapChunkSize * MapChunkSize];
            for(int y = 0; y < noisemap.GetLength(0); y++) {
                for(int x = 0; x < noisemap.GetLength(1); x++) {
                    float curHeight = noisemap[x, y];
                    foreach(MapGenerator.TerrainType biome in biomes) {
                        if (curHeight <= biome.heightThreshold) {
                            colourmap[y * MapChunkSize + x] = biome.color; 
                            break;
                        }
                    }
                }
        }
        
        return new MapData(noisemap, colourmap);
    }

    public void RequestMapData(Action<MapData> callback) {
        ThreadStart threadStart = delegate {
            MapDataThread(callback);
        };
        new Thread(threadStart).Start();
    }

    public void MapDataThread(Action<MapData> callback) {
        MapData data = GenerateMap();
        lock (MapDataThreadInfoQueue)
        {
            MapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, data));
        }
    }

    public void RequestMeshData(Action<MeshData> callback, MapData mapData) {
        ThreadStart threadStart = delegate {
            MeshDataThread(callback, mapData);
        };
        new Thread(threadStart).Start();
    }

    public void MeshDataThread(Action<MeshData> callback, MapData mapData) {
        MeshData data = MeshGenerator.GenerateMesh(mapData.noisemap, maxHeight, MapChunkSize, MapChunkSize, flatWaterlevel, levelOfDetail);
        lock (MapDataThreadInfoQueue)
        {
            MeshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, data));
        }
    }

    #endregion

    #region builtins

    private void Update() {
        if (MapDataThreadInfoQueue.Count > 0) {
            for(int i = 0; i < MapDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MapData> threadInfo;
                MapDataThreadInfoQueue.TryDequeue(out threadInfo);
                threadInfo.callback(threadInfo.param);
            }
        }
        if (MeshDataThreadInfoQueue.Count > 0) {
            for(int i = 0; i < MeshDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MeshData> threadInfo;
                MeshDataThreadInfoQueue.TryDequeue(out threadInfo);
                threadInfo.callback(threadInfo.param);
            }
        }
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

    #endregion

    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float heightThreshold;
        public Color color;
    }

    public struct MapData {
        public float[,] noisemap;
        public Color[] colormap;

        public MapData(float[,] noisemap, Color[] colormap) {
            this.noisemap = noisemap;
            this.colormap = colormap;
        }
    }

    public struct MapThreadInfo<T> {
        public Action<T> callback;
        public T param;

        public MapThreadInfo(Action<T> callback, T param) {
            this.callback = callback;
            this.param = param;
        }
    }
}
