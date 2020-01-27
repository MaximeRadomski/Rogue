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
            var item = _character.Inventory[i];
            var slotBack = transform.Find("SlotBack" + i).gameObject;
            var slotIcon = transform.Find("SlotIcon" + i).gameObject;
            if (i < _character.Inventory.Count && i < _character.InventoryPlace) //Full
            {
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
                        slotIcon.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsConsumables_" + ((Consumable)item).IconId);
                        break;
                }
            }
            else if (i < _character.InventoryPlace) //Empty
            {
                slotBack.GetComponent<BoxCollider>().enabled = false;
                slotIcon.GetComponent<SpriteRenderer>().sprite = null;
            }
            else //Out of range
            {
                slotBack.GetComponent<BoxCollider>().enabled = false;
                slotIcon.GetComponent<SpriteRenderer>().sprite = null;
                slotIcon.GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
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
        switch (item.InventoryItemType)
        {
            case InventoryItemType.Weapon:
                DisplayStatsWeapon(_tabs[0], (Weapon)item, "SkinContainerWeapon", "StatsListWeapon");
                break;
            case InventoryItemType.Skill:
                DisplayStatsSkill(_tabs[1], (Skill)item, "SkinContainerSkill", "StatsListSkill");
                break;
            case InventoryItemType.Consumable:
                DisplayStatsConsumable(_tabs[2], (Consumable)item, "SkinContainerConsumable", "StatsListConsumable");
                break;
        }
    }

    public void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        Destroy(gameObject);
    }
}
