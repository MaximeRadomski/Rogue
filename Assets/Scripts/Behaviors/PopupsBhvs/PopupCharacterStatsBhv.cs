using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PopupCharacterStatsBhv : StatsDisplayerBhv
{
    private Character _character;
    private SkinContainerBhv _skinContainerBhv;
    private List<GameObject> _tabs;
    private List<ButtonBhv> _buttonsTabs;
    private TMPro.TextMeshPro _title;
    private int _currentTab;
    private Vector3 _resetTabPosition;
    private Vector3 _currentTabPosition;

    private System.Func<bool> _sceneUpdateAction;

    public void SetPrivates(Character character, System.Func<bool> sceneUpdateAction, bool isInventoryAvailable, int tabId)
    {
        _title = transform.Find("Title").GetComponent<TMPro.TextMeshPro>();
        _character = character;
        _sceneUpdateAction = sceneUpdateAction;
        _currentTab = tabId;
        _resetTabPosition = new Vector3(-10.0f, 10.0f, 0.0f);
        _currentTabPosition = transform.position;
        _tabs = new List<GameObject>();
        _buttonsTabs = new List<ButtonBhv>();
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
        for (int i = 0; i < 5; ++i)
        {
            _tabs.Add(transform.Find("Tab" + i).gameObject);
            _buttonsTabs.Add(transform.Find("ButtonTab" + i).GetComponent<ButtonBhv>());
            if (i == _currentTab)
            {
                BringTabFront(_tabs[i]);
            }
            else
            {
                UnpressButton(_buttonsTabs[i].gameObject);
                BringTabBack(_tabs[i], i);
            }
        }
        _skinContainerBhv = _tabs[0].transform.Find("SkinContainer").GetComponent<SkinContainerBhv>();
        SetButtons(isInventoryAvailable);
        DisplayStatsCharacter();
        DisplayStatsWeapon(_tabs[1], _character.Weapons[0], "SkinContainerWeapon0", "StatsList1");
        DisplayStatsWeapon(_tabs[2], _character.Weapons[1], "SkinContainerWeapon1", "StatsList2");
        DisplayStatsSkill(_tabs[3], _character.Skills[0], "SkinContainerSkill0", "StatsList3");
        DisplayStatsSkill(_tabs[4], _character.Skills[1], "SkinContainerSkill1", "StatsList4");
    }

    private void SetButtons(bool isInventoryAvailable)
    {
        foreach (var button in _buttonsTabs)
        {
            button.EndActionDelegate = ChangeTab;
        }
        transform.Find("ExitButton").GetComponent<ButtonBhv>().EndActionDelegate = ExitPopup;
        var inventoryButton = transform.Find("InventoryButton");
        if (isInventoryAvailable)
            inventoryButton.GetComponent<ButtonBhv>().EndActionDelegate = SwitchToInventory;
        else
            inventoryButton.gameObject.SetActive(false);
        _tabs[1].transform.Find("SkinContainerWeapon0").GetComponent<ButtonBhv>().BeginActionDelegate = RandomizeWeapon;
        _tabs[2].transform.Find("SkinContainerWeapon1").GetComponent<ButtonBhv>().BeginActionDelegate = RandomizeWeapon;
    }

    private void RandomizeWeapon()
    {
        var idWeapon = Constants.LastEndActionClickedName.Contains("0") ? 0 : 1;
        var weapon = _character.Weapons[idWeapon];
        weapon.WeaponParts = WeaponsData.CreateWeaponPartsFromTypeSubType(weapon.Type, Random.Range(0, WeaponsData.WeaponTypeNames[weapon.Type.GetHashCode()].Length), weapon.NbSkinParts);
        _instantiator.LoadWeaponSkin(weapon, _tabs[idWeapon + 1].transform.Find("SkinContainerWeapon" + idWeapon).gameObject);
    }

    private void DisplayStatsCharacter()
    {
        _instantiator.LoadCharacterSkin(_character, _skinContainerBhv.gameObject);
        _tabs[0].transform.Find("Name").GetComponent<TMPro.TextMeshPro>().text = _character.Name;
        _tabs[0].transform.Find("Race").GetComponent<TMPro.TextMeshPro>().text = _character.Race.ToString();
        _tabs[0].transform.Find("Gender").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsGender_" + _character.Gender.GetHashCode());
        _tabs[0].transform.Find("Hp").GetComponent<TMPro.TextMeshPro>().text = _character.Hp.ToString();
        _tabs[0].transform.Find("HpMax").GetComponent<TMPro.TextMeshPro>().text = _character.HpMax.ToString();
        _tabs[0].transform.Find("Pa").GetComponent<TMPro.TextMeshPro>().text = _character.PaMax.ToString();
        _tabs[0].transform.Find("Pm").GetComponent<TMPro.TextMeshPro>().text = _character.PmMax.ToString();
        PopulateStatsList("StatsList" + 0, GenerateStatsListCharacter, null);
    }

    private string GenerateStatsListCharacter(object parameter)
    {
        string statsList = "";
        statsList += MakeTitle("Racial Characteristics");
        statsList += MakeContent("Strong Against: ", _character.StrongAgainst + " +" + RacesData.StrongAgainstDamagePercent + "%");
        statsList += MakeContent("Strong In: ", _character.StrongIn + " +" + RacesData.StrongInDamagePercent + "%");
        statsList += MakeContent("Fav Weapons: ", _character.FavWeapons[0] + ", " + _character.FavWeapons[1]);
        statsList += MakeContent("Leveling Health: ", "+" + _character.LevelingHealthPercent + "%");
        statsList += MakeContent("Leveling Damages: ", "+" + _character.LevelingDamagePercent + "%");
        statsList += MakeContent("Diet: ", _character.Diet.ToString());

        statsList += MakeTitle("Gender Characteristics");
        statsList += MakeContent("Weapons Damages: ", _character.Gender == CharacterGender.Female ? "-" + RacesData.GenderDamage + "%" : "+" + RacesData.GenderDamage + "%");
        statsList += MakeContent("Critical Damages: ", _character.Gender == CharacterGender.Male ? "-" + RacesData.GenderCritical + "%" : "+" + RacesData.GenderCritical + "%");

        statsList += MakeTitle("Weapons Handling");
        statsList += MakeContent("Fav Weapons Damages: ", "+0%");
        statsList += MakeContent("Other Weapons Damages: ", "-" + RacesData.NotRaceWeaponDamagePercent + "%");

        statsList += MakeTitle("Inventory: <material=\"LongWhite\">" + (_character.Inventory?.Count ?? 0) + "/" + _character.InventoryPlace + "</material>");
        for (int i = 0; i < _character.InventoryPlace; ++i)
        {
            if (i >= _character.Inventory.Count)
            {
                statsList += MakeContent((i + 1) + ". ", "Empty\n<material=\"LongGreyish\">     -</material>");
                continue;
            }                
            var item = _character.Inventory[i];
            statsList += MakeContent((i + 1) + ". ", item.GetNameWithColor() + "\n     <material=\"LongGreyish\">" + item.InventoryItemType + " - " + item.Weight + " " + Constants.UnitWeight + "</material>");                
        }
        statsList += MakeTitle("Weight: <material=\"LongWhite\">" + _character.GetTotalWeight() + "/" + _character.WeightLimit + " " + Constants.UnitWeight + "</material>");
        statsList += MakeContent("Weapons: ", _character.GetCurrentWeaponsWeight() + " " + Constants.UnitWeight);
        statsList += MakeContent("Skills: ", _character.GetCurrentSkillsWeight() + " " + Constants.UnitWeight);
        statsList += MakeContent("Inventory: ", _character.GetCurrentInventoryWeight() + " " + Constants.UnitWeight);
        return statsList;
    }

    private void ChangeTab()
    {
        var newTab = int.Parse(Constants.LastEndActionClickedName.Substring(Helper.CharacterAfterString(Constants.LastEndActionClickedName, "Tab")));
        if (newTab == _currentTab)
            return;
        if (newTab == 0)
            _title.text = "Character";
        else
            _title.text = "Equipment";
        _currentTab = newTab;
        for (int i = 0; i < _buttonsTabs.Count; ++i)
        {
            if (i == _currentTab)
            {
                PressButton(_buttonsTabs[i].gameObject);
                BringTabFront(_tabs[i]);
            }
            else
            {
                UnpressButton(_buttonsTabs[i].gameObject);
                BringTabBack(_tabs[i], i);
            }
        }
    }

    private void PressButton(GameObject button)
    {
        var child = button.transform.GetChild(0);
        child.transform.position = button.transform.position;
    }

    private void UnpressButton(GameObject button)
    {
        var child = button.transform.GetChild(0);
        child.transform.position = button.transform.position + new Vector3(0.0f, -Constants.Pixel * 5.0f, 0.0f); 
    }

    private void BringTabFront(GameObject tab)
    {
        tab.transform.position = _currentTabPosition;
    }

    private void BringTabBack(GameObject tab, int id)
    {
        tab.transform.position = _resetTabPosition + new Vector3(0.5f * (float)id, -1.0f * (float)id, 0.0f);
    }

    private void SwitchToInventory()
    {
        Constants.DecreaseInputLayer();
        _instantiator.NewPopupInventory(_character, _sceneUpdateAction);
        Destroy(gameObject);
    }
}
