using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupInventoryBhv : StatsDisplayerBhv
{
    private Character _character;
    private List<GameObject> _tabs;
    private List<GameObject> _buttons;
    private GameObject _selectedSprite;
    private int _selectedItem;
    private Vector3 _resetTabPosition;
    private Vector3 _currentTabPosition;
    private List<Vector3> _buttonsPosition;
    private List<TMPro.TextMeshPro> _buttonsText;
    private TMPro.TextMeshPro _weightText;

    private System.Func<bool, object> _afterManageAction;
    private System.Func<bool> _sceneUpdateAction;

    public void SetPrivates(Character character, System.Func<bool> sceneUpdateAction, System.Func<bool, object> afterManageAction)
    {
        _character = character;
        _sceneUpdateAction = sceneUpdateAction;
        _afterManageAction = afterManageAction;
        _selectedItem = 0;
        _selectedSprite = transform.Find("SelectedSprite").gameObject;
        _resetTabPosition = new Vector3(-10.0f, -10.0f, 0.0f);
        _currentTabPosition = transform.position;
        _tabs = new List<GameObject>();
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
        for (int i = 0; i < 3; ++i)
        {
            _tabs.Add(transform.Find("Tab" + ((InventoryItemType)i).ToString()).gameObject);
        }
        _buttons = new List<GameObject>();
        _buttons.Add(transform.Find("ButtonPositive").GetComponent<ButtonBhv>().gameObject);
        _buttons.Add(transform.Find("ButtonNegative").GetComponent<ButtonBhv>().gameObject);
        _buttonsText = new List<TMPro.TextMeshPro>();
        _buttonsText.Add(_buttons[0].transform.GetChild(0).GetComponent<TMPro.TextMeshPro>());
        _buttonsText.Add(_buttons[1].transform.GetChild(0).GetComponent<TMPro.TextMeshPro>());
        _weightText = transform.Find("Weight").GetComponent<TMPro.TextMeshPro>();
        _buttonsPosition = new List<Vector3>();
        _buttonsPosition.Add(_buttons[0].transform.position);
        _buttonsPosition.Add(_buttons[1].transform.position);
        SetButtons();
    }

    private void SetButtons()
    {
        transform.Find("ExitButton").GetComponent<ButtonBhv>().EndActionDelegate = ExitPopup;
        if (_afterManageAction == null)
            transform.Find("StatsButton").GetComponent<ButtonBhv>().EndActionDelegate = SwitchToStats;
        else
            transform.Find("StatsButton").gameObject.SetActive(false);
        _buttons[0].GetComponent<ButtonBhv>().EndActionDelegate = PositiveAction;
        _buttons[1].GetComponent<ButtonBhv>().EndActionDelegate = NegativeAction;
        _weightText.gameObject.GetComponent<ButtonBhv>().EndActionDelegate = WeightAction;
        for (int i = 0; i < 6; ++i)
        {
            var slotBack = transform.Find("SlotBack" + i).gameObject;
            var slotIcon = transform.Find("SlotIcon" + i).gameObject;
            if (i < _character.Inventory.Count && i < _character.InventoryPlace) //Full
            {
                var item = _character.Inventory[i];
                slotBack.GetComponent<ButtonBhv>().EndActionDelegate = UpdateView;
                switch (item.InventoryItemType)
                {
                    case InventoryItemType.Weapon:
                        slotIcon.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsWeapon_" + ((Weapon)item).Type.GetHashCode());
                        break;
                    case InventoryItemType.Skill:
                        slotIcon.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsSkill_" + ((Skill)item).IconId);
                        break;
                    case InventoryItemType.Item:
                        slotIcon.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsItem_" + ((Item)item).IconId);
                        break;
                }
            }
            else if (i < _character.InventoryPlace) //Empty
            {
                slotBack.GetComponent<ButtonBhv>().EndActionDelegate = null;
                //slotBack.GetComponent<BoxCollider2D>().enabled = false;
                slotIcon.GetComponent<SpriteRenderer>().sprite = null;
            }
            else //Out of range
            {
                slotBack.GetComponent<ButtonBhv>().EndActionDelegate = null;
                slotBack.GetComponent<BoxCollider2D>().enabled = false;
                slotBack.GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
                slotIcon.GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        Constants.SetLastEndActionClickedName("SlotBack" + _selectedItem);
        UpdateView();
    }

    private void UpdateView()
    {
        foreach (var tab in _tabs)
        {
            tab.transform.position = _resetTabPosition;
        }
        if (_character.Inventory.Count > 0)
        {
            var id = int.Parse(Constants.LastEndActionClickedName[Helper.CharacterAfterString(Constants.LastEndActionClickedName, "SlotBack")].ToString());
            _selectedItem = id < _character.Inventory.Count ? id : _character.Inventory.Count - 1;
            _selectedSprite.transform.position = transform.Find("SlotBack" + _selectedItem).transform.position;
            var item = _character.Inventory[_selectedItem];
            switch (item.InventoryItemType)
            {
                case InventoryItemType.Weapon:
                    DisplayStatsWeapon(_tabs[0], (Weapon)item, "SkinContainerWeapon", "StatsListWeapon");
                    _tabs[0].transform.position = _currentTabPosition;
                    break;
                case InventoryItemType.Skill:
                    DisplayStatsSkill(_tabs[1], (Skill)item, "SkinContainerSkill", "StatsListSkill");
                    _tabs[1].transform.position = _currentTabPosition;
                    break;
                case InventoryItemType.Item:
                    DisplayStatsItem(_tabs[2], (Item)item, "SkinContainerItem", "StatsListItem");
                    _tabs[2].transform.position = _currentTabPosition;
                    break;
            }
            _buttons[0].transform.position = _buttonsPosition[0];
            _buttons[1].transform.position = _buttonsPosition[1];
            _buttonsText[0].text = item.PositiveAction;
            _buttonsText[1].text = item.NegativeAction;
        }
        else //Inventory Empty
        {
            _buttons[0].transform.position = _resetTabPosition;
            _buttons[1].transform.position = _resetTabPosition;
            _selectedSprite.transform.position = _resetTabPosition;
        }
        
        var content = _character.GetTotalWeight() + "/" + _character.WeightLimit;
        if (_character.GetTotalWeight() <= _character.WeightLimit)
            _weightText.text = "<material=\"LongWhite\">" + content + "</material>";
        else
            _weightText.text = "<material=\"LongRed\">" + content + "</material>";
    }

    private void NegativeAction()
    {
        _instantiator.NewPopupYesNo(Constants.YesNoTitle, Constants.YesNoContent, Constants.Cancel, Constants.Proceed, OnDiscard);
    }

    private object OnDiscard(bool result = false)
    {
        if (result)
        {
            _character.Inventory.RemoveAt(_selectedItem);
            SetButtons();
        }
        return result;
    }

    private void PositiveAction()
    {
        if (_character.Inventory[_selectedItem].InventoryItemType == InventoryItemType.Item)
        {
            ((Item)_character.Inventory[_selectedItem]).OnUse(_character, _selectedItem, OnPositiveAction);
        }
        else
        {
            _instantiator.NewPopupSwitch(_character.Inventory[_selectedItem], _selectedItem, _character, OnPositiveAction);
        }
    }

    private object OnPositiveAction(bool result)
    {
        if (result)
        {
            SetButtons();
            _sceneUpdateAction?.Invoke();
        }
        return result;
    }

    private void WeightAction()
    {
        bool overweight = _character.GetTotalWeight() > _character.WeightLimit;
        var content = "You can carry up to " + _character.WeightLimit + " " + Constants.UnitWeight + " worth of items.";
        var positive = "Ok";
        if (overweight)
        {
            content = "You are in overweight. Any action will take two times more time.";
            positive = "Damn";
        }
        _instantiator.NewPopupYesNo("Weight", content, string.Empty, positive, null);
    }

    private void SwitchToStats()
    {
        Constants.DecreaseInputLayer();
        _instantiator.NewPopupCharacterStats(_character, _sceneUpdateAction, isInventoryAvailable:true);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        _afterManageAction?.Invoke(false);
        Destroy(gameObject);
    }
}
