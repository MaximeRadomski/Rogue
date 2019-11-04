using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoppingTextBhv : MonoBehaviour
{
    private UnityEngine.UI.Text _text;
    private bool _isMoving;
    private Vector2 _positionToReach;
    private Color _colorToFade;
    private Color _shadowColorToFade;
    private UnityEngine.UI.Shadow _textShadow;


    public void SetPrivates(string text, Vector2 startingPosition, TextType type)
    {
        transform.position = new Vector2(startingPosition.x, startingPosition.y + 0.6f);
        _positionToReach = new Vector2(startingPosition.x, startingPosition.y + 1.1f);
        _text = GetComponent<UnityEngine.UI.Text>();
        _text.color = Helper.ColorFromTextType(type.GetHashCode());
        _text.text = text;
        _textShadow = GetComponent<UnityEngine.UI.Shadow>();
        _colorToFade = new Color(_text.color.r, _text.color.g, _text.color.b, 0.0f);
        _shadowColorToFade = Helper.ColorFromTextType(-1);
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
            _text.color = Color.Lerp(_text.color, _colorToFade, 0.05f);
            _textShadow.effectColor = Color.Lerp(_textShadow.effectColor, _shadowColorToFade, 0.05f);
            if (_text.color == _colorToFade)
            {
                _isMoving = false;
                Destroy(gameObject);
            }
        }
    }
}
