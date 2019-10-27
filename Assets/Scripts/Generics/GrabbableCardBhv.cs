using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableCardBhv : MonoBehaviour
{
    private SoundControlerBhv _soundControler;
    private Vector2 _initialTouchPosition;
    private Vector2 _touchPosition;
    private Vector2 _initialPosition;
    private Vector2 _likePosition;
    private Vector2 _dislikePosition;
    private CardState _state;
    private float _rotateAngle;
    private bool _isReseting;
    private bool _hasMoved;
    private bool _isStretching;
    private Vector3 _resetedScale;
    private Vector3 _pressedScale;

    void Start()
    {
        SetPrivates();
    }

    private void SetPrivates()
    {
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _initialPosition = new Vector2(0.0f, 0.0f);
        _likePosition = new Vector3(5.0f, 0.0f);
        _dislikePosition = new Vector3(-5.0f, 0.0f);
        _state = CardState.Active;
        _rotateAngle = -15.0f;
        _isReseting = false;
        _hasMoved = false;
        _isStretching = false;
        _resetedScale = new Vector3(1.0f, 1.0f, 1.0f);
        _pressedScale = new Vector3(1.05f, 1.05f, 1.0f);
    }

    public void BeginAction(Vector2 initialTouchPosition)
    {
        _isStretching = true;
        transform.localScale = _pressedScale;
        _soundControler.PlaySound(_soundControler.ClickIn);
        _isReseting = false;
        _initialTouchPosition = initialTouchPosition;
    }

    public void GrabAction(Vector2 touchPosition)
    {
        if (!_hasMoved && Vector2.Distance(_initialTouchPosition, touchPosition) > 0.1f)
            _hasMoved = true;
        if (_hasMoved)
        {
            transform.position = touchPosition - _initialTouchPosition;
            transform.eulerAngles = new Vector3(0.0f, 0.0f, _rotateAngle * (transform.position.x / 2));
        }
    }

    public void EndAction()
    {
        if (_hasMoved)
        {
            if (transform.position.x > 1.0f)
                Like();
            else if (transform.position.x < -1.0f)
                Dislike();
            else
                _isReseting = true;
        }
        else
            GameObject.Find("SampleText").GetComponent<UnityEngine.UI.Text>().text = "I CLICKED THE CARD LOL";
        _soundControler.PlaySound(_soundControler.ClickOut);
    }

    public void CancelAction()
    {
        _isReseting = true;
    }

    void Update()
    {
        if (_isStretching)
            StretchOnBegin();
        if (_state == CardState.Liked)
            Like();
        else if (_state == CardState.Disliked)
            Dislike();
        else if (_isReseting)
            ResetPosition();
    }

    private void StretchOnBegin()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _resetedScale, 0.1f);
        if (transform.localScale == _resetedScale)
            _isStretching = false;
    }

    private void ResetPosition()
    {
        transform.position = Vector2.Lerp(transform.position, _initialPosition, 0.3f);
        transform.eulerAngles = new Vector3(0.0f, 0.0f, _rotateAngle * (transform.position.x / 2));
        if ((Vector2)transform.position == _initialPosition)
        {
            _isReseting = false;
            _hasMoved = false;
        }
    }

    public void Like()
    {
        _state = CardState.Liked;
        transform.position = Vector2.Lerp(transform.position, _likePosition, 0.1f);
        transform.eulerAngles = new Vector3(0.0f, 0.0f, _rotateAngle * (transform.position.x / 2));
        if (Helpers.FloatEqualsPrecision(transform.position.x, _likePosition.x, 0.1f))
            HardReset();
    }

    public void Dislike()
    {
        _state = CardState.Disliked;
        transform.position = Vector2.Lerp(transform.position, _dislikePosition, 0.1f);
        transform.eulerAngles = new Vector3(0.0f, 0.0f, _rotateAngle * (transform.position.x / 2));
        if (Helpers.FloatEqualsPrecision(transform.position.x, _dislikePosition.x, 0.1f))
            HardReset();
    }

    private void HardReset()
    {
        _state = CardState.Active;
        transform.position = _initialPosition;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
    }
}

public enum CardState
{
    Active,
    Liked,
    Disliked
}
