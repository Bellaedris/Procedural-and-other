using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitDisplay : MonoBehaviour
{

    #region variables
    public int nbIter = 100;
    public bool thickLines;
    
    private UIManager manager;
    #endregion

    private void Start() {
        manager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!manager.simulate) {
            DrawOrbits();
        }
    }

    void DrawOrbits() {
        CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();
        VirtualBody[] vbodies = new VirtualBody[bodies.Length];
        Vector3[][] drawPoints = new Vector3[bodies.Length][];

        //init virtual bodies to have positions without touching the actual bodies
        for(int i = 0; i < vbodies.Length; i++) {
            vbodies[i] = new VirtualBody(bodies[i]);
            drawPoints[i] = new Vector3[nbIter];
        }

        //update the virtual positions for nbIter steps
        for (int i = 0; i < nbIter; i++) {
            //update velocity
            for(int j = 0; j < vbodies.Length; j++) {
                vbodies[j].velocity += CalculateAcceleration(j, vbodies) * UniverseRules.timeStep;
            }
            //update position
            for(int j = 0; j < vbodies.Length; j++) {
                if (!bodies[j].staticStar) vbodies[j].position += vbodies[j].velocity * UniverseRules.timeStep;

                //save the point to draw
                drawPoints[j][i] = vbodies[j].position;
            }
        }

        //draw the lines
        for(int i = 0; i < vbodies.Length; i++) {
            Color trajectoryColor = bodies[i].starColor;
            if (thickLines) {
                LineRenderer lr = bodies[i].GetComponent<LineRenderer>();
                lr.enabled = true;
                lr.positionCount = drawPoints[i].Length;
                lr.SetPositions(drawPoints[i]);
                lr.material = bodies[i].GetComponent<MeshRenderer>().sharedMaterial;
                lr.startColor = trajectoryColor;
                lr.endColor = trajectoryColor;
                lr.widthMultiplier = .1f;
            } else {
                LineRenderer lr = bodies[i].GetComponent<LineRenderer>();
                lr.enabled = false;
                for (int j = 0; j < drawPoints[i].Length - 1; j++) {
                    Debug.DrawLine(drawPoints[i][j], drawPoints[i][j + 1], trajectoryColor);
                }
            }
        }
    }

    private Vector3 CalculateAcceleration(int j, VirtualBody[] vbodies) {
        Vector3 acceleration = Vector3.zero;
        for(int i = 0; i < vbodies.Length; i++) {
            if (i == j) {
                continue;
            }
            Vector3 direction = (vbodies[i].position - vbodies[j].position).normalized;
            
            float sqrDst = direction.sqrMagnitude;
            float force = UniverseRules.GRAV_CONST * ((vbodies[j].mass * vbodies[i].mass) / sqrDst);

            acceleration += (direction * force);
        }
        
        return acceleration;
    }
}

public class VirtualBody {
    public float mass;
    public Vector3 position;
    public Vector3 velocity;

    public VirtualBody(CelestialBody body) {
        mass = body.mass;
        position = body.transform.position;
        velocity = body.initialVelocity;
    }
};