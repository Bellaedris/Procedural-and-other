using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
    public static Texture2D GenerateTexture(float[] noiseMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[texture.width * texture.height];
        
        for(int i = 0; i < noiseMap.Length; i++) {
            //Debug.Log(noiseMap[i]);
            pixels[i] = Color.Lerp(Color.black, Color.white, noiseMap[i]); 
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }
}