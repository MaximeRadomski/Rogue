﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoulTreeSceneBhv : SceneBhv
{
    public Sprite SoulTreeOnLevel;

    private Soul _soul;

    void Start()
    {
        SetPrivates();
        SetButtons();
        UpdateTreeDisplay();
    }

    protected override void SetPrivates()
    {
        base.SetPrivates();
        _soul = PlayerPrefsHelper.GetSoul();
        // DEBUG //
        _soul.RunAwayPercent_Level = 5;
        _soul.LootPercent_Level = 4;
        _soul.CritChance_Level = 3;
        _soul.InvPlace_Level = 2;
        _soul.InvWeight_Level = 1;
        _soul.StartingLevel_Level = 1;
        _soul.Xp = 894; // membres, parfois ça varie
        // DEBUG //
    }

    private void SetButtons()
    {
        int nbSoulStats = Soul.SoulStats.Length;
        GameObject.Find("ButtonStart").GetComponent<ButtonBhv>().EndActionDelegate = GoToNextScene;
        for (int i = 0; i < nbSoulStats; ++i)
        {
            var stat = Soul.SoulStats[i];
            GameObject treeBranch = GameObject.Find(stat);
            var addButton = treeBranch.transform.Find("Add");
            addButton.GetComponent<ButtonBhv>().EndActionDelegate = AddAction;
            addButton.name = "Add" + stat;
        }
    }

    private void UpdateTreeDisplay()
    {
        int nbSoulStats = Soul.SoulStats.Length;
        for (int i = 0; i < nbSoulStats; ++i)
        {
            var stat = Soul.SoulStats[i];
            HandleTreeBranchDisplay(stat);
        }
        GameObject.Find("XpGained").GetComponent<TMPro.TextMeshPro>().text = _soul.Xp.ToString();
    }

    private void HandleTreeBranchDisplay(string stat)
    {
        int statLevel = (int)_soul.GetFieldValue(stat + "_Level");
        int statMax = (int)_soul.GetFieldValue(stat + "_Max");
        int statId = (int)_soul.GetFieldValue(stat + "_Id");
        int statPrice = (int)_soul.GetFieldValue(stat + "_Price");
        GameObject treeBranch = GameObject.Find(stat);
        for (int i = 1; i <= statLevel; ++i)
        {
            if (i == 1)
                treeBranch.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsSoulTree_" + (statId * 2));
            treeBranch.transform.Find("Level" + i.ToString("D2")).GetComponent<SpriteRenderer>().sprite = SoulTreeOnLevel;
        }
        treeBranch.transform.Find("Level").GetComponent<TMPro.TextMeshPro>().text = statLevel.ToString();
        var currentPrice = statPrice * (statLevel + 1);
        var priceTextObject = treeBranch.transform.Find("Price").GetComponent<TMPro.TextMeshPro>();
        var addSpriteObject = treeBranch.transform.Find("Add" + stat).GetComponent<SpriteRenderer>();
        if (statLevel == statMax)
        {
            priceTextObject.enabled = true;
            priceTextObject.text = "Max";
            addSpriteObject.enabled = false;
        }
        else if (_soul.Xp > currentPrice)
        {
            priceTextObject.enabled = false;
            addSpriteObject.enabled = true;
        }
        else
        {
            priceTextObject.enabled = true;
            priceTextObject.text = "<material=\"LongOrange\">" + currentPrice.ToString() + "</material>";
            addSpriteObject.enabled = false;
        }
    }

    private void AddAction()
    {
        var stat = Constants.LastEndActionClickedName.Substring(Helper.CharacterAfterString(Constants.LastEndActionClickedName, "Add"));
        int statId = (int)_soul.GetFieldValue(stat + "_Id");
        int statLevel = (int)_soul.GetFieldValue(stat + "_Level");
        int statMax = (int)_soul.GetFieldValue(stat + "_Max");
        int statAdd = (int)_soul.GetFieldValue(stat + "_Add");
        int statPrice = (int)_soul.GetFieldValue(stat + "_Price");
        string statName = Soul.SoulStatsNames[statId];
        string statDescription = Soul.SoulStatsDescriptions[statId];
        string statUnit = Soul.SoulStatsUnit[statId];
        var fullTitle = statName + (statLevel > 0 ? "  " + statLevel.ToString() : string.Empty);
        var currentDescription = statDescription + MakeContent("Current:", " +" + (statAdd * statLevel) + " " + statUnit);
        var currentPrice = statPrice * (statLevel + 1);
        var negative = "Cancel";
        var positive = "<material=\"LongOrange\">" + currentPrice.ToString() + "</material>";
        if (statLevel == statMax || _soul.Xp < currentPrice)
        {
            negative = null;
            positive = "Back";
        }
        var nextDescription = string.Empty;
        if (statLevel != statMax)
            nextDescription += MakeContent("Next:", " +" + (statAdd * (statLevel + 1)) + " " + statUnit);
        //Plural
        if (statUnit.Length > 3 && statUnit[0] != '<') //Check '<' because of custom materials
        {
            if (statAdd * statLevel > 1)
                currentDescription += "s";
            if (statAdd * (statLevel + 1) > 1 && nextDescription != string.Empty)
                nextDescription += "s";
        }
        Instantiator.NewPopupYesNo(fullTitle, currentDescription + nextDescription, negative, positive, AfterAddAction);

        object AfterAddAction(bool result)
        {
            if (result == false || statLevel == statMax || _soul.Xp < currentPrice)
                return result;
            _soul.Xp -= currentPrice;
            var levelFieldInfo = _soul.GetType().GetField(stat + "_Level");
            levelFieldInfo.SetValue(_soul, statLevel + 1);
            UpdateTreeDisplay();
            return result;
        }
    }

    public void GoToNextScene()
    {
        //_soul = PlayerPrefsHelper.GetSoul();
        PlayerPrefsHelper.SaveSoul(_soul);
        //DEBUG CREATION
        //Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "YOUR NEW BODY AWAITS", 2.0f, OnToRaceChoiceScene);
        //DEBUG SELECTION
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "YOUR NEW BODY AWAITS", 2.0f, OnToCharacterSelectionScene);
    }

    protected string MakeContent(string libelle, string content)
    {
        return "\n<material=\"LongGreyish\">" + libelle + "</material><material=\"LongWhite\">" + content + "</material>";
    }

    public object OnToRaceChoiceScene(bool result)
    {
        NavigationService.LoadNextScene(Constants.RaceChoiceScene);
        return result;
    }

    public object OnToCharacterSelectionScene(bool result)
    {
        NavigationService.LoadNextScene(Constants.CharacterSelectionScene);
        return result;
    }
}
