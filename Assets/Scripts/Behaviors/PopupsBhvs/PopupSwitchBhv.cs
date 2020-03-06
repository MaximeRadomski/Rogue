using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSwitchBhv : StatsDisplayerBhv
{
    public Sprite[] _backgrounds;

    private System.Func<bool, object> _resultAction;

    private int _mainItemId;
    private InventoryItem _mainItem;
    private Character _character;
    private GameObject _selectedSprite;
    private GameObject _positiveButton;
    private List<GameObject> _weaponContainers;
    private List<GameObject> _skillContainers;
    private List<Vector3> _containersPositions;
    private int _selectedItem;
    private InventoryItemType _itemType;
    private bool _isSwitching;

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
        _weaponContainers = new List<GameObject>();
        _weaponContainers.Add(transform.Find("Weapon" + 0).gameObject);
        _weaponContainers.Add(transform.Find("Weapon" + 1).gameObject);
        _skillContainers = new List<GameObject>();
        _skillContainers.Add(transform.Find("Skill" + 0).gameObject);
        _skillContainers.Add(transform.Find("Skill" + 1).gameObject);
        _itemType = item.InventoryItemType == InventoryItemType.Weapon ? InventoryItemType.Weapon : InventoryItemType.Skill;
        if (_itemType == InventoryItemType.Weapon)
        {
            _skillContainers[0].SetActive(false);
            _skillContainers[1].SetActive(false);
            transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = "Switch Weapon";
            GetComponent<SpriteRenderer>().sprite = _backgrounds[0];
        }
        else
        {
            _weaponContainers[0].SetActive(false);
            _weaponContainers[1].SetActive(false);
            transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = "Switch Skill";
            GetComponent<SpriteRenderer>().sprite = _backgrounds[1];
        }
        _isSwitching = false;
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
        int id = isMain ? 1 : 0;
        switch (item.InventoryItemType)
        {
            case InventoryItemType.Weapon:
                DisplayStatsWeapon(_weaponContainers[id], (Weapon)item, "SkinContainerWeapon", null, false);                
                break;
            case InventoryItemType.Skill:
                DisplayStatsSkill(_skillContainers[id], (Skill)item, "SkinContainerSkill", null, false);
                break;
        }
    }

    private void PositiveDelegate()
    {
        InventoryItem tmpItem = _itemType == InventoryItemType.Weapon ? _character.Weapons[_selectedItem] : (InventoryItem)_character.Skills[_selectedItem];
        if (_itemType == InventoryItemType.Weapon)
        {
            var weapon = (Weapon)_mainItem;
            var notSelectedWeapon = _selectedItem == 0 ? 1 : 0;
            //GreatSword check
            if ((weapon.Type == WeaponType.GreatSword && !WeaponsData.IsSmallWeapon(_character.Weapons[notSelectedWeapon].Type))
                || (_character.Weapons[notSelectedWeapon].Type == WeaponType.GreatSword && !WeaponsData.IsSmallWeapon(weapon.Type)))
            {
                _instantiator.NewSnack("Great Swords can only be equipped with small weapons (knives, daggers, gauntlets).");
                NegativeDelegate();
                return;
            }
            _character.Weapons[_selectedItem] = weapon;
            DisplayItem(_character.Weapons[_selectedItem]);
        }
        else
        {
            var skill = (Skill)_mainItem;
            if (skill.Type == SkillType.Racial && _character.Race != skill.Race)
            {
                _instantiator.NewSnack("You do not have the proper race to use this skill");
                NegativeDelegate();
                return;
            }
            _character.Skills[_selectedItem] = skill;
            DisplayItem(_character.Skills[_selectedItem]);
        }
        _character.Inventory[_mainItemId] = tmpItem;
        DisplayItem(_character.Inventory[_mainItemId], true);
        _positiveButton.GetComponent<BoxCollider2D>().enabled = false;
        Invoke(nameof(AfterSwitch), 0.5f);
        _containersPositions = new List<Vector3>();
        if (_itemType == InventoryItemType.Weapon)
        {
            _containersPositions.Add(_weaponContainers[0].transform.position);
            _containersPositions.Add(_weaponContainers[1].transform.position);
            _weaponContainers[0].transform.position = _containersPositions[1];
            _weaponContainers[1].transform.position = _containersPositions[0];
        }
        else
        {
            _containersPositions.Add(_skillContainers[0].transform.position);
            _containersPositions.Add(_skillContainers[1].transform.position);
            _skillContainers[0].transform.position = _containersPositions[1];
            _skillContainers[1].transform.position = _containersPositions[0];
        }
        _isSwitching = true;
    }

    private void Update()
    {
        if (_isSwitching)
        {
            if (_itemType == InventoryItemType.Weapon)
            {
                _weaponContainers[0].transform.position = Vector3.Lerp(_weaponContainers[0].transform.position, _containersPositions[0], 0.5f);
                _weaponContainers[1].transform.position = Vector3.Lerp(_weaponContainers[1].transform.position, _containersPositions[1], 0.5f);
            }
            else
            {
                _skillContainers[0].transform.position = Vector3.Lerp(_skillContainers[0].transform.position, _containersPositions[0], 0.5f);
                _skillContainers[1].transform.position = Vector3.Lerp(_skillContainers[1].transform.position, _containersPositions[1], 0.5f);
            }
        }
    }

    private void AfterSwitch()
    {
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

    public override void ExitPopup()
    {
        _resultAction(false);
        base.ExitPopup();
    }
}
