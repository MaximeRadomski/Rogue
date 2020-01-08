using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeyBhv : MonoBehaviour
{
    public Vector3[] LayoutPositions;
    public Sprite[] Sprites;

    private GameObject _keyboard;
    //private GameObject _keyHover;
    private ButtonBhv _buttonBhv;
    private TMPro.TextMeshPro _target;
    private TMPro.TextMeshPro _textMeshLower;
    private TMPro.TextMeshPro _textMeshUpper;
    private SpriteRenderer _spriteRenderer;
    private string _originalTargetText;
    private string _lowerCase;
    private string _upperCase;
    private int _layoutId;
    private float _maxWidth;
    private bool _isUpperCase;

    public void SetPrivates(TMPro.TextMeshPro target, float maxWidth = -1)
    {
        _target = target;
        _originalTargetText = target.text;
        _maxWidth = maxWidth;
        _keyboard = this.transform.parent.gameObject;
        //_keyHover = _keyboard.transform.Find("KeyHover").gameObject;
        _buttonBhv = GetComponent<ButtonBhv>();
        _textMeshLower = transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _textMeshUpper = transform.GetChild(1).GetComponent<TMPro.TextMeshPro>();
        _spriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
        var name = this.gameObject.name;
        if (name.Contains("Layout"))
        {
            char layoutIdChar = name[CharacterAfterString(name, "Layout")];
            string layoutIdStr = layoutIdChar.ToString();
            _layoutId = int.Parse(layoutIdStr);
            _buttonBhv.EndActionDelegate = ChangeLayout;
        }
        else if (name.Contains("Shift"))
        {
            _buttonBhv.EndActionDelegate = Shift;
        }
        else if (name.Contains("Del"))
        {
            _buttonBhv.EndActionDelegate = Del;
        }
        else if (name.Contains("Close"))
        {
            _buttonBhv.EndActionDelegate = Close;
        }
        else if (name.Contains("Cancel"))
        {
            _buttonBhv.EndActionDelegate = Cancel;
        }
        else if (name.Contains("Validate"))
        {
            _buttonBhv.EndActionDelegate = Validate;
        }
        else if (name.Contains("Space"))
        {
            _upperCase = " ";
            _lowerCase = " ";
            _isUpperCase = false;
            UpdateTextMesh();
            _buttonBhv.EndActionDelegate = AddLetter;
        }
        else
        {
            _upperCase = name[CharacterAfterString(name, "Key")].ToString();
            _lowerCase = name[CharacterAfterString(name, "Key") + 1].ToString();
            _isUpperCase = false;
            UpdateTextMesh();
            _buttonBhv.EndActionDelegate = AddLetter;
        }
    }

    #region Layout

    private void ChangeLayout()
    {
        for (int i = 0; i < _keyboard.transform.childCount; ++i)
        {
            var inputKeyBhv = _keyboard.transform.GetChild(i).GetComponent<InputKeyBhv>();
            if (inputKeyBhv != null)
                inputKeyBhv.GoToLayoutPosition(_layoutId);
        }
    }

    public void GoToLayoutPosition(int idLayout)
    {
        transform.position = LayoutPositions[idLayout];
    }

    #endregion

    #region Shift

    private void Shift()
    {
        _isUpperCase = !_isUpperCase;
        if (_isUpperCase)
            _spriteRenderer.sprite = Sprites[1];
        else
            _spriteRenderer.sprite = Sprites[0];

        for (int i = 0; i < _keyboard.transform.childCount; ++i)
        {
            var inputKeyBhv = _keyboard.transform.GetChild(i).GetComponent<InputKeyBhv>();
            if (inputKeyBhv != null)
                inputKeyBhv.UpperLower(_isUpperCase);
        }
    }

    public void UpperLower(bool isUpperCase)
    {
        _isUpperCase = isUpperCase;
        UpdateTextMesh();
    }

    #endregion

    #region Del

    private void Del()
    {
        if (_target.text.Length > 0)
            _target.text = _target.text.Substring(0, _target.text.Length - 1);
    }

    #endregion

    #region Close

    private void Close()
    {
        Destroy(_keyboard.gameObject);
        --Constants.InputLayer;
    }

    #endregion

    #region Cancel

    private void Cancel()
    {
        _target.text = _originalTargetText;
        Close();
    }

    #endregion

    #region Validate

    private void Validate()
    {
        Close();
    }

    #endregion

    #region Letter

    private void UpdateTextMesh()
    {
        _textMeshUpper.enabled = _isUpperCase;
        _textMeshLower.enabled = !_isUpperCase;
    }

    private void AddLetter()
    {
        _target.text += _isUpperCase ? _upperCase : _lowerCase;
        if (_target.renderedWidth > _maxWidth)
            Del();
    }

    #endregion

    private int CharacterAfterString(string str, string subStr)
    {
        return str.IndexOf(subStr) + subStr.Length;
    }
}
