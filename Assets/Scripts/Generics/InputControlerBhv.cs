using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControlerBhv : MonoBehaviour
{
    private Vector3 _touchPosWorld;
    private GameObject _currentObject;

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
                    CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
                    _currentObject = hitInformation.transform.gameObject;
                    if (_currentObject.tag == Constants.TagButton)
                    {
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                            _currentObject.GetComponent<ButtonBhv>().BeginAction();
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                        {
                            _currentObject.GetComponent<ButtonBhv>().EndAction();
                            _currentObject = null;
                        }
                        else
                            _currentObject.GetComponent<ButtonBhv>().DoAction();
                    }
                    else if (_currentObject.tag == Constants.TagGrabbableCard)
                    {
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                            _currentObject.GetComponent<GrabbableCardBhv>().BeginAction(touchPosWorld2D);
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                        {
                            _currentObject.GetComponent<GrabbableCardBhv>().EndAction();
                            _currentObject = null;
                        }
                        else
                            _currentObject.GetComponent<GrabbableCardBhv>().GrabAction(touchPosWorld2D);
                    }
                }
                else
                    CancelCurrentObjectIfNewBeforeEnd();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
            if (hitInformation.collider != null)
            {
                CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
                _currentObject = hitInformation.transform.gameObject;
                if (_currentObject.tag == Constants.TagButton)
                    _currentObject.GetComponent<ButtonBhv>().BeginAction();
                else if (_currentObject.tag == Constants.TagGrabbableCard)
                    _currentObject.GetComponent<GrabbableCardBhv>().BeginAction(touchPosWorld2D);
            }
            else
                CancelCurrentObjectIfNewBeforeEnd();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
            if (hitInformation.collider != null)
            {
                CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
                _currentObject = hitInformation.transform.gameObject;
                if (_currentObject.tag == Constants.TagButton)
                {
                    _currentObject.GetComponent<ButtonBhv>().EndAction();
                    _currentObject = null;
                }
                else if (_currentObject.tag == Constants.TagGrabbableCard)
                {
                    _currentObject.GetComponent<GrabbableCardBhv>().EndAction();
                    _currentObject = null;
                }
            }
            else
                CancelCurrentObjectIfNewBeforeEnd();
        }
        else if (Input.GetMouseButton(0))
        {
            _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
            RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
            if (hitInformation.collider != null)
            {
                CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
                _currentObject = hitInformation.transform.gameObject;
                if (_currentObject.tag == Constants.TagButton)
                    _currentObject.GetComponent<ButtonBhv>().DoAction();
                else if (_currentObject.tag == Constants.TagGrabbableCard)
                    _currentObject.GetComponent<GrabbableCardBhv>().GrabAction(touchPosWorld2D);
            }
            else
                CancelCurrentObjectIfNewBeforeEnd();
        }
        else
            _touchPosWorld = new Vector3(-99,-99,-99);
    }

    private void CancelCurrentObjectIfNewBeforeEnd(GameObject touchedGameObject = null)
    {
        if (_currentObject == null || _currentObject == touchedGameObject)
            return;
        if (_currentObject.tag == Constants.TagButton)
        {
            _currentObject.GetComponent<ButtonBhv>().CancelAction();
            _currentObject = null;
        }
        else if (_currentObject.tag == Constants.TagGrabbableCard)
        {
            _currentObject.GetComponent<GrabbableCardBhv>().CancelAction();
            _currentObject = null;
        }
    }
}
