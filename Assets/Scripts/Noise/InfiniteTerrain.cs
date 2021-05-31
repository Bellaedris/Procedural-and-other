using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class InfiniteTerrain : MonoBehaviour
{

    #region variables
    public Transform viewer;
    public Material defaultMat;
    public const float viewDistance = 300f;

    public static Vector2 viewerPos2D;

    private int chunkSize;
    private int visibleChunksCount;
    private static MapGenerator mapGen;

    Dictionary<Vector2, Chunk> chunks = new Dictionary<Vector2, Chunk>();
    List<Chunk> chunksActiveLastFrame = new List<Chunk>();
    #endregion

    #region builtins
    // Start is called before the first frame update
    void Start()
    {
        mapGen = FindObjectOfType<MapGenerator>();
        chunkSize = MapGenerator.MapChunkSize - 1;
        visibleChunksCount = Mathf.RoundToInt(viewDistance / chunkSize);

    }

    // Update is called once per frame
    void Update()
    {
        viewerPos2D = new Vector2(viewer.transform.position.x, viewer.transform.position.z);
        UpdateVisibleChunks();
    }

    #endregion

    void UpdateVisibleChunks() {
        foreach(Chunk chunk in chunksActiveLastFrame) {
            chunk.updateActive(false);
        }
        chunksActiveLastFrame.Clear();

        int currentChunkX = Mathf.RoundToInt(viewerPos2D.x / chunkSize);
        int currentChunkY = Mathf.RoundToInt(viewerPos2D.y / chunkSize);

        for(int yOffset = -visibleChunksCount; yOffset <= visibleChunksCount; yOffset++) {
            for(int xOffset = -visibleChunksCount; xOffset <= visibleChunksCount; xOffset++) {
                Vector2 currentChunkCoord = new Vector2(currentChunkX + xOffset, currentChunkY + yOffset);
                //create chunk and add it to the dictionnary
                if (chunks.ContainsKey(currentChunkCoord)) {
                    chunks[currentChunkCoord].updateChunk();
                    if (chunks[currentChunkCoord].isActive()) {
                        chunksActiveLastFrame.Add(chunks[currentChunkCoord]);
                    }
                } else {
                    chunks.Add(currentChunkCoord, new Chunk(currentChunkCoord, chunkSize, defaultMat));
                }
            }
        }
    }

    struct Chunk {
        Vector2 position;
        GameObject chunkMesh;
        Bounds chunkBounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public Chunk(Vector2 coord, float size, Material defaultMat) {
            this.position = coord * size;
            chunkBounds = new Bounds(position, Vector2.one * size);

            //create the chunk mesh
            chunkMesh = new GameObject("Terrain Chunk");
            chunkMesh.transform.position = new Vector3(position.x, 0, position.y);

            meshRenderer = chunkMesh.AddComponent<MeshRenderer>();
            meshFilter = chunkMesh.AddComponent<MeshFilter>();

            meshRenderer.sharedMaterial = defaultMat;

            updateActive(false);

            mapGen.RequestMapData(OnMapDataReceived);
        }

        public void updateChunk() {
            float distanceFromViewer = Mathf.Sqrt(chunkBounds.SqrDistance(viewerPos2D));
            updateActive(distanceFromViewer <= viewDistance);
        }

        public void updateActive(bool state) {
            chunkMesh.SetActive(state);
        }

        public bool isActive() {
            return chunkMesh.activeSelf;
        }

        public void OnMapDataReceived(MapGenerator.MapData mapData) {
            meshRenderer.sharedMaterial.mainTexture = TextureGenerator.TextureFromColorMap(
                mapData.colormap, MapGenerator.MapChunkSize, MapGenerator.MapChunkSize
            );
            mapGen.RequestMeshData(OnMeshDataReceived, mapData);
        }

        public void OnMeshDataReceived(MeshData meshData) {
            meshFilter.sharedMesh = meshData.CreateMesh();
        }

    }

}
