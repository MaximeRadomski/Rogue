using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PopupLootBhv : StatsDisplayerBhv
{
    private Character _character;
    private List<GameObject> _tabs;
    private Vector3 _resetTabPosition;
    private Vector3 _currentTabPosition;
    private TMPro.TextMeshPro _weightText;
    private TMPro.TextMeshPro _lootHistoryText;
    private InventoryItem _loot;

    private System.Func<bool, object> _afterManageAction;
    private System.Func<bool> _sceneUpdateAction;

    public void SetPrivates(Character character, System.Func<bool> sceneUpdateAction, System.Func<bool, object> afterManageAction, InventoryItem loot)
    {
        _character = character;
        _sceneUpdateAction = sceneUpdateAction;
        _afterManageAction = afterManageAction;
        _loot = loot;
        _resetTabPosition = new Vector3(-10.0f, -10.0f, 0.0f);
        _currentTabPosition = transform.position;
        _tabs = new List<GameObject>();
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
        for (int i = 0; i < 3; ++i)
        {
            _tabs.Add(transform.Find("Tab" + ((InventoryItemType)i).ToString()).gameObject);
        }
        _weightText = transform.Find("Weight").GetComponent<TMPro.TextMeshPro>();
        _lootHistoryText = transform.Find("LootHistory").GetComponent<TMPro.TextMeshPro>();
        SetButtons();
    }

    private void SetButtons()
    {
        transform.Find("ExitButton").GetComponent<ButtonBhv>().EndActionDelegate = ExitPopup;
        transform.Find("InventoryButton").GetComponent<ButtonBhv>().EndActionDelegate = SwitchToInventory;
        _weightText.gameObject.GetComponent<ButtonBhv>().EndActionDelegate = WeightAction;
        UpdateView();
    }

    private void UpdateView()
    {
        foreach (var tab in _tabs)
        {
            tab.transform.position = _resetTabPosition;
        }
        var item = _loot;
        switch (item.InventoryItemType)
        {
            case InventoryItemType.Weapon:
                DisplayStatsWeapon(_tabs[0], (Weapon)item, "SkinContainerWeapon", "StatsListWeaponLoot");
                _tabs[0].transform.position = _currentTabPosition;
                break;
            case InventoryItemType.Skill:
                DisplayStatsSkill(_tabs[1], (Skill)item, "SkinContainerSkill", "StatsListSkillLoot");
                _tabs[1].transform.position = _currentTabPosition;
                break;
            case InventoryItemType.Item:
                DisplayStatsItem(_tabs[2], (Item)item, "SkinContainerItem", "StatsListItemLoot");
                _tabs[2].transform.position = _currentTabPosition;
                break;
        }
        _lootHistoryText.text = _loot.LootHistory;
        var content = _character.GetTotalWeight() + "/" + _character.WeightLimit;
        if (_character.GetTotalWeight() <= _character.WeightLimit)
            _weightText.text = "<material=\"LongWhite\">" + content + "</material>";
        else
            _weightText.text = "<material=\"LongRed\">" + content + "</material>";
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

    private void SwitchToInventory()
    {
        Constants.DecreaseInputLayer();
        _instantiator.NewPopupInventory(_character, _sceneUpdateAction, _afterManageAction, _loot);
        Destroy(gameObject);
    }

    public override void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        _afterManageAction?.Invoke(false);
        Destroy(gameObject);
    }
}
