using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBhv : MonoBehaviour
{
    public Camera Camera;
    public float sceneWidth = 4;

    void Start()
    {
        //if (Screen.currentResolution.)
        float unitsPerPixel = sceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        if (desiredHalfHeight > 3.50f)
            Camera.orthographicSize = desiredHalfHeight;
    }
}
