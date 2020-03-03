using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbBhv : MonoBehaviour
{
    private TMPro.TextMeshPro _textMesh;
    private GameObject _content;
    private SpriteRenderer _contentSpriteRenderer;
    private float _height;

    void Start()
    {
        _textMesh = transform.Find(gameObject.name + "Text").GetComponent<TMPro.TextMeshPro>();
        _content = transform.Find(gameObject.name + "Mask").Find(gameObject.name + "Content").gameObject;
        _height = _content.GetComponent<SpriteRenderer>().size.y;
        _contentSpriteRenderer = _content.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_contentSpriteRenderer.transform.position.x + _height  <= transform.position.x)
            _contentSpriteRenderer.transform.position = new Vector3(transform.position.x + _height, _contentSpriteRenderer.transform.position.y, 0.0f);
        else
            _contentSpriteRenderer.transform.position += new Vector3(-0.005f, 0.0f, 0.0f);
    }

    public void UpdateContent(int current, int max, Instantiator instantiator, TextType textType, int? changingAmount = null)
    {
        if (changingAmount != null && changingAmount != 0)
        {
            instantiator.PopText((changingAmount >= 0 ? "+" : "") + changingAmount, transform.position + new Vector3(0.0f, 0.5f, 0.0f), textType);
        }
        _textMesh.text = current.ToString();
        float ratio = (float)current / max;
        _content.transform.position = new Vector3(_content.transform.position.x,
            transform.position.y + (_height * ratio) - _height,
            0.0f);
    }
}
