using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBhv : MonoBehaviour
{
    public delegate void ActionDelegate();
    public ActionDelegate BeginActionDelegate;
    public ActionDelegate DoActionDelegate;
    public ActionDelegate EndActionDelegate;

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
        _resetedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
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
        _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _resetedColor, 0.1f);
        if (_spriteRenderer.color == _resetedColor)
            _isResetingColor = false;
    }
}
