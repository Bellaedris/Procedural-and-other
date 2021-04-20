using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoise(int width, int height, int octaves, float persistance, float lacunarity, float scale, Vector2 offset, float redistribution, int seed, 
                                        bool islandMode, float waterCoefficient, float warping1, float warping2
    ) {
        float[,] results = new float[width, height]; 
        float begin = Time.realtimeSinceStartup;

        //seed to have the possibility to recreate a noisemap
        System.Random randomGenerator = new System.Random(seed);
        //random offset
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = randomGenerator.Next(-1000, 1000) + offset.x;
            float offsetY = randomGenerator.Next(-1000, 1000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        for (float y = 0; y < width; y++) {
            for (float x = 0; x < height; x++) {
                // Initial values
                float amplitude = 1;
                float frequency = 1;
                float noiseValue = 0;

                // the FBM algorithm to generate layers
                for (int i = 0; i < octaves; i++) {
                    float xCoord = x / width * scale * frequency + octaveOffsets[i].x;
                    float yCoord = y / height * scale * frequency + octaveOffsets[i].y;

                    //warping reusing code from https://www.iquilezles.org/www/articles/warp/warp.htm
                    //add 1st level of warping
                    Vector2 q = new Vector2(Mathf.PerlinNoise(xCoord, yCoord), 
                                            Mathf.PerlinNoise(xCoord + 5.2f, yCoord + 1.3f));

                    //second level of warping
                    Vector2 r = new Vector2(Mathf.PerlinNoise(xCoord + warping1 * q.x + 1.7f, yCoord + warping1 * q.y + 9.2f), 
                                            Mathf.PerlinNoise(xCoord + warping1 * q.x + 5.2f, yCoord + warping1 * q.y + 1.3f));

                    noiseValue += Mathf.PerlinNoise(xCoord + warping2 * r.x, yCoord + warping2 * r.y) * amplitude;

                    frequency *= lacunarity;
                    amplitude *= persistance;
                }

                //redistribution to allow creation of flat areas if need be
                float finalValue = Mathf.Pow(noiseValue, redistribution);
                //create an island shape by lowering noise value the further you are from the middle of the map
                if (islandMode) 
                    finalValue = finalValue - Vector2.Distance(new Vector2(x, y), new Vector2(width / 2, height / 2)) / (width * waterCoefficient);
                results[(int) x, (int) y] = finalValue;
            }
        }

        //NormalizeValues(results);
        Debug.Log("time to generate heightmap: " + (Time.realtimeSinceStartup - begin));
        return results;
    }

    //looks for the map an min value, then normalizes the values to 0 - 1
    //will modify the input heightmap
    private static float[,] NormalizeValues(float[,] heightmap) {
        float maxVal = 0;

        for(int y = 0; y < heightmap.GetLength(0); y++) {
            for(int x = 0; x < heightmap.GetLength(1); x++) {
                if (heightmap[x, y] > maxVal) maxVal = heightmap[x, y];
            }
        }
        
        //normalize using the new max value
        for(int y = 0; y < heightmap.GetLength(0); y++) {
            for(int x = 0; x < heightmap.GetLength(1); x++) {
                heightmap[x, y] = heightmap[x, y] / maxVal;
            }
        }

        return heightmap;
    }

}
