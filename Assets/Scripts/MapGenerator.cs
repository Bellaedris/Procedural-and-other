using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    #region variables
    public int width = 100;
    public int height = 100;
    public float scale = 1.0f;
    public int octaves = 1;
    [Range(0,1)]
    public float persistance = 1f;
    public float lacunarity = 1f;
    public float redistribution = 1f;
    public float warping1 = 0f;
    public float warping2 = 0f;
    public int seed = 1;
    [Tooltip("Proportion of water over land of the island")]
    [Range(0,1)]
    public float waterCoefficient;
    [Tooltip("Generate island-ish shapes")]
    public bool islandMode;
    public bool autoUpdate;
    public Vector2 offset;
    public Renderer renderObject;
    #endregion

    public void GenerateMap() {
        float[] noisemap = NoiseGenerator.GenerateNoise(width, height, octaves, persistance, lacunarity, scale, offset, redistribution, seed, 
                                                        islandMode, waterCoefficient, warping1, warping2);
        
        Texture2D texture = TextureGenerator.GenerateTexture(noisemap, width, height);

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
}
