using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

class PopupMerchantBhv : StatsDisplayerBhv
{
    public List<Sprite> _buySellSprites;

    private Character _character;
    private List<GameObject> _tabs;
    private GameObject _buttonPositive;
    private GameObject _selectedSprite;
    private int _selectedItem;
    private Vector3 _resetTabPosition;
    private Vector3 _currentTabPosition;
    private List<Vector3> _buttonPriceNPositivePosition;
    private TMPro.TextMeshPro _buttonPositiveText;
    private TMPro.TextMeshPro _priceText;
    private TMPro.TextMeshPro _playerGoldText;
    private TMPro.TextMeshPro _weightText;
    private bool _isBuying;
    private System.Func<bool, object> _afterManageAction;
    private AlignmentMerchant _alignment;
    private InventoryItemType _type;

    private List<InventoryItem> _items;
    private List<InventoryItem> _itemsForSale;
    private int _itemsInventoryPlace;

    public void SetPrivates(Character character, AlignmentMerchant alignment, InventoryItemType type, bool isBuying, System.Func<bool, object> afterManageAction, List<InventoryItem> itemsForSale)
    {
        _isBuying = isBuying;
        _alignment = alignment;
        _character = character;
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
        _buttonPositive = transform.Find("ButtonPositive").GetComponent<ButtonBhv>().gameObject;
        _buttonPositiveText = _buttonPositive.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
        _weightText = transform.Find("Weight").GetComponent<TMPro.TextMeshPro>();
        _priceText = transform.Find("ItemPrice").GetComponent<TMPro.TextMeshPro>();
        _playerGoldText = transform.Find("PlayerGold").GetComponent<TMPro.TextMeshPro>();
        _buttonPriceNPositivePosition = new List<Vector3>();
        _buttonPriceNPositivePosition.Add(_priceText.transform.position);
        _buttonPriceNPositivePosition.Add(_buttonPositive.transform.position);

        if (_isBuying)
        {
            if (itemsForSale == null)
                _itemsForSale = PopulateItemsForSale(type);
            else
                _itemsForSale = itemsForSale;
            _items = _itemsForSale;
            _itemsInventoryPlace = _itemsForSale.Count;
        }
        else
        {
            _itemsForSale = itemsForSale;
            _items = _character.Inventory;
            _itemsInventoryPlace = _character.InventoryPlace;
        }

        SetButtons();
    }

    private void SetButtons()
    {
        transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = _isBuying ? "Buy" : "Sell";
        transform.Find("ExitButton").GetComponent<ButtonBhv>().EndActionDelegate = ExitPopup;
        transform.Find("SwitchButton").GetComponent<ButtonBhv>().EndActionDelegate = SwitchBuySell;
        transform.Find("SwitchButton").GetComponent<SpriteRenderer>().sprite = _isBuying ? _buySellSprites[1] : _buySellSprites[0];
        _buttonPositive.GetComponent<ButtonBhv>().EndActionDelegate = PositiveAction;
        _weightText.gameObject.GetComponent<ButtonBhv>().EndActionDelegate = WeightAction;
        for (int i = 0; i < 6; ++i)
        {
            var slotBack = transform.Find("SlotBack" + i).gameObject;
            var slotIcon = transform.Find("SlotIcon" + i).gameObject;
            if (i < _items.Count && i < _itemsInventoryPlace) //Full
            {
                var item = _items[i];
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
            else if (i < _itemsInventoryPlace) //Empty
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
        Constants.LastEndActionClickedName = "SlotBack" + _selectedItem;
        UpdateView();
    }

    private void UpdateView()
    {
        foreach (var tab in _tabs)
        {
            tab.transform.position = _resetTabPosition;
        }
        if (_items.Count > 0)
        {
            var id = int.Parse(Constants.LastEndActionClickedName[Helper.CharacterAfterString(Constants.LastEndActionClickedName, "SlotBack")].ToString());
            _selectedItem = id < _items.Count ? id : _items.Count - 1;
            _selectedSprite.transform.position = transform.Find("SlotBack" + _selectedItem).transform.position;
            var item = _items[_selectedItem];
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
            _priceText.transform.position = _buttonPriceNPositivePosition[0];
            _buttonPositive.transform.position = _buttonPriceNPositivePosition[1];
            _priceText.text = item.GetPrice() + " " + Constants.UnitGold;
            _buttonPositiveText.text = _isBuying ? "Buy" : "Sell";
        }
        else //Inventory Empty
        {
            _priceText.transform.position = _resetTabPosition;
            _buttonPositive.transform.position = _resetTabPosition;
            _selectedSprite.transform.position = _resetTabPosition;
        }

        _playerGoldText.text = _character.Gold + " " + Constants.UnitGold;
        var content = _character.GetTotalWeight() + "/" + _character.WeightLimit;
        if (_character.GetTotalWeight() <= _character.WeightLimit)
            _weightText.text = "<material=\"LongWhite\">" + content + "</material>";
        else
            _weightText.text = "<material=\"LongRed\">" + content + "</material>";
    }

    private void PositiveAction()
    {

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

    private List<InventoryItem> PopulateItemsForSale(InventoryItemType type)
    {
        var items = new List<InventoryItem>();
        var nbItems = 3;
        if (Random.Range(0, 100) < 5)
            nbItems = 6;
        else if (Random.Range(0, 100) < 15)
            nbItems = 5;
        else if (Random.Range(0, 100) < 30)
            nbItems = 4;
        for (int i = 0; i < nbItems; ++i)
        {
            if (type == InventoryItemType.Weapon)
                items.Add(WeaponsData.GetRandomWeapon());
            else if (type == InventoryItemType.Skill)
                items.Add(SkillsData.GetRandomSkill());
            else if (type == InventoryItemType.Item)
                items.Add(ItemsData.GetRandomItem());
        }
        return items;
    }

    private void SwitchBuySell()
    {
        Constants.DecreaseInputLayer();
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.SetActive(false); //In order for the "GameObject.Find()" not to see any doublons
        _instantiator.NewPopupMerchant(_character, _alignment, _type, !_isBuying, _afterManageAction, _itemsForSale);
        Destroy(gameObject);
    }
}
