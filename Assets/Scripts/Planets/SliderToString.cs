using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderToString : MonoBehaviour
{
    public Slider sliderUI;
    private TextMeshProUGUI textSliderValue;

    public void ShowSliderValue () {
        textSliderValue = GetComponent<TextMeshProUGUI>();
        string sliderMessage = sliderUI.value + "";
        textSliderValue.SetText(sliderMessage);
    }
}
