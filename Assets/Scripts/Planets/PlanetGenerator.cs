using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{

    #region variables
    public int density = 10;
    public float planetRadius;

    public bool autoUpdate;

    private List<Vector3> points;
    #endregion

    #region customMethods

    // function found at https://stackoverflow.com/questions/9600801/evenly-distributing-n-points-on-a-sphere
    public void GenerateSphere() {
        this.points = PlanetMeshGenerator.TestGenerateSphere(density, planetRadius);
    }

    public void Projection() {
        this.points = PlanetMeshGenerator.TestProjection(density, planetRadius);
    }

    #endregion

    #region builtins
    // Start is called before the first frame update
    void Start()
    {
        Gizmos.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnDrawGizmos() {
        //draw the points
        if (points.Count <= 0 || !autoUpdate) return;
        foreach(Vector3 point in points) {
            Debug.Log(point);
            Gizmos.DrawSphere(point, .1f);
        }
    }
    #endregion
}
