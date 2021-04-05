using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    #region variables
    public float mass;
    public Vector3 initialVelocity = new Vector3(0, 0, 0);
    public bool staticStar;
    public Color starColor;

    private Vector3 velocity;
    private static List<CelestialBody> bodies;
    private Rigidbody rb;
    #endregion

    private void Start() {
        velocity = initialVelocity;
        rb = GetComponent<Rigidbody>();

        GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", starColor);
    }

    private void FixedUpdate() {
        foreach(CelestialBody body in bodies) {
            if (body != this) {
                UpdateAceleration(body);
            }
        }
        if (!staticStar) UpdatePosition();
    }

    private void OnEnable() {
        if (bodies == null) bodies = new List<CelestialBody>();
        bodies.Add(this);
    }

    //update the acceleration of the current body based on the forces exerted by the other bodies
    private void UpdateAceleration(CelestialBody other) {
        Vector3 direction = (other.gameObject.transform.position - transform.position).normalized;
        float sqrDst = direction.sqrMagnitude;

        float force = UniverseRules.GRAV_CONST * ((mass * other.mass) / sqrDst);
        Vector3 acceleration = direction * force;

        velocity +=  acceleration * UniverseRules.timeStep;
    }

    //updates the position of the body
    private void UpdatePosition() {
        rb.MovePosition(rb.position + velocity * UniverseRules.timeStep);
    }

}
