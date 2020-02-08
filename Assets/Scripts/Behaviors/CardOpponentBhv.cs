using System.Collections.Generic;
using UnityEngine;

public class CardOpponentBhv : CardBhv
{
    private SkinContainerBhv _skinContainerBhv;
    private List<Character> _opponentCharacters;
    private List<BoxCollider2D> _opponentsBox;
    private BoxCollider2D _fixBox;
    private float _originalFixBoxY;
    private float _customFixBoxY;

    public override void SetPrivates(int id, int day, MapType mapType, Character character, Instantiator instantiator)
    {
        _minutesNeededVenturePositive = Helper.RandomIntMultipleOf(10, 30, 5);
        _minutesNeededVentureNegative = _minutesNeededVenturePositive * 2;
        _fixBox = gameObject.GetComponents<BoxCollider2D>()[1];
        _originalFixBoxY = _fixBox.size.y;
        _customFixBoxY = 1.3f;
        _skinContainerBhv = transform.Find("SkinContainer").GetComponent<SkinContainerBhv>();
        _instantiator = instantiator;
        base.SetPrivates(id, day, mapType, character, instantiator);
        InitOpponent(day);
        _minutesNeededAvoid = _opponentCharacters.Count * 10 + 20;
        _boxColliders2D = gameObject.GetComponents<BoxCollider2D>();
        if (id == 0)
        {
            foreach (BoxCollider2D box in _boxColliders2D)
                box.enabled = false;
            transform.localScale = _disabledScale;
        }
        PositiveOutcomePercent = CalculateMatchPercent();
    }

    private int CalculateMatchPercent()
    {
        return 100;
    }

    private void InitOpponent(int day)
    {

        _opponentCharacters = new List<Character>();
        var nbOpponents = 1;
        if (Random.Range(0, 100) < 5)
            nbOpponents = 4;
        else if (Random.Range(0, 100) < 15)
            nbOpponents = 3;
        else if (Random.Range(0, 100) < 30)
            nbOpponents = 2;
        if (nbOpponents == 1)
            transform.Find("Opponents").GetComponent<TMPro.TextMeshPro>().text = "Opponent";
        for (int i = 0; i < nbOpponents; ++i)
        {
            _opponentCharacters.Add(RacesData.GetCharacterFromRaceAndLevel(
                (CharacterRace)Random.Range(0, Helper.EnumCount<CharacterRace>()),
                Random.Range(1, day + 1)));
        }
        DisplayCharacterStats(0);
            
    }

    protected override void HandleSortingLayerAndOrder(int id)
    {
        _skinContainerBhv.SetSkinContainerSortingLayer(Constants.SortingLayerCard);
        _skinContainerBhv.SetSkinContainerSortingLayerOrder(id);
        base.HandleSortingLayerAndOrder(id);
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

        if (_opponentsBox == null)
            _opponentsBox = new List<BoxCollider2D>();
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
                if (i <= _opponentsBox.Count)
                    _opponentsBox.Add(transform.Find("OpponentNb" + (i + 1) + "Back").GetComponent<BoxCollider2D>());
            }
            else if (_boxColliders2D.Length <= 2)
            {
                var tmpToCopyBox = transform.Find("OpponentNb" + (i + 1) + "Back").GetComponent<BoxCollider2D>();
                var tmpNewBox = gameObject.AddComponent<BoxCollider2D>();
                tmpNewBox.offset = new Vector2(tmpToCopyBox.offset.x, tmpToCopyBox.offset.y) + (Vector2)tmpToCopyBox.transform.position - (Vector2)transform.position;
                tmpNewBox.size = new Vector2(tmpToCopyBox.size.x, tmpToCopyBox.size.y * 1.2f);
                transform.Find("OpponentNb" + (i + 1)).gameObject.SetActive(false);
                transform.Find("OpponentNb" + (i + 1) + "Back").gameObject.SetActive(false);
            }
            if (i == id)
                transform.Find("SelectedSprite").transform.position = transform.Find("OpponentNb" + (i + 1) + "Back").transform.position;
        }

        _instantiator.LoadCharacterSkin(_opponentCharacters[id], _skinContainerBhv.gameObject);
    }

    public void SelectOpponent1() { DisplayCharacterStats(0); }
    public void SelectOpponent2() { DisplayCharacterStats(1); }
    public void SelectOpponent3() { DisplayCharacterStats(2); }
    public void SelectOpponent4() { DisplayCharacterStats(3); }
    public void SelectOpponent5() { DisplayCharacterStats(4); }
    public void SelectOpponent6() { DisplayCharacterStats(5); }

    public override void Venture()
    {
        base.Venture();
        if (Helper.FloatEqualsPrecision(transform.position.x, _likePosition.x, 0.1f))
            _swipeSceneBhv.GoToFightScene(_opponentCharacters);
    }

    public override void BeginAction(Vector2 initialTouchPosition)
    {
        ResetFixBox();
        base.BeginAction(initialTouchPosition);
    }

    public override void DoAction(Vector2 touchPosition)
    {
        EnableFixBox();
        base.DoAction(touchPosition);
    }

    public override void EndAction(Vector2 lastTouchPosition)
    {
        ResetFixBox();
        base.EndAction(lastTouchPosition);
    }

    private void EnableFixBox()
    {
        _fixBox.size = new Vector2(_fixBox.size.x, _customFixBoxY);
        foreach (var box in _opponentsBox)
        {
            box.enabled = false;
        }
    }

    private void ResetFixBox()
    {
        _fixBox.size = new Vector2(_fixBox.size.x, _originalFixBoxY);
        foreach (var box in _opponentsBox)
        {
            box.enabled = true;
        }
    }
}