using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    #region variables
    public float mass = 1f;
    public Vector3 initialVelocity;
    public bool staticStar;
    public Color starColor = Color.gray;
    public string name = "Astre";
    public Vector3 initialPosition;

    private Vector3 velocity;
    private static List<CelestialBody> bodies;
    private Rigidbody rb;
    private UIManager uiManager;
    private TrailRenderer trail;
    #endregion

    #region setters
    public void SetColor(Color color) {
        this.starColor = color;
    }

    public void SetMass(float mass) {
        this.mass = mass;
    }

    public void SetName(string name) {
        this.name = name;
    }

    public void SetStaticStar(bool isStatic) {
        this.staticStar = isStatic;
    }

    public void SetPosition(float xCoord, float zCoord) {
        transform.position = new Vector3(xCoord, 0, zCoord);
        this.initialPosition = new Vector3(xCoord, 0, zCoord);
    }

    public void SetInitialVelocity(float xVelocity, float zVelocity) {
        this.initialVelocity = new Vector3(xVelocity, 0, zVelocity);
    }
    #endregion

    #region builtins

    private void Awake() {
        MeshRenderer rend = GetComponent<MeshRenderer>();
        rend.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        updateColor();

        velocity = initialVelocity;
        rb = GetComponent<Rigidbody>();
        uiManager = FindObjectOfType<UIManager>();
        trail = GetComponent<TrailRenderer>();

        initialPosition = transform.position;
        trail.sharedMaterial =  GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void FixedUpdate() {
        updateColor();
        if (!uiManager.simulate) {
            return;
        }
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

    private void OnDestroy() {
        bodies.Remove(this);
    }

    private void OnMouseUp() {
        uiManager.SetSelected(this);
    }

    #endregion

    //update the acceleration of the current body based on the forces exerted by the other bodies
    private void UpdateAceleration(CelestialBody other) {
        Vector3 direction = other.gameObject.transform.position - transform.position;
        float sqrDst = direction.sqrMagnitude;

        float force = UniverseRules.GRAV_CONST * (other.mass / sqrDst);
        Vector3 acceleration = direction.normalized * force;

        velocity +=  acceleration * UniverseRules.timeStep;
    }

    //updates the position of the body
    private void UpdatePosition() {
        rb.MovePosition(rb.position + velocity * UniverseRules.timeStep);
    }

    public void updateColor() {
        GetComponent<MeshRenderer>().sharedMaterial.SetColor("_BaseColor", starColor);
    }

    public void InitializeVelocity() {
        velocity = initialVelocity;
    }
}
