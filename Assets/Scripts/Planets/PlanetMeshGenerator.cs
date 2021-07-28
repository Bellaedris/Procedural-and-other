using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GK;

// many thanks to https://peter-winslow.medium.com/creating-procedural-planets-in-unity-part-1-df83ecb12e91
// for the help on the whole sphere gen part
public class PlanetMeshGenerator
{

    private static List<Triangle> triangles;
    private static List<Vector3> vertices;

    public static SphereMeshData GenerateSphere(int subdivisions, float planetRadius) {
        triangles = new List<Triangle> ();
        vertices = new List<Vector3> ();
        generateBaseIcosohedron();
        Subdivide(subdivisions);
        return GenerateMesh(triangles, vertices, planetRadius);
    }

    //generates the mesh from the triangulation
    public static SphereMeshData GenerateMesh(List<Triangle> triangles, List<Vector3> vertices, float planetRadius) {
        int numTris = triangles.Count;
        int numVertices = vertices.Count;
        SphereMeshData meshData = new SphereMeshData(numVertices, numTris * 3);

        //apply the planet radius
        for(int i = 0; i < vertices.Count; i++) {
            vertices[i] *= planetRadius;
        }

        meshData.vertices = vertices.ToArray();

        for (int triangleIndex = 0; triangleIndex < numTris; triangleIndex++) {
            meshData.AddTriangle(
                triangles[triangleIndex].triangle[0], 
                triangles[triangleIndex].triangle[1], 
                triangles[triangleIndex].triangle[2]
            );
        }

        return meshData;
    }

    // subdivide the base icosahedron n times
    public static void Subdivide(int iterations) {
        Dictionary<int, int> midPointCache = new Dictionary<int, int>();
        for(int i = 0; i < iterations; i++) {
            List<Triangle> newTriangles = new List<Triangle>();
            foreach(Triangle triangle in triangles) {
                int a = triangle.triangle[0];
                int b = triangle.triangle[1];
                int c = triangle.triangle[2];

                int ab = GetMidPointIndex(midPointCache, a, b);
                int bc = GetMidPointIndex(midPointCache, b, c);
                int ca = GetMidPointIndex(midPointCache, c, a);

                newTriangles.Add(new Triangle(a, ab, ca));
                newTriangles.Add(new Triangle(b, bc, ab));
                newTriangles.Add(new Triangle(c, ca, bc));
                newTriangles.Add(new Triangle(ab, bc, ca));
            }
            
            triangles = newTriangles;
        }
    }

    // https://peter-winslow.medium.com/creating-procedural-planets-in-unity-part-1-df83ecb12e91
    // get the index of the point in the middle, return int if it already exists
    public static int GetMidPointIndex(Dictionary<int, int> cache, int a, int b) {
        int smallerIndex = Mathf.Min (a, b);
        int greaterIndex = Mathf.Max (a, b);
        int key = (smallerIndex << 16) + greaterIndex;

        int ret;
        if (cache.TryGetValue(key, out ret))
            return ret;

        //if the index has not been stored, compute the midpoint
        Vector3 midPoint = Vector3.Lerp(vertices[a], vertices[b], 0.5f).normalized;
        vertices.Add(midPoint);
        ret = vertices.Count - 1;

        cache.Add(key, ret);
        return ret;
    }

    // generate basic icosahedron
    // https://peter-winslow.medium.com/creating-procedural-planets-in-unity-part-1-df83ecb12e91
    public static void generateBaseIcosohedron() {

        // An icosahedron has 12 vertices, and
        // since it's completely symmetrical the
        // formula for calculating them is kind of
        // symmetrical too:
 
        float t = (1.0f + Mathf.Sqrt (5.0f)) / 2.0f;

        vertices.Add (new Vector3 (-1, t, 0).normalized);
        vertices.Add (new Vector3 (1, t, 0).normalized);
        vertices.Add (new Vector3 (-1, -t, 0).normalized);
        vertices.Add (new Vector3 (1, -t, 0).normalized);
        vertices.Add (new Vector3 (0, -1, t).normalized);
        vertices.Add (new Vector3 (0, 1, t).normalized);
        vertices.Add (new Vector3 (0, -1, -t).normalized);
        vertices.Add (new Vector3 (0, 1, -t).normalized);
        vertices.Add (new Vector3 (t, 0, -1).normalized);
        vertices.Add (new Vector3 (t, 0, 1).normalized);
        vertices.Add (new Vector3 (-t, 0, -1).normalized);
        vertices.Add (new Vector3 (-t, 0, 1).normalized);

        // And here's the formula for the 20 sides,
        // referencing the 12 vertices we just created.        
        triangles.Add (new Triangle(0, 11, 5));
        triangles.Add (new Triangle(0, 5, 1));
        triangles.Add (new Triangle(0, 1, 7));
        triangles.Add (new Triangle(0, 7, 10));
        triangles.Add (new Triangle(0, 10, 11));
        triangles.Add (new Triangle(1, 5, 9));
        triangles.Add (new Triangle(5, 11, 4));
        triangles.Add (new Triangle(11, 10, 2));
        triangles.Add (new Triangle(10, 7, 6));
        triangles.Add (new Triangle(7, 1, 8));
        triangles.Add (new Triangle(3, 9, 4));
        triangles.Add (new Triangle(3, 4, 2));
        triangles.Add (new Triangle(3, 2, 6));
        triangles.Add (new Triangle(3, 6, 8));
        triangles.Add (new Triangle(3, 8, 9));
        triangles.Add (new Triangle(4, 9, 5));
        triangles.Add (new Triangle(2, 4, 11));
        triangles.Add (new Triangle(6, 2, 10));
        triangles.Add (new Triangle(8, 6, 7));
        triangles.Add (new Triangle(9, 8, 1));
    }
}

public struct SphereMeshData {
    public Vector3[] vertices;
    public Vector2[] uvs;
    public int[] triangles;

    int triangleIndex;

    public SphereMeshData(int numVertices, int numTriangles) {
        vertices = new Vector3[numVertices];
        uvs = new Vector2[numVertices];
        triangles = new int[numTriangles];
        triangleIndex = 0;
    }

    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}

public struct Triangle {
    public List<int> triangle;

    public Triangle(int a, int b, int c) {
        triangle = new List<int>() {a, b, c};
    }
}