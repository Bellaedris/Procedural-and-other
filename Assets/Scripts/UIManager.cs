using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    #region variables
    public CelestialBody selected;
    public bool simulate;
    public TMP_InputField planetName;
    public Slider posX; 
    public Slider posZ; 
    public Slider mass;
    public Slider velocityX;
    public Slider velocityZ;
    public Toggle staticStar;

    private FlexibleColorPicker cp;
    private CameraController camera;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cp = FindObjectOfType<FlexibleColorPicker>();
        camera = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected && cp) {
            UpdateColor();
        }
    }

    public void UpdateX(float newX) {
        selected.transform.position = new Vector3(newX, 0, selected.transform.position.z);
    }

    public void UpdateZ(float newZ) {
        selected.transform.position = new Vector3(selected.transform.position.x, 0, newZ);
    }

    public void UpdateStaticStar(bool val) {
        selected.staticStar = val;
    }

    public void toggleSimulation() {
        simulate = !simulate;
        CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();
        foreach(CelestialBody body in bodies) {
            LineRenderer lr = body.GetComponent<LineRenderer>();
            lr.enabled = false;
        }
    }

    public void UpdateColor() {
        selected.starColor = cp.color;
    }

    public void UpdateName() {
        selected.name = planetName.text;
    }

    public void UpdateBaseXVelocity(float val) {
        selected.initialVelocity = new Vector3(val, 0, selected.initialVelocity.z);
    }

    public void UpdateBaseZVelocity(float val) {
        selected.initialVelocity = new Vector3(selected.initialVelocity.x, 0, val);
    }

    public void UpdateMass(float val) {
        selected.mass = val;
    }

    public void updateCameraFollow(bool follow) {
        if (follow) {
            camera.following = selected;
        } else {
            camera.following = null;
        }
    }
}
