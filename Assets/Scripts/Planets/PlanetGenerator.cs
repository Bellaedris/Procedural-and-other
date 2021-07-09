using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{

    #region variables
    public int subdivisions = 2;
    public float planetRadius;
    public GameObject renderObject;

    public bool autoUpdate;
    #endregion

    #region customMethods

    public void GenerateSphere() {
        SphereMeshData meshData = PlanetMeshGenerator.GenerateSphere(subdivisions, planetRadius);

        MeshFilter mesh = renderObject.GetComponent<MeshFilter>();
        mesh.sharedMesh = meshData.CreateMesh();
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

    private void OnValidate() {
        if (subdivisions > 6 || subdivisions < 0) {
            subdivisions = 0;
        }
    }
    
    #endregion
}
