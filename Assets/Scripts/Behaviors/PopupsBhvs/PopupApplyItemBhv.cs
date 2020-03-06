using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupApplyItemBhv : StatsDisplayerBhv
{
    public Sprite[] _backgrounds;

    private System.Func<InventoryItem, object> _resultAction;

    private int _mainItemId;
    private InventoryItem _mainItem;
    private Character _character;
    private GameObject _selectedSprite;
    private GameObject _positiveButton;
    private GameObject _weaponAffected;
    private GameObject _skillAffected;
    private List<Vector3> _containersPositions;
    private int _selectedItem;
    private InventoryItemType _itemType;

    public void SetPrivates(InventoryItem item, int itemId, Character character, InventoryItemType itemType,
        System.Func<InventoryItem, object> resultAction)
    {
        _mainItemId = itemId;
        _mainItem = item;
        _character = character;
        _resultAction = resultAction;
        _selectedItem = 0;
        _selectedSprite = transform.Find("SelectedSprite").gameObject;
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
        transform.position = Camera.main.transform.position;
        _weaponAffected = transform.Find("WeaponAffected").gameObject;
        _skillAffected = transform.Find("SkillAffected").gameObject;
        _itemType = itemType;
        if (_itemType == InventoryItemType.Weapon)
        {
            _skillAffected.SetActive(false);
            GetComponent<SpriteRenderer>().sprite = _backgrounds[0];
        }
        else
        {
            _weaponAffected.SetActive(false);
            GetComponent<SpriteRenderer>().sprite = _backgrounds[1];
        }
        SetButtons();
    }

    private void SetButtons()
    {
        (_positiveButton = transform.Find("ButtonPositive").gameObject).GetComponent<ButtonBhv>().EndActionDelegate = PositiveDelegate;
        transform.Find("ButtonNegative").GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;
        for (int i = 0; i < 2; ++i)
        {
            var slotBack = transform.Find("SlotBack" + i).gameObject;
            var slotIcon = transform.Find("SlotIcon" + i).gameObject;
            slotBack.GetComponent<ButtonBhv>().EndActionDelegate = UpdateView;
            InventoryItem item = _itemType == InventoryItemType.Weapon ? _character.Weapons[i] : (InventoryItem)_character.Skills[i];
            switch (item.InventoryItemType)
            {
                case InventoryItemType.Weapon:
                    slotIcon.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsWeapon_" + ((Weapon)item).Type.GetHashCode());
                    break;
                case InventoryItemType.Skill:
                    slotIcon.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsSkill_" + ((Skill)item).IconId);
                    break;
            }
        }
        DisplayItem(_mainItem, true);
        Constants.SetLastEndActionClickedName("SlotBack" + _selectedItem);
        UpdateView();
    }

    private void UpdateView()
    {
        var id = int.Parse(Constants.LastEndActionClickedName[Helper.CharacterAfterString(Constants.LastEndActionClickedName, "SlotBack")].ToString());
        _selectedItem = id;
        _selectedSprite.transform.position = transform.Find("SlotBack" + _selectedItem).transform.position;
        InventoryItem item = _itemType == InventoryItemType.Weapon ? _character.Weapons[_selectedItem] : (InventoryItem)_character.Skills[_selectedItem];
        DisplayItem(item);
    }

    private void DisplayItem(InventoryItem item, bool isMain = false)
    {
        if (isMain)
            DisplayStatsItem(transform.Find("ItemUsed").gameObject, (Item)item, "SkinContainerItem", null, false);
        switch (item.InventoryItemType)
        {
            case InventoryItemType.Weapon:
                DisplayStatsWeapon(_weaponAffected, (Weapon)item, "SkinContainerWeapon", null, false);
                break;
            case InventoryItemType.Skill:
                DisplayStatsSkill(_skillAffected, (Skill)item, "SkinContainerSkill", null, false);
                break;
        }
    }

    private void PositiveDelegate()
    {
        InventoryItem tmpItem = _itemType == InventoryItemType.Weapon ? _character.Weapons[_selectedItem] : (InventoryItem)_character.Skills[_selectedItem];
        Constants.DecreaseInputLayer();
        _resultAction(tmpItem);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Constants.DecreaseInputLayer();
        _resultAction(null);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        _resultAction(null);
        base.ExitPopup();
    }
}
