using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[] GenerateNoise(int width, int height, int octaves, float persistance, float lacunarity, float scale, Vector2 offset, float redistribution, int seed, 
                                        bool islandMode, float waterCoefficient, float warpingX, float warpingY
    ) {
        float[] results = new float[width * height]; 
        float[] warpX = GenerateWarpNoise(width, height, seed);
        float[] warpY = GenerateWarpNoise(width, height, seed);

        //seed to have the possibility to recreate a noisemap
        System.Random randomGenerator = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = randomGenerator.Next(-100000, 100000) + offset.x;
            float offsetY = randomGenerator.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        for (float y = 0; y < width; y++) {
            for (float x = 0; x < height; x++) {
                // Initial values
                float amplitude = 1;
                float frequency = 1;
                float noiseValue = 0;
                float range = 0f;

                // the FBM algorithm which overlay perlin noise
                for (int i = 0; i < octaves; i++) {
                    float xCoord = x / width * scale * frequency + octaveOffsets[i].x;
                    float yCoord = y / height * scale * frequency + octaveOffsets[i].y;

                    //add first layer of domain warping
                    xCoord += warpX[(int)y * width + (int)x] * warpingX;
                    yCoord += warpY[(int)y * width + (int)x] * warpingY;
                    //add second layer of domain warping
                    float warpedX = Mathf.PerlinNoise(xCoord, yCoord) + warpX[(int)y * width + (int)x] * warpingX;
                    float warpedY = Mathf.PerlinNoise(xCoord, yCoord) + warpY[(int)y * width + (int)x] * warpingY;

                    noiseValue += Mathf.PerlinNoise(warpedX, warpedY) * amplitude;

                    frequency *= lacunarity;
                    amplitude *= persistance;
                    range += 1f / (Mathf.Pow(2, i));
                }

                float finalValue = Mathf.Pow(noiseValue / (range / 2f), redistribution);
                if (islandMode) 
                    finalValue = finalValue - Vector2.Distance(new Vector2(x, y), new Vector2(width / 2, height / 2)) / (width * waterCoefficient);
                results[(int)y * width + (int)x] = finalValue;
            }
        }

        return results;
    }

    private static float[] GenerateWarpNoise(int width, int height, int seed) {
        float[] results = new float[width * height]; 
        int octaves = 3;
        float lacunarity = 2f;
        float persistance = 0.4f;

        //seed to have the possibility to recreate a noisemap
        System.Random randomGenerator = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = randomGenerator.Next(-100000, 100000);
            float offsetY = randomGenerator.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        for (float y = 0; y < width; y++) {
            for (float x = 0; x < height; x++) {
                // Initial values
                float amplitude = 1;
                float frequency = 1;
                float noiseValue = 0;
                float range = 0f;

                // the FBM algorithm which overlay perlin noise
                for (int i = 0; i < octaves; i++) {
                    float xCoord = x / width * frequency + octaveOffsets[i].x;
                    float yCoord = y / height * frequency + octaveOffsets[i].y;
                    noiseValue += Mathf.PerlinNoise(xCoord, yCoord) * amplitude;

                    frequency *= lacunarity;
                    amplitude *= persistance;
                    range += 1f / (Mathf.Pow(2, i));
                }

                results[(int)y * width + (int)x] = noiseValue * 2 - 1;
            }
        }

        return results;
    }
}

