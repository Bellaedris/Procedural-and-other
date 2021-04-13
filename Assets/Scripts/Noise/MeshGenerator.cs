using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class MeshGenerator
{
    public static Mesh GenerateMesh(float[,] noisemap, float minHeight, float maxHeight, int width, int height, float pointsPerUnit) {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        int nx = noisemap.GetLength(1);
        int ny = noisemap.GetLength(0);
       
        Vector3[] vertices = new Vector3[nx * ny];
        //calculate vertices
        for (int y = 0; y < ny; y++)
        {
            //only write the point if he is not at the end of a line (as it would not have a point to its right)
            for (int x = 0; x < nx; x++)
            {
                vertices[y * nx + x] = new Vector3((float) x / pointsPerUnit, noisemap[x, y] * maxHeight, (float) y / pointsPerUnit);
            }
        }

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / width, vertices[i].z / height);
        }

        List<int> triangles = new List<int>();
        //skips the first line as it has to points above 
        for (int y = 1; y < ny; y++)
        {
            //only write the point if he is not at the end of a line (as it would not have a point to its right)
            for (int x = 0; x < nx - 1; x++)
            {
                int current = y * nx + x; // the "number" of the current point in a "1D" fashion
                //write current, diagonal, above and current, right, diagonal
                int above = current - ny; //the point above the current
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

        mesh.SetVertices(vertices);
        mesh.triangles = triangles.ToArray();
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        return mesh;
    }

    public static Mesh generateWater(int width, int height) {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        //calculate vertices
        for (int y = 0; y < height; y++)
        {
            //only write the point if he is not at the end of a line (as it would not have a point to its right)
            for (int x = 0; x < width; x++)
            {
                vertices.Add(new Vector3(x, 0, y));
            }
        }

        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / width, vertices[i].z / height);
        }

        List<int> triangles = new List<int>();
        //skips the first line as it has to points above 
        for (int y = 1; y < height; y++)
        {
            //only write the point if he is not at the end of a line (as it would not have a point to its right)
            for (int x = 0; x < width - 1; x++)
            {
                int current = y * height + x; // the "number" of the current point in a "1D" fashion
                //write current, diagonal, above and current, right, diagonal
                int above = current - width; //the point above the current
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

        mesh.SetVertices(vertices);
        mesh.triangles = triangles.ToArray();
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        return mesh;
    }

}
