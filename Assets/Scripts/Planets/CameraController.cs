using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region variables
    public CelestialBody following;

    private Vector3 initialPosition;
    #endregion

    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (following) {
            transform.position = following.transform.position + new Vector3(0, 20, 0);
        } else {
            //TODO not call this on every frame, add it as an event on click on the button
            transform.position = initialPosition;
        }
    }
}
