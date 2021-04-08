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
    public GameObject colorPreview;
    public GameObject celestialBodyPrefab;
    public FlexibleColorPicker cp;

    private CameraController camera;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        camera = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateX(float newX) {
        selected.transform.position = new Vector3(newX, 0, selected.transform.position.z);
        selected.initialPosition = selected.transform.position;
    }

    public void UpdateZ(float newZ) {
        selected.transform.position = new Vector3(selected.transform.position.x, 0, newZ);
        selected.initialPosition = selected.transform.position;
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
            body.transform.position = body.initialPosition;
        }
    }

    public void UpdateColor() {
        selected.starColor = cp.color;
        colorPreview.GetComponent<Image>().color = cp.color;
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

    public void NewStar() {
        Vector3 spawnPos = celestialBodyPrefab.transform.position;
        while (Physics.OverlapSphere(spawnPos, 1f).Length > 0) {
            spawnPos.x += 1;
        }
        Instantiate(celestialBodyPrefab, spawnPos, celestialBodyPrefab.transform.rotation);
    }

    public void SetSelected(CelestialBody selected) {
        this.selected = selected;
        planetName.text = selected.name;
        staticStar.isOn = selected.staticStar;
        mass.value = selected.mass;
        posX.value = selected.transform.position.x;
        posZ.value = selected.transform.position.z;
        velocityX.value = selected.initialVelocity.x;
        velocityZ.value = selected.initialVelocity.z;
        colorPreview.GetComponent<Image>().color = selected.starColor;
        if (cp) cp.gameObject.SetActive(false);
    }
}
