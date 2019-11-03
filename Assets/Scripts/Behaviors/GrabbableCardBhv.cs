using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableCardBhv : MonoBehaviour
{
    private SoundControlerBhv _soundControler;
    private SpriteRenderer _spriteRenderer;
    private SwipeSceneBhv _swipeSceneBhv;
    private BoxCollider2D _boxCollider2D;
    private Canvas _canvas;

    private Character _opponentCharacter;

    private Vector2 _initialTouchPosition;
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
    private Vector3 _disabledScale;

    public void SetPrivates(int id)
    {
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _swipeSceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SwipeSceneBhv>();
        _canvas = transform.GetChild(0).GetComponent<Canvas>();

        _initialPosition = Constants.CardInitialPosition;
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
        _spriteRenderer.sortingOrder = id;
        _canvas.overrideSorting = true;
        _canvas.sortingLayerName = "Card";
        _canvas.sortingOrder = id;
        if (id == 0)
        {
            _boxCollider2D.enabled = false;
            transform.localScale = _disabledScale;
        }
        InitOpponent();
    }

    public void BringToFront()
    {
        _isStretching = true;
        gameObject.name = "Card1";
        _spriteRenderer.sortingOrder = 1;
        _canvas.sortingOrder = 1;
        _boxCollider2D.enabled = true;
    }

    private void InitOpponent()
    {
        _opponentCharacter = RacesData.GetCharacterFromRaceAndLevel((CharacterRace)Random.Range(0, Helpers.EnumCount<CharacterRace>()), 1);
        DisplayCharacterStats();
    }

    public void DisplayCharacterStats()
    {
        _canvas.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Name:" + _opponentCharacter.Name;
        _canvas.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = "Gender:" + _opponentCharacter.Gender;
        _canvas.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "Race:" + _opponentCharacter.Race;
        _canvas.transform.GetChild(3).GetComponent<UnityEngine.UI.Text>().text = "Level:" + _opponentCharacter.Level;

        _canvas.transform.GetChild(4).GetComponent<UnityEngine.UI.Text>().text = _opponentCharacter.Weapons[0].Name;
        _canvas.transform.GetChild(4).GetComponent<UnityEngine.UI.Text>().color = Helpers.ColorFromTextType(_opponentCharacter.Weapons[0].Rarity.GetHashCode());
        _canvas.transform.GetChild(5).GetComponent<UnityEngine.UI.Text>().text = "Type:" + _opponentCharacter.Weapons[0].Type;
        _canvas.transform.GetChild(6).GetComponent<UnityEngine.UI.Text>().text = "Rarity:" + _opponentCharacter.Weapons[0].Rarity;

        _canvas.transform.GetChild(7).GetComponent<UnityEngine.UI.Text>().text = _opponentCharacter.Weapons[1].Name;
        _canvas.transform.GetChild(7).GetComponent<UnityEngine.UI.Text>().color = Helpers.ColorFromTextType(_opponentCharacter.Weapons[1].Rarity.GetHashCode());
        _canvas.transform.GetChild(8).GetComponent<UnityEngine.UI.Text>().text = "Type:" + _opponentCharacter.Weapons[1].Type;
        _canvas.transform.GetChild(9).GetComponent<UnityEngine.UI.Text>().text = "Rarity:" + _opponentCharacter.Weapons[1].Rarity;
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
            transform.position = touchPosition - _initialTouchPosition + _initialPosition;
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
            _swipeSceneBhv.GoToFightScene(_opponentCharacter);
    }

    public void Dislike()
    {
        _state = CardState.Disliked;
        transform.position = Vector2.Lerp(transform.position, _dislikePosition, 0.1f);
        transform.eulerAngles = new Vector3(0.0f, 0.0f, _rotateAngle * (transform.position.x / 2));
        if (Helpers.FloatEqualsPrecision(transform.position.x, _dislikePosition.x, 1.0f))
            _swipeSceneBhv.NewCard();
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
