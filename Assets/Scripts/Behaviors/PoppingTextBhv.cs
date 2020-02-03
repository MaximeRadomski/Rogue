using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoppingTextBhv : MonoBehaviour
{
    private TMPro.TextMeshPro _text;
    private string _material;
    private bool _isMoving;
    private Vector2 _positionToReach;
    private Color _colorToFade;
    private Color _shadowColorToFade;


    public void SetPrivates(string text, Vector2 startingPosition, TextType type, TextThickness thickness)
    {
        transform.position = new Vector2(startingPosition.x, startingPosition.y + 0.6f);
        _positionToReach = new Vector2(startingPosition.x, startingPosition.y + 1.1f);
        _text = GetComponent<TMPro.TextMeshPro>();
        _material = Helper.MaterialFromTextType(type.GetHashCode(), thickness);
        _text.text = "<material=\"" + _material + "\">" + text + "</material>";
        _colorToFade = new Color(_text.color.r, _text.color.g, _text.color.b, 0.0f);
        _isMoving = true;
    }

    private void Update()
    {
        if (_isMoving)
            MoveAndFade();
    }

    private void MoveAndFade()
    {
        transform.position = Vector2.Lerp(transform.position, _positionToReach, 0.05f);
        if (transform.position.y >= _positionToReach.y - 0.05f)
        {
            tag = Constants.TagCell;
            _text.color = Color.Lerp(_text.color, _colorToFade, 0.1f);
            if (_text.color == _colorToFade)
            {
                _isMoving = false;
                Destroy(gameObject);
            }
        }
    }
}
