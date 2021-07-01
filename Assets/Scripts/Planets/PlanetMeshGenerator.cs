using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMeshGenerator
{
    public static void GenerateSphere(float density, float planetRadius) {
        List<Vector3> points = GeneratePoints(density, planetRadius);
        // triangulation on the projection
        // DelaunayTriangulation(StereographicProjection(points));
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
    public static List<Vector3> StereographicProjection(List<Vector3> points) {
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

    public static List<Vector3> TestGenerateSphere(float density, float planetRadius) {
        return GeneratePoints(density, planetRadius);
    }
    public static List<Vector3> TestProjection(float density, float planetRadius) {
        return StereographicProjection(GeneratePoints(density, planetRadius) );
    }
}
