using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PopupCharacterStatsBhv : MonoBehaviour
{
    private Character _character;
    private SkinContainerBhv _skinContainerBhv;
    private List<GameObject> _tabs;
    private List<ButtonBhv> _buttonsTabs;
    private Instantiator _instantiator;
    private int _currentTab;
    private Vector3 _resetTabPosition;
    private Vector3 _currentTabPosition;

    public void SetPrivates(Character character)
    {
        _character = character;
        _currentTab = 0;
        _resetTabPosition = new Vector3(-10.0f, 10.0f, 0.0f);
        _currentTabPosition = transform.position;
        _tabs = new List<GameObject>();
        _buttonsTabs = new List<ButtonBhv>();
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
        for (int i = 0; i < 5; ++i)
        {
            _tabs.Add(transform.Find("Tab" + i).gameObject);
            _buttonsTabs.Add(transform.Find("ButtonTab" + i).GetComponent<ButtonBhv>());
            if (i == 0)
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
        SetButtons();
        DisplayStatsCharacter();
        DisplayStatsWeapon(1, _character.Weapons[0]);
        DisplayStatsWeapon(2, _character.Weapons[1]);
        DisplayStatsSkill(3, _character.Skills[0]);
        DisplayStatsSkill(4, _character.Skills[1]);
    }

    private void SetButtons()
    {
        foreach (var button in _buttonsTabs)
        {
            button.EndActionDelegate = ChangeTab;
        }
        transform.Find("ExitButton").GetComponent<ButtonBhv>().EndActionDelegate = ExitPopup;
        _tabs[1].transform.Find("SkinContainerWeapon0").GetComponent<ButtonBhv>().EndActionDelegate = RandomizeWeapon;
        _tabs[2].transform.Find("SkinContainerWeapon1").GetComponent<ButtonBhv>().EndActionDelegate = RandomizeWeapon;
    }

    private void RandomizeWeapon()
    {
        var idWeapon = Constants.LastEndActionClickedName.Contains("0") ? 0 : 1;
        var weapon = _character.Weapons[idWeapon];
        weapon.WeaponParts = WeaponsData.CreateWeaponPartsFromTypeSubType(weapon.Type, Random.Range(0, WeaponsData.WeaponTypeNames[weapon.Type.GetHashCode()].Length), weapon.NbSkinParts);
        _instantiator.LoadWeaponSkin(weapon, _tabs[idWeapon + 1].transform.Find("SkinContainerWeapon" + idWeapon).gameObject);
    }

    private void PopulateStatsList(string name, System.Func<object, string> generator, object parameter)
    {
        var statsList = GameObject.Find(name);
        var statsListText = statsList.GetComponent<TMPro.TextMeshProUGUI>();
        statsListText.text = generator(parameter);
        var textHeight = statsListText.preferredHeight;
        var parent = statsList.transform.parent.GetComponent<UnityEngine.UI.ScrollRect>();
        var parentSizeY = parent.GetComponent<RectTransform>().sizeDelta.y;
        if (textHeight < parentSizeY)
            textHeight = parentSizeY;
        statsList.GetComponent<RectTransform>().sizeDelta += new Vector2(0.0f, textHeight);        
        parent.normalizedPosition = new Vector2(0.0f, 1.0f);
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

        statsList += MakeTitle("Gender Characteristics");
        statsList += MakeContent("Weapons Damages: ", _character.Gender == CharacterGender.Female ? "-" + RacesData.GenderDamage + "%" : "+" + RacesData.GenderDamage + "%");
        statsList += MakeContent("Critical Damages: ", _character.Gender == CharacterGender.Male ? "-" + RacesData.GenderCritical + "%" : "+" + RacesData.GenderCritical + "%");

        statsList += MakeTitle("Weapons Handling");
        statsList += MakeContent("Fav Weapons Damages: ", "+0%");
        statsList += MakeContent("Other Weapons Damages: ", "-" + RacesData.NotRaceWeaponDamagePercent + "%");

        statsList += MakeTitle("Inventory: <material=\"LongWhite\">" + _character.GetCurrentInventoryWeight() + "/" + _character.WeightLimit + " " + Constants.UnitWeight + "</material>");
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
        return statsList;
    }

    private void DisplayStatsWeapon(int tabId, Weapon weapon)
    {
        _instantiator.LoadWeaponSkin(weapon, _tabs[tabId].transform.Find("SkinContainerWeapon" + (tabId - 1)).gameObject);        
        _tabs[tabId].transform.Find("Name").GetComponent<TMPro.TextMeshPro>().text = weapon.GetNameWithColor();
        _tabs[tabId].transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsWeapon_" + weapon.Type.GetHashCode());
        _tabs[tabId].transform.Find("Damages").GetComponent<TMPro.TextMeshPro>().text = weapon.BaseDamage.ToString();
        _tabs[tabId].transform.Find("Pa").GetComponent<TMPro.TextMeshPro>().text = weapon.PaNeeded.ToString();
        _tabs[tabId].transform.Find("Range").GetComponent<TMPro.TextMeshPro>().text = weapon.MinRange + (weapon.MaxRange != weapon.MinRange ? "-" + weapon.MaxRange : "");
        PopulateStatsList("StatsList" + tabId, GenerateStatsListWeapon, weapon);
    }

    private string GenerateStatsListWeapon(object parameter)
    {
        var weapon = (Weapon)parameter;
        string statsList = "";
        statsList += MakeTitle(weapon.Type.GetDescription() + " Characteristics");
        statsList += MakeContent("Damage Range: ", "+/- " + weapon.DamageRangePercentage + "%");
        statsList += MakeContent("Critical Chance: ", weapon.CritChancePercent + "%");
        statsList += MakeContent("Critical Multiplier: ", "+" + weapon.CritMultiplierPercent + "%");
        statsList += MakeContent("Weight: ", weapon.Weight + " " + Constants.UnitWeight);

        if (weapon.Specificity != null && weapon.Specificity != string.Empty)
        {
            statsList += MakeTitle(weapon.Type.GetDescription() + " Specificity");
            statsList += MakeContent("", weapon.Specificity);
        }

        if (weapon.RangeZones != null && weapon.RangeZones.Count > 0)
        {
            statsList += MakeTitle(weapon.Type.GetDescription() + " Area Of Effect");
            var zones = "";
            foreach (var zone in weapon.RangeZones)
                zones += (zones != "" ? ", " : "") + zone.GetDescription();
            statsList += MakeContent("Zones: ", zones);
        }
        return statsList;
    }

    private void DisplayStatsSkill(int tabId, Skill skill)
    {
        _instantiator.LoadSkillSkin(skill, _tabs[tabId].transform.Find("SkinContainerSkill" + (tabId - 3)).gameObject);
        _tabs[tabId].transform.Find("Name").GetComponent<TMPro.TextMeshPro>().text = skill.Name;
        _tabs[tabId].transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsSkill_" + skill.IconId);
        _tabs[tabId].transform.Find("Cooldown").GetComponent<TMPro.TextMeshPro>().text = skill.CooldownType == CooldownType.Normal ? skill.CooldownMax.ToString() : "-";
        _tabs[tabId].transform.Find("Pa").GetComponent<TMPro.TextMeshPro>().text = skill.PaNeeded.ToString();
        _tabs[tabId].transform.Find("Range").GetComponent<TMPro.TextMeshPro>().text = skill.MinRange + (skill.MaxRange != skill.MinRange ? "-" + skill.MaxRange : "");
        PopulateStatsList("StatsList" + tabId, GenerateStatsListSkill, skill);
    }

    private string GenerateStatsListSkill(object parameter)
    {
        var skill = (Skill)parameter;
        string statsList = "";
        statsList += MakeTitle("Description");
        statsList += MakeContent("", skill.Description);

        statsList += MakeTitle("Skill Characteristics");
        statsList += MakeContent("Weight: ", skill.Weight + " " + Constants.UnitWeight);
        return statsList;
    }

    private string MakeTitle(string title)
    {
        return "<align=\"center\"><material=\"LongYellow\">" + title + "</material></align>\n";
    }

    private string MakeContent(string libelle, string content)
    {
        return "<material=\"LongGreyish\">" + libelle + "</material>" + content + "\n";
    }

    private void ChangeTab()
    {
        var newTab = int.Parse(Constants.LastEndActionClickedName.Substring(Helper.CharacterAfterString(Constants.LastEndActionClickedName, "Tab")));
        if (newTab == _currentTab)
            return;
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

    public void ExitPopup()
    {
        Constants.DecreaseInputLayer();
        Destroy(gameObject);
    }
}
