using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class MeshGenerator
{
    public static Mesh GenerateMesh(float[,] noisemap, int minHeight, int maxHeight) {
        Mesh mesh = new Mesh();
       
        List<Vector3> vertices = new List<Vector3>();
        //calculate vertices
        for (int y = 0; y < noisemap.GetLength(0); y++)
        {
            //only write the point if he is not at the end of a line (as it would not have a point to its right)
            for (int x = 0; x < noisemap.GetLength(1); x++)
            {
                vertices.Add(new Vector3(x, noisemap[x, y] * maxHeight, y));
            }
        }

        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / noisemap.GetLength(1), vertices[i].z / noisemap.GetLength(0));
        }

        List<int> triangles = new List<int>();
        //TODO triangles aren't at correct coordinates, they are onto each other which is weird af
        //skips the first line as it has to points above 
        for (int y = 1; y < noisemap.GetLength(0); y++)
        {
            //only write the point if he is not at the end of a line (as it would not have a point to its right)
            for (int x = 0; x < noisemap.GetLength(1) - 1; x++)
            {
                int current = y * noisemap.GetLength(0) + x; // the "number" of the current point in a "1D" fashion
                //write current, diagonal, above and current, right, diagonal
                int above = current - noisemap.GetLength(1); //the point above the current
                int rDiagonal = above + 1; //the point in the right diagonal of the current
                int right = current + 1;//the point to the right of the current

                triangles.Add(current);
                triangles.Add(rDiagonal);
                triangles.Add(above);

                triangles.Add(current);
                triangles.Add(right);
                triangles.Add(rDiagonal);
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }

}
