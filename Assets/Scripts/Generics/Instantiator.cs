﻿using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    void Start()
    {
        SetPrivates();
    }

    public void SetPrivates()
    {
    }

    public void EditViaKeyboard()
    {
        var target = GameObject.Find(Constants.LastEndActionClickedName);
        if (target == null)
            return;
        ShowKeyboard(target.GetComponent<TMPro.TextMeshPro>(), target.GetComponent<BoxCollider2D>().size.x);
    }

    public void ShowKeyboard(TMPro.TextMeshPro target, float maxWidth = -1)
    {
        var tmpKeyboardObject = Resources.Load<GameObject>("Prefabs/Keyboard");
        var tmpKeyboardInstance = Instantiate(tmpKeyboardObject, tmpKeyboardObject.transform.position, tmpKeyboardObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpKeyboardInstance.name);
        for (int i = 0; i < tmpKeyboardInstance.transform.childCount; ++i)
        {
            var inputKeyBhv = tmpKeyboardInstance.transform.GetChild(i).GetComponent<InputKeyBhv>();
            if (inputKeyBhv != null)
                inputKeyBhv.SetPrivates(target, maxWidth);
        }
        tmpKeyboardInstance.transform.Find("InputKeyLayout" + PlayerPrefs.GetInt(Constants.PpFavKeyboardLayout, Constants.PpFavKeyboardLayoutDefault)).GetComponent<InputKeyBhv>().ChangeLayout();
        if (target.transform.position.y < -Camera.main.orthographicSize + Constants.KeyboardHeight)
            Camera.main.gameObject.GetComponent<CameraBhv>().FocusY(target.transform.position.y + (Camera.main.orthographicSize - Constants.KeyboardHeight));
    }

    public void NewEffect(InventoryItemType type, Vector3 position, Vector3? charPosition, int effectId, int layer)
    {
        var tmpEffectObject = Resources.Load<GameObject>("Prefabs/Effect" + type + effectId.ToString("D2"));
        var tmpEffectInstance = Instantiate(tmpEffectObject, position, tmpEffectObject.transform.rotation);
        tmpEffectInstance.GetComponent<SpriteRenderer>().sortingOrder = (layer * 100) + 50; //8 is skin waist order
        if (charPosition != null)
            tmpEffectInstance.transform.eulerAngles = new Vector3(0.0f, 0.0f, Helper.GetAngleFromTwoPositions(charPosition ?? new Vector3(), position) + 90);
        StartCoroutine(Helper.ExecuteAfterDelay(1.0f, () =>
        {
            Destroy(tmpEffectInstance);
            return true;
        }, lockInputWhile:false));

    }

    public void NewPopupCharacterStats(Character character, System.Func<bool> sceneUpdateAction, bool isInventoryAvailable = false, int tabId = 0)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupCharacterStats");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupCharacterStatsBhv>().SetPrivates(character, sceneUpdateAction, isInventoryAvailable, tabId);
    }

    public void NewPopupInventory(Character character, System.Func<bool> updateAction, System.Func<bool, object> resultAction = null, InventoryItem loot = null)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupInventory");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupInventoryBhv>().SetPrivates(character, updateAction, resultAction, loot);
    }

    public void NewPopupLoot(Character character, System.Func<bool> updateAction, System.Func<bool, object> resultAction = null, InventoryItem loot = null)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupLoot");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupLootBhv>().SetPrivates(character, updateAction, resultAction, loot);
    }

    public void NewPopupMerchant(Character character, AlignmentMerchant alignment, InventoryItemType type, bool isBuying, System.Func<List<InventoryItem>, object> resultAction, List<InventoryItem> itemsForSale)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupMerchant");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupMerchantBhv>().SetPrivates(character, alignment, type, isBuying, resultAction, itemsForSale);
    }

    public void NewPopupYesNo(string title, string content, string negative, string positive,
        System.Func<bool, object> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupYesNo");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupYesNoBhv>().SetPrivates(title, content, negative, positive, resultAction);
    }

    public void NewPopupSwitch(InventoryItem item, int itemId, Character character,
        System.Func<bool, object> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupSwitch");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupSwitchBhv>().SetPrivates(item, itemId, character, resultAction);
    }

    public void NewPopupApplyItem(InventoryItem item, int itemId, Character character, InventoryItemType itemType,
        System.Func<InventoryItem, object> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupApplyItem");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupApplyItemBhv>().SetPrivates(item, itemId, character, itemType, resultAction);
    }

    public void NewOverBlend(OverBlendType overBlendType, string message, float? constantLoadingSpeed,
        System.Func<bool, object> resultAction, bool reverse = false)
    {
        var tmpOverBlendObject = Resources.Load<GameObject>("Prefabs/OverBlend");
        var tmpOverBlendInstance = Instantiate(tmpOverBlendObject, tmpOverBlendObject.transform.position, tmpOverBlendObject.transform.rotation);
        tmpOverBlendInstance.GetComponent<OverBlendBhv>().SetPrivates(overBlendType, message, constantLoadingSpeed, resultAction, reverse);
    }

    public void NewOverTitle(string text, string imagePath, System.Func<bool, object> resultAction, Direction mainDirection, Direction secondaryDirection = Direction.None)
    {
        var tmpOverTitleObject = Resources.Load<GameObject>("Prefabs/OverTitle");
        var tmpOverTitleInstance = Instantiate(tmpOverTitleObject, tmpOverTitleObject.transform.position, tmpOverTitleObject.transform.rotation);
        tmpOverTitleInstance.GetComponent<OverBlendTitleBhv>().SetPrivates(text, imagePath, resultAction, mainDirection, secondaryDirection);
    }

    public void NewSnack(string content, float duration = 2.0f)
    {
        var tmpSnackObject = Resources.Load<GameObject>("Prefabs/Snack");
        var tmpSnackInstance = Instantiate(tmpSnackObject, tmpSnackObject.transform.position, tmpSnackObject.transform.rotation);
        tmpSnackInstance.GetComponent<SnackBhv>().SetPrivates(content, duration);
    }

    public PauseMenuBhv NewPauseMenu()
    {
        var tmpPauseMenuObject = Resources.Load<GameObject>("Prefabs/PauseMenu");
        var tmpPauseMeuInstance = Instantiate(tmpPauseMenuObject, tmpPauseMenuObject.transform.position, tmpPauseMenuObject.transform.rotation);
        var pauseMenuBhv = tmpPauseMeuInstance.GetComponent<PauseMenuBhv>();
        pauseMenuBhv.SetPrivates();
        return pauseMenuBhv;
    }

    public void PopText(string text, Vector2 position, TextType type, TextThickness thickness = TextThickness.Thick)
    {
        var tmpPoppingTextObject = Resources.Load<GameObject>("Prefabs/PoppingText");

        var tmpTexts = GameObject.FindGameObjectsWithTag(Constants.TagPoppingText);
        var nbTextsOnThisPosition = 0;
        foreach (var tmpText in tmpTexts)
        {
            if (Helper.VectorEqualsPrecision(tmpText.GetComponent<PoppingTextBhv>().StartingPosition, position, 0.01f))
                ++nbTextsOnThisPosition;
        }

        var tmpPoppingTextInstance = Instantiate(tmpPoppingTextObject, position, tmpPoppingTextObject.transform.rotation);
        tmpPoppingTextInstance.GetComponent<PoppingTextBhv>().SetPrivates(text, position + new Vector2(0.0f, -0.3f * nbTextsOnThisPosition), type, thickness);
    }

    public void PopIcon(Sprite sprite, Vector2 position)
    {
        var tmpPoppingIconObject = Resources.Load<GameObject>("Prefabs/PoppingIcon");
        var tmpPoppingIconInstance = Instantiate(tmpPoppingIconObject, position, tmpPoppingIconObject.transform.rotation);
        tmpPoppingIconInstance.GetComponent<PoppingIconBhv>().SetPrivates(sprite, position + new Vector2(0.0f, +0.3f));
    }

    public void NewRandomCard(int id, int day, Biome biome, Character character)
    {
        if (!biome.EncounteredMerchant)
        {
            var merchant = Random.Range(0, 100);
            if (merchant < biome.MerchantPercent)
            {
                biome.EncounteredMerchant = true;
                NewCardMerchant(id, day, biome, character);
                return;
            }
        }
        if (!biome.EncounteredInn)
        {
            var inn = Random.Range(0, 100);
            if (inn < biome.InnPercent)
            {
                biome.EncounteredInn = true;
                NewCardInn(id, day, biome, character);
                return;
            }
        }

        var rand = Random.Range(0, 2);
        if (rand == 0)
            NewCardJourneyEvent(id, day, biome, character);
        else
            NewCardOpponent(id, day, biome, character);
    }

    public void NewCardOpponent(int id, int day, Biome biome, Character character)
    {
        var tmpCardObject = Resources.Load<GameObject>("Prefabs/CardOpponent");
        var tmpCardInstance = Instantiate(tmpCardObject, tmpCardObject.transform.position, tmpCardObject.transform.rotation);
        tmpCardInstance.GetComponent<CardOpponentBhv>().SetPrivates(id, day, biome, character, this);
    }

    public void NewCardJourneyEvent(int id, int day, Biome biome, Character character)
    {
        var tmpCardObject = Resources.Load<GameObject>("Prefabs/CardJourneyEvent");
        var tmpCardInstance = Instantiate(tmpCardObject, tmpCardObject.transform.position, tmpCardObject.transform.rotation);
        tmpCardInstance.GetComponent<CardJourneyEventBhv>().SetPrivates(id, day, biome, character, this);

    }

    public void NewCardInn(int id, int day, Biome biome, Character character)
    {
        var tmpCardObject = Resources.Load<GameObject>("Prefabs/CardInn");
        var tmpCardInstance = Instantiate(tmpCardObject, tmpCardObject.transform.position, tmpCardObject.transform.rotation);
        tmpCardInstance.GetComponent<CardInnBhv>().SetPrivates(id, day, biome, character, this);
    }

    public void NewCardMerchant(int id, int day, Biome biome, Character character)
    {
        var tmpCardObject = Resources.Load<GameObject>("Prefabs/CardMerchant");
        var tmpCardInstance = Instantiate(tmpCardObject, tmpCardObject.transform.position, tmpCardObject.transform.rotation);
        tmpCardInstance.GetComponent<CardMerchantBhv>().SetPrivates(id, day, biome, character, this);
    }

    public void NewCardBiome(int id, int day, Biome biome, int choice, int maxChoice, Character character)
    {
        var tmpCardObject = Resources.Load<GameObject>("Prefabs/CardBiome");
        var tmpCardInstance = Instantiate(tmpCardObject, tmpCardObject.transform.position, tmpCardObject.transform.rotation);
        tmpCardInstance.GetComponent<CardBiomeBhv>().SetPrivates(id, day, biome, character, this);
        tmpCardInstance.GetComponent<CardBiomeBhv>().SetChoice(choice, maxChoice);
    }

    public GameObject NewCharacterGameObject(string characterName, bool isPlayer = false, string id = "")
    {
        var character = PlayerPrefsHelper.GetCharacter(characterName);
        var characterObject = Resources.Load<GameObject>("Prefabs/Character" + CharacterRace.Human);
        var characterInstance = Instantiate(characterObject, new Vector2(-3.0f, -5.0f), characterObject.transform.rotation);
        LoadCharacterSkin(character, characterInstance.transform.Find("SkinContainer").gameObject);
        if (isPlayer)
            characterInstance.name = Constants.GoPlayerName;
        else
        {
            characterInstance.name = Constants.GoOpponentName + id;
        }
        var playerBhv = characterInstance.GetComponent<CharacterBhv>();
        playerBhv.X = 0;
        playerBhv.Y = 0;
        playerBhv.Character = character;
        playerBhv.Character.IsPlayer = isPlayer;
        return characterInstance;
    }

    public GameObject NewCharacterFrame(CharacterRace race, Vector3 position, int id, bool IsPlayer = false)
    {
        var tmpFrameObject = Resources.Load<GameObject>("Prefabs/FrameHuman"); //TODO Change by race
        var tmpPosition = tmpFrameObject.transform.position + position;
        if (IsPlayer)
            tmpPosition = position;
        var tmpFrameInstance = Instantiate(tmpFrameObject, tmpPosition, tmpFrameObject.transform.rotation);
        if (IsPlayer)
        {
            tmpFrameInstance.GetComponent<PositionBhv>().enabled = false;
            tmpFrameInstance.transform.parent = GameObject.Find("CharacterUI").transform;
            tmpFrameInstance.transform.Find("HealthBar").gameObject.SetActive(false);
        }
        tmpFrameInstance.name = "FrameCharacter" + id;
        return tmpFrameInstance;
    }

    public void LoadCharacterSkin(Character character, GameObject skinContainer)
    {
        for (int i = 0; i < character.BodyParts.Count; ++i)
        {
            var tmpBodyPart = skinContainer.transform.Find(RacesData.BodyParts[i]);
            if (tmpBodyPart != null)
            {
                tmpBodyPart.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet(character.BodyParts[i]);
            }       
        }
    }

    public void LoadWeaponSkin(Weapon weapon, GameObject skinContainer)
    {
        for (int i = 0; i < 5; ++i)
        {
            var tmpWeaponPart = skinContainer.transform.Find("WeaponPart"+i);
            var tmpWeaponPartShadow = skinContainer.transform.Find("WeaponPart" + i + "Shadow");
            if (i >= weapon.NbSkinParts)
            {
                tmpWeaponPart.GetComponent<SpriteRenderer>().enabled = false;
                tmpWeaponPartShadow.GetComponent<SpriteRenderer>().enabled = false;
                continue;
            }
            tmpWeaponPart.GetComponent<SpriteRenderer>().enabled = true;
            tmpWeaponPartShadow.GetComponent<SpriteRenderer>().enabled = true;
            tmpWeaponPart.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet(weapon.WeaponParts[i]);
            tmpWeaponPartShadow.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet(weapon.WeaponParts[i]);
        }
    }

    public void LoadSkillSkin(Skill skill, GameObject skinContainer)
    {
        skinContainer.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Skills/Skills_" + skill.IconId);
    }

    public void LoadItemSkin(Item item, GameObject skinContainer)
    {
        skinContainer.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Items/Items_" + item.IconId);
    }

    public GameObject NewCell(int x, int y, char c, Grid grid)
    {
        var cellGameObject = Resources.Load<GameObject>("Prefabs/Cell");
        var cellInstance = Instantiate(cellGameObject, cellGameObject.transform.position, cellGameObject.transform.rotation);
        cellInstance.transform.parent = grid.transform;
        cellInstance.transform.position = new Vector3(x * grid.cellSize.x, y * grid.cellSize.y, 0.0f) + grid.transform.position;
        cellInstance.gameObject.name = "Cell" + x + y;
        var cellBhv = cellInstance.GetComponent<CellBhv>();
        cellBhv.X = x;
        cellBhv.Y = y;
        cellBhv.Type = (CellType)int.Parse(c.ToString(), System.Globalization.NumberStyles.Integer);
        if (cellBhv.Type == CellType.Spawn || cellBhv.Type == CellType.OpponentSpawn)
            cellBhv.State = CellState.Spawn;
        else
            cellBhv.State = CellState.None;
        cellBhv.SetPrivates();
        return cellInstance;
    }

    public GameObject NewSoulStat(Vector3 position, int level, int id, string effect)
    {
        var soulStat = Resources.Load<GameObject>("Prefabs/SoulStat");
        var cellInstance = Instantiate(soulStat, position, soulStat.transform.rotation);
        cellInstance.transform.Find("Level").GetComponent<TMPro.TextMeshPro>().text = level.ToString();
        cellInstance.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsSoulTree_" + (id * 2));
        cellInstance.transform.Find("Effect").GetComponent<TMPro.TextMeshPro>().text = effect;
        return cellInstance;
    }
}
