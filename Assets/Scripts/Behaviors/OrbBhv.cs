using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbBhv : MonoBehaviour
{
    private TMPro.TextMeshPro _textMesh;
    private GameObject _content;
    private Vector3 _originalPosition;
    private float _height;

    void Start()
    {
        _textMesh = transform.Find(gameObject.name + "Text").GetComponent<TMPro.TextMeshPro>();
        _content = transform.Find(gameObject.name + "Mask").Find(gameObject.name + "Content").gameObject;
        _originalPosition = _content.transform.position;
        _height = _content.GetComponent<SpriteRenderer>().size.y;
    }

    public void UpdateContent(int current, int max)
    {
        _textMesh.text = current.ToString();
        float ratio = (float)current / max;
        _content.transform.position = _originalPosition + new Vector3(0.0f, (_height * ratio) - _height, 0.0f);
    }
}
