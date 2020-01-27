using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupInventoryBhv : StatsDisplayerBhv
{
    private Character _character;
    private List<GameObject> _tabs;
    private int _selectedItem;
    private Vector3 _resetTabPosition;
    private Vector3 _currentTabPosition;

    public void SetPrivates(Character character)
    {
        _character = character;
        _selectedItem = 0;
        _resetTabPosition = new Vector3(-10.0f, -10.0f, 0.0f);
        _currentTabPosition = transform.position;
        _tabs = new List<GameObject>();
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
        for (int i = 0; i < 3; ++i)
        {
            _tabs.Add(transform.Find("Tab" + ((InventoryItemType)i).ToString()).gameObject);
        }
        SetButtons();
    }

    private void SetButtons()
    {
        transform.Find("ExitButton").GetComponent<ButtonBhv>().EndActionDelegate = ExitPopup;
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
                    case InventoryItemType.Consumable:
                        slotIcon.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsConsumable_" + ((Consumable)item).IconId);
                        break;
                }
            }
            else if (i < _character.InventoryPlace) //Empty
            {
                //slotBack.GetComponent<BoxCollider2D>().enabled = false;
                slotIcon.GetComponent<SpriteRenderer>().sprite = null;
            }
            else //Out of range
            {
                slotBack.GetComponent<BoxCollider2D>().enabled = false;
                slotBack.GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
                slotIcon.GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        Constants.LastEndActionClickedName = "SlotBack0";
        UpdateView();
    }

    private void UpdateView()
    {
        var id = int.Parse(Constants.LastEndActionClickedName[Helper.CharacterAfterString(Constants.LastEndActionClickedName, "SlotBack")].ToString());
        _selectedItem = id;
        transform.Find("SelectedSprite").transform.position = transform.Find("SlotBack" + _selectedItem).transform.position;
        var item = _character.Inventory[id];
        foreach (var tab in _tabs)
        {
            tab.transform.position = _resetTabPosition;
        }
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
            case InventoryItemType.Consumable:
                DisplayStatsConsumable(_tabs[2], (Consumable)item, "SkinContainerConsumable", "StatsListConsumable");
                _tabs[2].transform.position = _currentTabPosition;
                break;
        }
        transform.Find("Weight").GetComponent<TMPro.TextMeshPro>().text = _character.GetTotalWeight() + "/" + _character.WeightLimit;
    }

    public void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        Destroy(gameObject);
    }
}
