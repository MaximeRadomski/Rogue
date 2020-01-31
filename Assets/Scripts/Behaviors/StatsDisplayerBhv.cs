using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsDisplayerBhv : MonoBehaviour
{
    protected Instantiator _instantiator;

    protected void PopulateStatsList(string name, System.Func<object, string> generator, object parameter)
    {
        var statsList = GameObject.Find(name);
        var statsListText = statsList.GetComponent<TMPro.TextMeshProUGUI>();
        statsListText.text = generator(parameter);
        var textHeight = statsListText.preferredHeight;
        var parent = statsList.transform.parent.GetComponent<UnityEngine.UI.ScrollRect>();
        var parentSizeY = parent.GetComponent<RectTransform>().sizeDelta.y;
        if (textHeight < parentSizeY)
            textHeight = parentSizeY;
        statsList.GetComponent<RectTransform>().sizeDelta = new Vector2(statsList.GetComponent<RectTransform>().sizeDelta.x, textHeight);
        parent.normalizedPosition = new Vector2(0.0f, 1.0f);
    }

    protected void DisplayStatsWeapon(GameObject container, Weapon weapon, string skinContainerName, string statsListName, bool displayList = true)
    {
        _instantiator.LoadWeaponSkin(weapon, container.transform.Find(skinContainerName).gameObject);
        container.transform.Find("Name").GetComponent<TMPro.TextMeshPro>().text = weapon.GetNameWithColor();
        container.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsWeapon_" + weapon.Type.GetHashCode());
        container.transform.Find("Damages").GetComponent<TMPro.TextMeshPro>().text = weapon.BaseDamage.ToString();
        container.transform.Find("Pa").GetComponent<TMPro.TextMeshPro>().text = weapon.PaNeeded.ToString();
        container.transform.Find("Range").GetComponent<TMPro.TextMeshPro>().text = weapon.MinRange + (weapon.MaxRange != weapon.MinRange ? "-" + weapon.MaxRange : "");
        if (displayList)
            PopulateStatsList(statsListName, GenerateStatsListWeapon, weapon);
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

    protected void DisplayStatsSkill(GameObject container, Skill skill, string skinContainerName, string statsListName, bool displayList = true)
    {
        _instantiator.LoadSkillSkin(skill, container.transform.Find(skinContainerName).gameObject);
        container.transform.Find("Name").GetComponent<TMPro.TextMeshPro>().text = skill.GetNameWithColor();
        container.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsSkill_" + skill.IconId);
        container.transform.Find("Cooldown").GetComponent<TMPro.TextMeshPro>().text = skill.CooldownType == CooldownType.Normal ? skill.CooldownMax.ToString() : "-";
        container.transform.Find("Pa").GetComponent<TMPro.TextMeshPro>().text = skill.PaNeeded.ToString();
        container.transform.Find("Range").GetComponent<TMPro.TextMeshPro>().text = skill.MinRange + (skill.MaxRange != skill.MinRange ? "-" + skill.MaxRange : "");
        if (displayList)
            PopulateStatsList(statsListName, GenerateStatsListSkill, skill);
    }

    private string GenerateStatsListSkill(object parameter)
    {
        var skill = (Skill)parameter;
        string statsList = "";
        statsList += MakeTitle("Description");
        statsList += MakeContent("", skill.Description);

        bool hasSpecificityTitle = false;

        if (skill.Type == SkillType.Racial)
        {
            if (!hasSpecificityTitle)
            {
                statsList += MakeTitle("Specificity");
                hasSpecificityTitle = true;
            }
            statsList += MakeContent("Racial: ", skill.Race + "s only");
        }

        if (skill.CooldownType != CooldownType.Normal)
        {
            if (!hasSpecificityTitle)
            {
                statsList += MakeTitle("Specificity");
                hasSpecificityTitle = true;
            }
            if (skill.CooldownType == CooldownType.OnceAFight)
                statsList += MakeContent("", "This Skill can only be used once a fight");
            else
                statsList += MakeContent("", "This Skill is a passive effect");
        }

        statsList += MakeTitle("Skill Characteristics");
        statsList += MakeContent("Weight: ", skill.Weight + " " + Constants.UnitWeight);
        return statsList;
    }

    protected void DisplayStatsConsumable(GameObject container, Consumable consumable, string skinContainerName, string statsListName)
    {
        _instantiator.LoadConsumableSkin(consumable, container.transform.Find(skinContainerName).gameObject);
        container.transform.Find("Name").GetComponent<TMPro.TextMeshPro>().text = consumable.GetNameWithColor();
        container.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsConsumable_" + consumable.IconId);
        container.transform.Find("Cooldown").GetComponent<TMPro.TextMeshPro>().text = Helper.TimeFromMinutes(consumable.MinutesNeeded);
        PopulateStatsList(statsListName, GenerateStatsListConsumable, consumable);
    }

    private string GenerateStatsListConsumable(object parameter)
    {
        var consumable = (Consumable)parameter;
        string statsList = "";
        statsList += MakeTitle("Description");
        statsList += MakeContent("", consumable.Description);
        statsList += MakeContent(consumable.Story, "");

        statsList += MakeTitle("Consumable Characteristics");
        statsList += MakeContent("Weight: ", consumable.Weight + " " + Constants.UnitWeight);
        return statsList;
    }

    protected string MakeTitle(string title)
    {
        return "<align=\"center\"><material=\"LongYellow\">" + title + "</material></align>\n";
    }

    protected string MakeContent(string libelle, string content)
    {
        return "<material=\"LongGreyish\">" + libelle + "</material><material=\"LongWhite\">" + content + "</material>\n";
    }
}
