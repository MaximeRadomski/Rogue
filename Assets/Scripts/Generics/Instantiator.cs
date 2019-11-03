using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    private GameObject _canvas;

    void Start()
    {
        SetPrivates();
    }

    public void SetPrivates()
    {
        _canvas = GameObject.Find("Canvas");
    }

    public void PopText(string text, Vector2 position, TextType type)
    {
        var tmpPoppingTextObject = Resources.Load<GameObject>("Prefabs/PoppingText");
        var tmpPoppingTextInstance = Instantiate(tmpPoppingTextObject, position, tmpPoppingTextObject.transform.rotation, _canvas.transform);
        tmpPoppingTextInstance.GetComponent<PoppingTextBhv>().SetPrivates(text, position, type);
    }

    public void NewCard(int id)
    {
        var tmpCardObject = Resources.Load<GameObject>("Prefabs/Card");
        var tmpCardInstance = Instantiate(tmpCardObject, tmpCardObject.transform.position, tmpCardObject.transform.rotation, _canvas.transform);
        tmpCardInstance.GetComponent<GrabbableCardBhv>().SetPrivates(id);
    }
}
