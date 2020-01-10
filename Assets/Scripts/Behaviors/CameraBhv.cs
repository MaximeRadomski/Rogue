using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBhv : MonoBehaviour
{
    public Camera Camera;
    public float sceneWidth = 4;

    private Vector3 _beforeFocusPosition;

    void Start()
    {
        //if (Screen.currentResolution.)
        float unitsPerPixel = sceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        if (desiredHalfHeight > 3.50f)
            Camera.orthographicSize = desiredHalfHeight;
        _beforeFocusPosition = transform.position;
    }

    public void FocusY(float y)
    {
        _beforeFocusPosition = transform.position;
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public void Unfocus()
    {
        transform.position = _beforeFocusPosition;
    }
}
