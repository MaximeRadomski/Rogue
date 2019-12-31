using System.Collections.Generic;
using UnityEngine;

public class GrabbableCardBhv : MonoBehaviour
{
    private SoundControlerBhv _soundControler;
    private SpriteRenderer _spriteRenderer;
    private SkinContainerBhv _skinContainerBhv;
    private SwipeSceneBhv _swipeSceneBhv;
    private BoxCollider2D[] _boxColliders2D;

    private List<Character> _opponentCharacters;

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
        _skinContainerBhv = transform.Find("SkinContainer").GetComponent<SkinContainerBhv>();
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
        InitOpponent();
        _boxColliders2D = gameObject.GetComponents<BoxCollider2D>();
        if (id == 0)
        {
            foreach (BoxCollider2D box in _boxColliders2D)
                box.enabled = false;
            transform.localScale = _disabledScale;
        }
    }

    private void HandleSortingLayerAndOrder(int id)
    {
        _skinContainerBhv.SetSkinContainerSortingLayer(Constants.SortingLayerCard);
        _skinContainerBhv.SetSkinContainerSortingLayerOrder(id);
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

    public void BringToFront()
    {
        _isStretching = true;
        gameObject.name = "Card1";
        HandleSortingLayerAndOrder(1);
        foreach (BoxCollider2D box in _boxColliders2D)
            box.enabled = true;
    }

    private void InitOpponent()
    {

        _opponentCharacters = new List<Character>();
        var nbOpponents = Random.Range(1, 6);
        Debug.Log("nbOpponents: " + nbOpponents);
        for (int i = 0; i < nbOpponents; ++i)
        {
            _opponentCharacters.Add(RacesData.GetCharacterFromRaceAndLevel((CharacterRace)Random.Range(0, Helper.EnumCount<CharacterRace>()), 1));
        }
        DisplayCharacterStats(0);
            
    }

    public void DisplayCharacterStats(int id)
    {
        transform.Find("OpponentName").GetComponent<TMPro.TextMeshPro>().text = _opponentCharacters[id].Name;
        transform.Find("OpponentRace").GetComponent<TMPro.TextMeshPro>().text = _opponentCharacters[id].Race.ToString();
        transform.Find("OpponentGender").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsGender_" + _opponentCharacters[id].Gender.GetHashCode());
        transform.Find("OpponentLevel").GetComponent<TMPro.TextMeshPro>().text = _opponentCharacters[id].Level.ToString();
        transform.Find("OpponentWeapon1Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsWeapon_" + _opponentCharacters[id].Weapons[0].Type.GetHashCode());
        transform.Find("OpponentWeapon1Rarity").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsRarity_" + _opponentCharacters[id].Weapons[0].Rarity.GetHashCode());
        transform.Find("OpponentWeapon2Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsWeapon_" + _opponentCharacters[id].Weapons[1].Type.GetHashCode());
        transform.Find("OpponentWeapon2Rarity").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsRarity_" + _opponentCharacters[id].Weapons[1].Rarity.GetHashCode());
        transform.Find("OpponentSkill1Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsSkill_" + _opponentCharacters[id].Skills[0].IconId);
        transform.Find("OpponentSkill1Rarity").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsRarity_" + _opponentCharacters[id].Skills[0].Rarity.GetHashCode());
        transform.Find("OpponentSkill2Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsSkill_" + _opponentCharacters[id].Skills[1].IconId);
        transform.Find("OpponentSkill2Rarity").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsRarity_" + _opponentCharacters[id].Skills[1].Rarity.GetHashCode());
        transform.Find("OpponentGold").GetComponent<TMPro.TextMeshPro>().text = _opponentCharacters[id].Gold + " ©";

        var nbOpponents = _opponentCharacters.Count;
        for (int i = 0; i < 6; ++i)
        {
            if (i < nbOpponents)
            {
                switch (i)
                {
                    case 0: transform.Find("OpponentNb1Back").GetComponent<ButtonBhv>().EndActionDelegate = SelectOpponent1; break;
                    case 1: transform.Find("OpponentNb2Back").GetComponent<ButtonBhv>().EndActionDelegate = SelectOpponent2; break;
                    case 2: transform.Find("OpponentNb3Back").GetComponent<ButtonBhv>().EndActionDelegate = SelectOpponent3; break;
                    case 3: transform.Find("OpponentNb4Back").GetComponent<ButtonBhv>().EndActionDelegate = SelectOpponent4; break;
                    case 4: transform.Find("OpponentNb5Back").GetComponent<ButtonBhv>().EndActionDelegate = SelectOpponent5; break;
                    case 5: transform.Find("OpponentNb6Back").GetComponent<ButtonBhv>().EndActionDelegate = SelectOpponent6; break;
                }
            }
            else
            {
                var tmpToCopyBox = transform.Find("OpponentNb" + (i + 1) + "Back").GetComponent<BoxCollider2D>();
                var tmpNewBox = gameObject.AddComponent<BoxCollider2D>();
                tmpNewBox.offset = new Vector2(tmpToCopyBox.offset.x, tmpToCopyBox.offset.y) + (Vector2)tmpToCopyBox.transform.position - (Vector2)transform.position;
                tmpNewBox.size = new Vector2(tmpToCopyBox.size.x, tmpToCopyBox.size.y);
                transform.Find("OpponentNb" + (i + 1)).gameObject.SetActive(false);
                transform.Find("OpponentNb" + (i + 1) + "Back").gameObject.SetActive(false);
            }
            if (i == id)
                transform.Find("SelectedSprite").transform.position = transform.Find("OpponentNb" + (i + 1) + "Back").transform.position;
        }

        Instantiator.LoadCharacterSkin(_opponentCharacters[id], _skinContainerBhv.gameObject);
    }

    public void SelectOpponent1() { DisplayCharacterStats(0); }
    public void SelectOpponent2() { DisplayCharacterStats(1); }
    public void SelectOpponent3() { DisplayCharacterStats(2); }
    public void SelectOpponent4() { DisplayCharacterStats(3); }
    public void SelectOpponent5() { DisplayCharacterStats(4); }
    public void SelectOpponent6() { DisplayCharacterStats(5); }

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
        if (_state == CardState.Liked)
            Venture();
        else if (_state == CardState.Disliked)
            Avoid();
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

    public void Venture()
    {
        _state = CardState.Liked;
        transform.position = Vector2.Lerp(transform.position, _likePosition, 0.1f);
        transform.eulerAngles = new Vector3(0.0f, 0.0f, _rotateAngle * (transform.position.x / 2));
        if (Helper.FloatEqualsPrecision(transform.position.x, _likePosition.x, 0.1f))
            _swipeSceneBhv.GoToFightScene(_opponentCharacters);
    }

    public void Avoid()
    {
        _state = CardState.Disliked;
        transform.position = Vector2.Lerp(transform.position, _dislikePosition, 0.1f);
        transform.eulerAngles = new Vector3(0.0f, 0.0f, _rotateAngle * (transform.position.x / 2));
        if (Helper.FloatEqualsPrecision(transform.position.x, _dislikePosition.x, 1.0f))
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
