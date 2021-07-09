using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GK;

public class PlanetMeshGeneratorFibonacci
{
    public static SphereMeshData GenerateSphere(float density, float planetRadius) {
        List<Vector3> points = GeneratePoints(density, planetRadius);
        //triangulate the projection of the points
        DelaunayCalculator delaunayCalculator = new DelaunayCalculator();
        DelaunayTriangulation triangulation = new DelaunayTriangulation();
        delaunayCalculator.CalculateTriangulation(StereographicProjection(points), ref triangulation);

        return GenerateMesh(triangulation, points, planetRadius);
    }

    public static List<Vector3> GeneratePoints(float density, float planetRadius) {
        List<Vector3> points = new List<Vector3>();

        float phi = Mathf.PI * (3 - Mathf.Sqrt(5)); //golden angle in radians
        
        for (int i = 0; i < density; i++) {
            float y = 1 - (i / (float) (density - 1)) * 2; //y from 1 to -1
            float radius = Mathf.Sqrt(1 - y * y); // radius at y

            float theta = phi * i; //golden angle increment

            float x = Mathf.Cos(theta) * radius;
            float z = Mathf.Sin(theta) * radius;

            points.Add(new Vector3(x, y, z) * planetRadius);
        }

        return points;
    }

    // projects the points from a sphere to a plane
    // https://en.wikipedia.org/wiki/Stereographic_projection
    // and return the new points
    public static List<Vector2> StereographicProjection(List<Vector3> points) {
        List<Vector2> projectionPoints = new List<Vector2>();
        for(int i = 0; i < points.Count; i++) {
            projectionPoints.Add(
                new Vector2(
                    points[i].x / (1 - points[i].z),
                    points[i].y / (1 - points[i].z)
                )
            );
        }

        return projectionPoints;
    }

    // projection in 3D space to check the results in the inspector
    public static List<Vector3> StereographicProjectionDebug(List<Vector3> points) {
        List<Vector3> projectionPoints = new List<Vector3>();
        for(int i = 0; i < points.Count; i++) {
            projectionPoints.Add(
                new Vector3(
                    points[i].x / (1 - points[i].z),
                    0,
                    points[i].y / (1 - points[i].z)
                )
            );
        }

        return projectionPoints;
    }

    public static List<Vector3> InverseStereographicProjection(List<Vector2> points, float planetRadius) {
        List<Vector3> realPoints = new List<Vector3>();
        foreach(Vector2 point in points) {
            realPoints.Add(
                new Vector3(
                    (point.x * 2) / (1 + point.x * point.x + point.y * point.y),
                    (point.y * 2) / (1 + point.x * point.x + point.y * point.y),
                    (-1 + point.x * point.x + + point.y * point.y) / (1 + point.x * point.x + point.y * point.y)
                ) * planetRadius
            );
        }

        return realPoints;
    }

    //generates the mesh from the triangulation
    public static SphereMeshData GenerateMesh(DelaunayTriangulation triangles, List<Vector3> points, float planetRadius) {
        int numTris = triangles.Triangles.Count;
        int numVertices = triangles.Vertices.Count;
        SphereMeshData meshData = new SphereMeshData(numVertices, numTris);

        Debug.Log(triangles.Vertices[0]);
        Debug.Log(points[0]);

        meshData.vertices = InverseStereographicProjection(triangles.Vertices, planetRadius).ToArray();

        for (int triangleIndex = 0; triangleIndex < numTris / 3; triangleIndex++) {
            meshData.AddTriangle(
                triangles.Triangles[triangleIndex * 3], 
                triangles.Triangles[triangleIndex * 3 + 1], 
                triangles.Triangles[triangleIndex * 3 + 2]
            );
        }

        return meshData;
    }

    public static List<Vector3> TestGenerateSpherePoints(float density, float planetRadius) {
        return GeneratePoints(density, planetRadius);
    }

    public static List<Vector3> TestProjection(float density, float planetRadius) {
        return StereographicProjectionDebug(GeneratePoints(density, planetRadius) );
    }
}