using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSwitchBhv : MonoBehaviour
{
    private System.Func<bool, object> _resultAction;

    private int _mainItemId;
    private InventoryItem _mainItem;
    private Character _character;
    private Instantiator _instantiator;
    private GameObject _selectedSprite;
    private List<GameObject> _weaponContainers;
    private List<GameObject> _skillContainers;
    private List<Vector3> _containersPositions;
    private Vector3 _resetContainerPosition;
    private int _selectedItem;
    private InventoryItemType _itemType;

    public void SetPrivates(InventoryItem item, int itemId, Character character,
        System.Func<bool, object> resultAction)
    {
        _mainItemId = itemId;
        _mainItem = item;
        _character = character;
        _resultAction = resultAction;
        _selectedItem = 0;
        _selectedSprite = transform.Find("SelectedSprite").gameObject;
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
        transform.position = Camera.main.transform.position;
        _itemType = item.InventoryItemType == InventoryItemType.Weapon ? InventoryItemType.Weapon : InventoryItemType.Skill;
        transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = "Switch " + (_itemType == InventoryItemType.Weapon ? "Weapon" : "Switch");
        _weaponContainers = new List<GameObject>();
        _weaponContainers.Add(transform.Find("SkinContainerWeapon" + 0).gameObject);
        _weaponContainers.Add(transform.Find("SkinContainerWeapon" + 1).gameObject);
        _skillContainers = new List<GameObject>();
        _skillContainers.Add(transform.Find("SkinContainerSkill" + 0).gameObject);
        _skillContainers.Add(transform.Find("SkinContainerSkill" + 1).gameObject);
        _containersPositions = new List<Vector3>();
        _containersPositions.Add(_weaponContainers[0].transform.position);
        _containersPositions.Add(_weaponContainers[1].transform.position);
        _resetContainerPosition = new Vector3(-10.0f, -10.0f, 0.0f);
        SetButtons();
    }

    private void SetButtons()
    {
        transform.Find("ButtonPositive").GetComponent<ButtonBhv>().EndActionDelegate = PositiveDelegate;
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
        Constants.LastEndActionClickedName = "SlotBack" + _selectedItem;
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
        int id = isMain ? 1 : 0;
        switch (item.InventoryItemType)
        {
            case InventoryItemType.Weapon:
                transform.Find("Icon" + id).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsWeapon_" + ((Weapon)item).Type.GetHashCode());
                _instantiator.LoadWeaponSkin((Weapon)item, _weaponContainers[id]);
                _weaponContainers[id].transform.position = _containersPositions[id];
                _skillContainers[id].transform.position = _resetContainerPosition;
                break;
            case InventoryItemType.Skill:
                transform.Find("Icon" + id).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsSkill_" + ((Skill)item).IconId);
                _instantiator.LoadSkillSkin((Skill)item, _skillContainers[id]);
                _skillContainers[id].transform.position = _containersPositions[id];
                _weaponContainers[id].transform.position = _resetContainerPosition;
                break;
        }
    }

    private void PositiveDelegate()
    {
        InventoryItem tmpItem = _itemType == InventoryItemType.Weapon ? _character.Weapons[_selectedItem] : (InventoryItem)_character.Skills[_selectedItem];
        if (_itemType == InventoryItemType.Weapon)
            _character.Weapons[_selectedItem] = (Weapon)_mainItem;
        else
        {
            var skill = (Skill)_mainItem;
            if (skill.Type == SkillType.Racial && _character.Race != skill.Race)
            {
                //TODO SNACK :D
                return;
            }
            _character.Skills[_selectedItem] = skill;
        }            

        _character.Inventory[_mainItemId] = tmpItem;

        Constants.DecreaseInputLayer();
        _resultAction(true);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Constants.DecreaseInputLayer();
        _resultAction(false);
        Destroy(gameObject);
    }
}
