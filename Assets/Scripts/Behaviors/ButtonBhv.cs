using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBhv : MonoBehaviour
{
    public delegate void ActionDelegate();
    public ActionDelegate BeginActionDelegate;
    public ActionDelegate DoActionDelegate;
    public ActionDelegate EndActionDelegate;
    public bool Disabled;

    private SpriteRenderer _spriteRenderer;
    private SoundControlerBhv _soundControler;

    private bool _isStretching;
    private Vector3 _resetedScale;
    private Vector3 _pressedScale;
    private bool _isResetingColor;
    private Color _resetedColor;
    private Color _pressedColor;

    void Start()
    {
        SetPrivates();
    }

    private void SetPrivates()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();

        _isStretching = false;
        _resetedScale = new Vector3(1.0f, 1.0f, 1.0f);
        _pressedScale = new Vector3(1.2f, 1.1f, 1.0f);
        _isResetingColor = false;
        _resetedColor = Constants.ColorPlain;
        _pressedColor = new Color(0.85f, 0.85f, 0.85f, 1.0f);
    }

    public void BeginAction()
    {
        _soundControler.PlaySound(_soundControler.ClickIn);
        _isStretching = true;
        _isResetingColor = false;
        transform.localScale = _pressedScale;
        if (_spriteRenderer != null)
            _spriteRenderer.color = _pressedColor;
        BeginActionDelegate?.Invoke();
    }

    public void DoAction()
    {
        DoActionDelegate?.Invoke();
    }

    public void EndAction()
    {
        _soundControler.PlaySound(_soundControler.ClickOut);
        _isResetingColor = true;
        EndActionDelegate?.Invoke();
    }

    public void CancelAction()
    {
        _isResetingColor = true;
    }

    private void Update()
    {
        if (_isStretching)
            StretchOnBegin();
        if (_isResetingColor)
            ResetColorOnEnd();
    }

    private void StretchOnBegin()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _resetedScale, 0.2f);
        if (transform.localScale == _resetedScale)
            _isStretching = false;
    }

    private void ResetColorOnEnd()
    {
        if (_spriteRenderer == null)
        {
            _isResetingColor = false;
            return;
        }
        if (Disabled)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorPlainSemiTransparent, 0.1f);
            if (_spriteRenderer.color == Constants.ColorPlainSemiTransparent)
                _isResetingColor = false;
        }
        else
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _resetedColor, 0.1f);
            if (_spriteRenderer.color == _resetedColor)
                _isResetingColor = false;
        }
    }

    public void DisableButton()
    {
        Disabled = true;
        _spriteRenderer.color = Constants.ColorPlainSemiTransparent;
        GetComponent<BoxCollider2D>().enabled = false;
        for (int i = 0; i < transform.childCount; ++i)
        {
            var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
            var textMesh = transform.GetChild(i).GetComponent<TMPro.TextMeshPro>();
            if (spriteRenderer != null) spriteRenderer.color = Constants.ColorPlainSemiTransparent;
            if (textMesh != null) textMesh.color = Constants.ColorPlainSemiTransparent;
            var boxCollider = transform.GetChild(i).GetComponent<BoxCollider2D>();
            if (boxCollider != null)
                boxCollider.enabled = false;
        }
    }

    public void EnableButton()
    {
        Disabled = false;
        _spriteRenderer.color = Constants.ColorPlain;
        GetComponent<BoxCollider2D>().enabled = true;
        for (int i = 0; i < transform.childCount; ++i)
        {
            var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
            var textMesh = transform.GetChild(i).GetComponent<TMPro.TextMeshPro>();
            if (spriteRenderer != null) spriteRenderer.color = Constants.ColorPlain;
            if (textMesh != null) textMesh.color = Constants.ColorPlain;
            var boxCollider = transform.GetChild(i).GetComponent<BoxCollider2D>();
            if (boxCollider != null)
                boxCollider.enabled = false;
        }
    }
}
