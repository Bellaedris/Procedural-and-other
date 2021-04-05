using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D GenerateColorTexture(float[] noiseMap, int width, int height, MapGenerator.TerrainType[] biomes) {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[texture.width * texture.height];
        
        for(int i = 0; i < noiseMap.Length; i++) {
            float curHeight = noiseMap[i];
            foreach(MapGenerator.TerrainType biome in biomes) {
                if (curHeight <= biome.heightThreshold) {
                    pixels[i] = biome.color; 
                    break;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        //texture.filterMode = FilterMode.Point; 
        //texture.wrapMode = TextureWrapMode.Clamp;

        return texture;
    }

    public static Texture2D GenerateTexture(float[] noiseMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[texture.width * texture.height];
        
        for(int i = 0; i < noiseMap.Length; i++) {
            pixels[i] = Color.Lerp(Color.black, Color.white, noiseMap[i]); 
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }
}