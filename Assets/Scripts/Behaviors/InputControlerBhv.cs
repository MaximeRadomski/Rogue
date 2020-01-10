using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControlerBhv : MonoBehaviour
{
    private Vector3 _touchPosWorld;
    private InputBhv _currentInput;
    private bool _beginPhase, _doPhase, _endPhase;

    void Update()
    {
        var currentFrameInputLayer = Constants.InputLayer;
        // IF SCREEN TOUCH //
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
                RaycastHit2D[] hitsInformation = Physics2D.RaycastAll(touchPosWorld2D, Camera.main.transform.forward);
                foreach (var hitInformation in hitsInformation)
                {
                    if (hitInformation.collider != null)
                    {
                        CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
                        _currentInput = hitInformation.transform.gameObject.GetComponent<InputBhv>();
                        if (_currentInput?.Layer < currentFrameInputLayer)
                            continue;
                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                            _currentInput.BeginAction(touchPosWorld2D);
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                        {
                            Constants.LastEndActionClickedName = _currentInput.name;
                            _currentInput.EndAction(touchPosWorld2D);
                            _currentInput = null;
                        }
                        else
                            _currentInput.DoAction(touchPosWorld2D);
                    }
                    else
                        CancelCurrentObjectIfNewBeforeEnd();
                }
            }
        }
        else
        {
            _beginPhase = _doPhase = _endPhase = false;
            // IF MOUSE //
            if ((_beginPhase = Input.GetMouseButtonDown(0))
                || (_endPhase = Input.GetMouseButtonUp(0))
                || (_doPhase = Input.GetMouseButton(0)))
            {
                _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
                RaycastHit2D[] hitsInformation = Physics2D.RaycastAll(touchPosWorld2D, Camera.main.transform.forward);
                foreach (var hitInformation in hitsInformation)
                {
                    if (hitInformation.collider != null)
                    {
                        CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
                        _currentInput = hitInformation.transform.gameObject.GetComponent<InputBhv>();
                        if (_currentInput?.Layer < currentFrameInputLayer)
                            continue;
                        if (_beginPhase)
                            _currentInput.BeginAction(touchPosWorld2D);
                        else if (_endPhase)
                        {
                            Constants.LastEndActionClickedName = _currentInput.name;
                            _currentInput.EndAction(touchPosWorld2D);
                            _currentInput = null;
                        }
                        else if (_doPhase)
                            _currentInput.DoAction(touchPosWorld2D);
                    }
                    else
                        CancelCurrentObjectIfNewBeforeEnd();
                }
            }
            // ELSE //
            else
                _touchPosWorld = new Vector3(-99, -99, -99);
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
        //    RaycastHit2D[] hitsInformation = Physics2D.RaycastAll(touchPosWorld2D, Camera.main.transform.forward);
        //    foreach (var hitInformation in hitsInformation)
        //    {
        //        if (hitInformation.collider != null)
        //        {
        //            CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
        //            _currentInput = hitInformation.transform.gameObject.GetComponent<InputBhv>();
        //            if (_currentInput?.Layer < Constants.InputLayer)
        //                continue;
        //            _currentInput.BeginAction(touchPosWorld2D);
        //        }
        //        else
        //            CancelCurrentObjectIfNewBeforeEnd();
        //    }
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
        //    RaycastHit2D[] hitsInformation = Physics2D.RaycastAll(touchPosWorld2D, Camera.main.transform.forward);
        //    foreach (var hitInformation in hitsInformation)
        //    {
        //        if (hitInformation.collider != null)
        //        {
        //            CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
        //            _currentInput = hitInformation.transform.gameObject.GetComponent<InputBhv>();
        //            if (_currentInput?.Layer < Constants.InputLayer)
        //                continue;
        //            Constants.LastEndActionClickedName = _currentInput.name;
        //            _currentInput.EndAction(touchPosWorld2D);
        //            _currentInput = null;
        //        }
        //        else
        //            CancelCurrentObjectIfNewBeforeEnd();
        //    }
        //}
        //else if (Input.GetMouseButton(0))
        //{
        //    _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Vector2 touchPosWorld2D = new Vector2(_touchPosWorld.x, _touchPosWorld.y);
        //    RaycastHit2D[] hitsInformation = Physics2D.RaycastAll(touchPosWorld2D, Camera.main.transform.forward);
        //    foreach (var hitInformation in hitsInformation)
        //    {
        //        if (hitInformation.collider != null)
        //        {
        //            CancelCurrentObjectIfNewBeforeEnd(hitInformation.transform.gameObject);
        //            _currentInput = hitInformation.transform.gameObject.GetComponent<InputBhv>();
        //            if (_currentInput?.Layer < Constants.InputLayer)
        //                continue;
        //            _currentInput.DoAction(touchPosWorld2D);
        //        }
        //        else
        //            CancelCurrentObjectIfNewBeforeEnd();
        //    }
        //}
        //else
        //    _touchPosWorld = new Vector3(-99,-99,-99);
    }

    private void CancelCurrentObjectIfNewBeforeEnd(GameObject touchedGameObject = null)
    {
        if (_currentInput == null || _currentInput == touchedGameObject)
            return;
        _currentInput.CancelAction();
        _currentInput = null;
    }
}
