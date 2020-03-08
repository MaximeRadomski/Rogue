using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarBhv : MonoBehaviour
{
    private TMPro.TextMeshPro _text;
    private GameObject _content;
    private SpriteRenderer _contentSpriteRenderer;
    private float _width;
    private float? _frameHeight;

    void Start()
    {
        _text = transform.Find("Text").GetComponent<TMPro.TextMeshPro>();
        _content = transform.Find("Content").gameObject;
        _contentSpriteRenderer = _content.GetComponent<SpriteRenderer>();
        _width = _contentSpriteRenderer.sprite.rect.size.x * Constants.Pixel;
    }

    public void UpdateContent(int current, int max, string name, GameObject frame)
    {
        if (current < 0)
            current = 0;
        _text.text = name + "   (" + current + ")";
        float ratio = (float)current / max;
        _content.transform.localScale = new Vector3(1.0f * ratio, 1.0f, 1.0f);
        var space = _width * ((1.0f - ratio) / 2);
        _content.transform.position = new Vector3(transform.position.x - space, transform.position.y, 0.0f);

        if (frame != null)
        {
            var frameHealthBar = frame.transform.Find("HealthBar").gameObject;
            if (_frameHeight == null)
                _frameHeight = frameHealthBar.GetComponent<SpriteRenderer>().sprite.rect.size.y * Constants.Pixel;
            frameHealthBar.transform.localScale = new Vector3(1.0f, 1.0f * ratio, 1.0f);
            space = (_frameHeight ?? 0) * ((1.0f - ratio) / 2);
            frameHealthBar.transform.position = new Vector3(frameHealthBar.transform.position.x, frame.transform.position.y - space, 0.0f);
        }
    }
}
