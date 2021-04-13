using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ColorPicker : MonoBehaviour, IPointerDownHandler
{

    #region variables
    public FlexibleColorPicker picker;

    private UIManager um;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        um = FindObjectOfType<UIManager>();
    }

    public void OnPointerDown (PointerEventData eventData) 
    {
        if (um.selected) {
            if (picker.gameObject.activeSelf) {
                um.UpdateColor();
            }
            picker.gameObject.SetActive(!picker.gameObject.activeSelf);
            picker.color = um.selected.starColor;
        }
    }
    
}
