using System.Collections;
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
        _soul.Xp = 894; // membres, parfois ça varie
        // DEBUG //
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonStart").GetComponent<ButtonBhv>().EndActionDelegate = GoToRaceChoiceScene;
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
        int statLevel = (int)_soul.GetPropertyValue(stat + "_Level");
        int statMax = (int)_soul.GetPropertyValue(stat + "_Max");
        int statId = (int)_soul.GetPropertyValue(stat + "_Id");
        int statPrice = (int)_soul.GetPropertyValue(stat + "_Price");
        GameObject treeBranch = GameObject.Find(stat);
        for (int i = 1; i <= statLevel; ++i)
        {
            if (i == 1)
                treeBranch.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsSoulTree_" + (statId * 2));
            treeBranch.transform.Find("Level" + i.ToString("D2")).GetComponent<SpriteRenderer>().sprite = SoulTreeOnLevel;
        }
        treeBranch.transform.Find("Level").GetComponent<TMPro.TextMeshPro>().text = statLevel.ToString();
        var currentPrice = statPrice * (statLevel + 1);
        if (statLevel == statMax)
        {
            var priceObject = treeBranch.transform.Find("Price").GetComponent<TMPro.TextMeshPro>();
            priceObject.enabled = true;
            priceObject.text = "Max";
            treeBranch.transform.Find("Add").GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (_soul.Xp > currentPrice)
        {
            treeBranch.transform.Find("Price").GetComponent<TMPro.TextMeshPro>().enabled = false;
            treeBranch.transform.Find("Add").GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            var priceObject = treeBranch.transform.Find("Price").GetComponent<TMPro.TextMeshPro>();
            priceObject.enabled = true;
            priceObject.text = currentPrice.ToString();
            treeBranch.transform.Find("Add").GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void GoToRaceChoiceScene()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "YOUR NEW BODY AWAITS", 2.0f, OnToRaceChoiceScene);
    }

    public object OnToRaceChoiceScene(bool result)
    {
        NavigationService.LoadNextScene(Constants.RaceChoiceScene);
        return result;
    }
}
