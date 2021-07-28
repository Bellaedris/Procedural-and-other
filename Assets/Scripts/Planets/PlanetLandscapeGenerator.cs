using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetLandscapeGenerator
{
    public static Mesh generateNoise(Mesh meshData, float maxHeight, int seed, float scale,float lacunarity, float persistance, int octaves, float warpAmp) {
        FastNoiseLite gen = new FastNoiseLite(seed);

        gen.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        gen.SetFractalType(FastNoiseLite.FractalType.FBm);
        gen.SetRotationType3D(FastNoiseLite.RotationType3D.ImproveXYPlanes);
        gen.SetFractalLacunarity(lacunarity);
        gen.SetFractalOctaves(octaves);
        gen.SetFractalGain(persistance);
        gen.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
        gen.SetDomainWarpAmp(warpAmp);

        //seed to have the possibility to recreate a noisemap
        System.Random randomGenerator = new System.Random(seed);
        float offsetX = randomGenerator.Next(-1000, 1000);
        float offsetY = randomGenerator.Next(-1000, 1000);
        float offsetZ = randomGenerator.Next(-1000, 1000);
        Vector3 octaveOffsets = new Vector3(offsetX, offsetY, offsetZ);

        Vector3[] vertices = meshData.vertices;

        for(int i = 0; i < meshData.vertices.Length; i++) {
            float x = meshData.vertices[i].x * scale + offsetX;
            float y = meshData.vertices[i].y * scale + offsetY;
            float z = meshData.vertices[i].z * scale + offsetZ;

            float noiseValue = gen.GetNoise(x, y, z);

            /*warpingtests not concluent as of now
            Vector3 noiseValue = new Vector3 (
                gen.GetNoise(x, y, z),
                gen.GetNoise(x, y + 5.2f * maxHeight, z + 1.3f * maxHeight),
                gen.GetNoise(x, y + 3.4f * maxHeight, z + 0.6f * maxHeight)
            );
            float warpedValue = gen.GetNoise(x + 4 * noiseValue.x, y + 4 * noiseValue.y, z + 4 * noiseValue.z);
            vertices[i] += meshData.normals[i] * warpedValue * maxHeight;*/
            //Debug.Log(noiseValue);
            vertices[i] += meshData.normals[i] * noiseValue * maxHeight;
        }

        meshData.vertices = vertices;
        meshData.RecalculateNormals();

        return meshData;
    }
}
