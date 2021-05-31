using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

//TODO move the planets by moving the mouse
//TODO refine the initial velocity so it's more talking
//TODO allow to zoom/dezoom/scale planets

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
    public GameObject planetEdit;

    private CameraController camera;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        camera = FindObjectOfType<CameraController>();
    }

    //TODO setters pour les valeurs de celestial body (regrouper tout ce qui touche au celestial body dans le meme script)
    public void UpdatePosition() {
        Debug.Log("value");
        selected.SetPosition(posX.value, posZ.value);
    }

    public void UpdateStaticStar(bool val) {
        selected.SetStaticStar(val);
    }

    public void UpdateColor() {
        selected.SetColor(cp.color);
        colorPreview.GetComponent<Image>().color = cp.color;
    }

    public void UpdateName() {
        selected.SetName(planetName.text);
    }

    public void UpdateBaseVelocity() {
        selected.SetInitialVelocity(velocityX.value, velocityZ.value);
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

    //TODO déplacer dans le script celectial simulation?
    public void toggleSimulation() {
        simulate = !simulate;
        CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();
        foreach(CelestialBody body in bodies) {
            LineRenderer lr = body.GetComponent<LineRenderer>();
            TrailRenderer trail = body.GetComponent<TrailRenderer>();
            lr.enabled = false;
            trail.enabled = simulate? true : false;
            body.transform.position = body.initialPosition;
            body.InitializeVelocity();
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
        planetEdit.SetActive(true);
        planetName.text = selected.name;
        staticStar.isOn = selected.staticStar;
        mass.value = selected.mass;
        Debug.Log(selected.transform.position.z);
        posX.value = selected.transform.position.x;
        posZ.value = selected.transform.position.z;
        velocityX.value = selected.initialVelocity.x;
        velocityZ.value = selected.initialVelocity.z;
        colorPreview.GetComponent<Image>().color = selected.starColor;
        if (cp) cp.gameObject.SetActive(false);
        Debug.Log( posZ.value);
    }

    public void HideWindow() {
        if (cp) cp.gameObject.SetActive(false);
        planetEdit.SetActive(false);
    }

    public void DeleteSelected() {
        Destroy(selected.gameObject);
        selected = null;
        if (cp) cp.gameObject.SetActive(false);
        planetEdit.SetActive(false);
    }

    public void Exit() {
        Application.Quit();
    }

    //validates that the input string only contains numbers
    public void ValidateNumber() {
        
    }
}
