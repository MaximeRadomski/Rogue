using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardBhv : MonoBehaviour
{
    internal SoundControlerBhv _soundControler;
    internal SpriteRenderer _spriteRenderer;
    internal SpriteRenderer _cacheSpriteRenderer;
    internal SwipeSceneBhv _swipeSceneBhv;
    internal BoxCollider2D[] _boxColliders2D;

    internal Vector2 _initialTouchPosition;
    internal Vector2 _initialPosition;
    internal Vector2 _likePosition;
    internal Vector2 _dislikePosition;
    internal CardState _state;
    internal float _rotateAngle;
    internal bool _isReseting;
    internal bool _hasMoved;
    internal bool _isStretching;
    internal Vector3 _resetedScale;
    internal Vector3 _pressedScale;
    internal Vector3 _disabledScale;

    public virtual void SetPrivates(int id)
    {
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _cacheSpriteRenderer = transform.Find("Cache")?.GetComponent<SpriteRenderer>();
        _swipeSceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SwipeSceneBhv>();

        _initialPosition = Constants.CardInitialPosition;
        _initialTouchPosition = _initialPosition;
        _likePosition = new Vector3(6.0f, 0.0f);
        _dislikePosition = new Vector3(-6.0f, 0.0f);
        _state = CardState.Active;
        _rotateAngle = -15.0f;
        _isReseting = false;
        _hasMoved = false;
        _isStretching = false;
        _resetedScale = new Vector3(1.0f, 1.0f, 1.0f);
        _pressedScale = new Vector3(1.05f, 1.05f, 1.0f);
        _disabledScale = new Vector3(0.95f, 0.95f, 1.0f);
        gameObject.name = "Card" + id;
        //_canvas.overrideSorting = true;
        //_canvas.sortingLayerName = Constants.SortingLayerCard;
        HandleSortingLayerAndOrder(id);
        _boxColliders2D = gameObject.GetComponents<BoxCollider2D>();
        if (id == 0)
        {
            foreach (BoxCollider2D box in _boxColliders2D)
                box.enabled = false;
            transform.localScale = _disabledScale;
        }
    }

    internal virtual void HandleSortingLayerAndOrder(int id)
    {
        _spriteRenderer.sortingOrder = id * 99;
        for (int i = 0; i < transform.childCount; ++i)
        {
            int currentOrder = 0;
            var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
            var textMesh = transform.GetChild(i).GetComponent<TMPro.TextMeshPro>();
            if (spriteRenderer != null) currentOrder = spriteRenderer.sortingOrder;
            if (textMesh != null) currentOrder = textMesh.sortingOrder;

            int toSubstract = currentOrder / 100;
            int decimals = currentOrder - (toSubstract * 100);

            if (spriteRenderer != null) spriteRenderer.sortingOrder = (id * 100) + decimals;
            if (textMesh != null) textMesh.sortingOrder = (id * 100) + decimals;

            var boxCollider = transform.GetChild(i).GetComponent<BoxCollider2D>();
            if (boxCollider != null && id == 1)
                boxCollider.enabled = true;
            else if (boxCollider != null)
                boxCollider.enabled = false;
        }
    }

    public void BeginAction(Vector2 initialTouchPosition)
    {
        _initialTouchPosition = initialTouchPosition;
        _isStretching = true;
        transform.localScale = _pressedScale;
        _soundControler.PlaySound(_soundControler.ClickIn);
        _isReseting = false;
    }

    public void GrabAction(Vector2 touchPosition)
    {
        if (_initialTouchPosition == _initialPosition)
            return;
        if (!_hasMoved && Vector2.Distance(_initialTouchPosition, touchPosition) > 0.1f)
            _hasMoved = true;
        if (_hasMoved)
        {
            transform.position = touchPosition - _initialTouchPosition + _initialPosition;
            transform.eulerAngles = new Vector3(0.0f, 0.0f, _rotateAngle * (transform.position.x / 2));
        }
    }

    public void EndAction()
    {
        if (_hasMoved)
        {
            if (transform.position.x > 1.0f)
                Venture();
            else if (transform.position.x < -1.0f)
                Avoid();
            else
                _isReseting = true;
        }
        _soundControler.PlaySound(_soundControler.ClickOut);
        _initialTouchPosition = _initialPosition;
    }

    public void CancelAction()
    {
        _isReseting = true;
    }

    void Update()
    {
        if (_isStretching)
            StretchOnBegin();
        if (_state == CardState.Ventured)
            Venture();
        else if (_state == CardState.Avoided)
            Avoid();
        else if (_isReseting)
            ResetPosition();
    }

    private void StretchOnBegin()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _resetedScale, 0.1f);
        if (_cacheSpriteRenderer != null && _cacheSpriteRenderer.color != Constants.ColorPlainTransparent)
            _cacheSpriteRenderer.color = Color.Lerp(_cacheSpriteRenderer.color, Constants.ColorPlainTransparent, 0.1f);
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

    private void HardReset()
    {
        _state = CardState.Active;
        transform.position = _initialPosition;
        transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public virtual void BringToFront()
    {
        _isStretching = true;
        gameObject.name = "Card1";
        HandleSortingLayerAndOrder(1);
        foreach (BoxCollider2D box in _boxColliders2D)
            box.enabled = true;
    }

    public virtual void Venture()
    {
        _state = CardState.Ventured;
        transform.position = Vector2.Lerp(transform.position, _likePosition, 0.1f);
        transform.eulerAngles = new Vector3(0.0f, 0.0f, _rotateAngle * (transform.position.x / 2));
    }

    public virtual void Avoid()
    {
        _state = CardState.Avoided;
        transform.position = Vector2.Lerp(transform.position, _dislikePosition, 0.1f);
        transform.eulerAngles = new Vector3(0.0f, 0.0f, _rotateAngle * (transform.position.x / 2));
        if (Helper.FloatEqualsPrecision(transform.position.x, _dislikePosition.x, 1.0f))
            _swipeSceneBhv.NewCard();
    }
}

public enum CardState
{
    Active,
    Ventured,
    Avoided
}
