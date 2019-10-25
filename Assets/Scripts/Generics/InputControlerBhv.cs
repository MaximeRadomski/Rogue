using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControlerBhv : MonoBehaviour
{
    private Vector3 _touchPosWorld;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
                RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
                if (hitInformation.collider != null)
                {
                    GameObject touchedObject = hitInformation.transform.gameObject;
                    if (touchedObject.tag == "Button")
                    {
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                            touchedObject.GetComponent<ButtonBhv>().BeginAction();
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                            touchedObject.GetComponent<ButtonBhv>().EndAction();
                        else
                            touchedObject.GetComponent<ButtonBhv>().DoAction();
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
            if (hitInformation.collider != null)
            {
                GameObject touchedObject = hitInformation.transform.gameObject;
                if (touchedObject.tag == "Button")
                {
                    touchedObject.GetComponent<ButtonBhv>().BeginAction();
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
            if (hitInformation.collider != null)
            {
                GameObject touchedObject = hitInformation.transform.gameObject;
                if (touchedObject.tag == "Button")
                {
                    touchedObject.GetComponent<ButtonBhv>().EndAction();
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
            if (hitInformation.collider != null)
            {
                GameObject touchedObject = hitInformation.transform.gameObject;
                if (touchedObject.tag == "Button")
                {
                    touchedObject.GetComponent<ButtonBhv>().DoAction();
                }
            }
        }
        else
            _touchPosWorld = new Vector3(-99,-99,-99);
    }
}
