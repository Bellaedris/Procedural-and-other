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

    private FlexibleColorPicker cp;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cp = FindObjectOfType<FlexibleColorPicker>();
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

    public void UpdateStaticStar() {
        selected.staticStar = !selected.staticStar;
    }

    public void toggleSimulation() {
        simulate = !simulate;
    }

    public void UpdateColor() {
        selected.starColor = cp.color;
    }

    public void UpdateName() {
        selected.name = planetName.text;
    }
}
