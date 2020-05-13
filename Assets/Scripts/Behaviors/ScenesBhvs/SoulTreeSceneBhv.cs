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
    }

    private void HandleTreeBranchDisplay(string stat)
    {
        
        var test = _soul.GetPropertyValue(stat + "_Level");
        int statLevel = (int)test;
        int statId = (int)_soul.GetType().GetProperty(stat + "_Id").GetValue(_soul, null);
        int statPrice = (int)_soul.GetType().GetProperty(stat + "_Price").GetValue(_soul, null);
        GameObject treeBranch = null;
        for (int i = 1; i <= statLevel; ++i)
        {
            if (i == 1)
            {
                treeBranch = GameObject.Find(stat);
                treeBranch.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsSoulTree_" + (statId * 2));
            }
            treeBranch.transform.Find("Level" + i.ToString("D2")).GetComponent<SpriteRenderer>().sprite = SoulTreeOnLevel;
        }
        treeBranch.transform.Find("Level").GetComponent<TMPro.TextMeshPro>().text = statLevel.ToString();
        if (_soul.XpKept > statPrice * (statLevel + 1))
        {
            treeBranch.transform.Find("Price").GetComponent<TMPro.TextMeshPro>().enabled = false;
            treeBranch.transform.Find("Price").GetComponent<BoxCollider2D>().enabled = false;
            treeBranch.transform.Find("Add").GetComponent<SpriteRenderer>().enabled = true;
            treeBranch.transform.Find("Add").GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            treeBranch.transform.Find("Price").GetComponent<TMPro.TextMeshPro>().enabled = true;
            treeBranch.transform.Find("Price").GetComponent<BoxCollider2D>().enabled = true;
            treeBranch.transform.Find("Add").GetComponent<SpriteRenderer>().enabled = false;
            treeBranch.transform.Find("Add").GetComponent<BoxCollider2D>().enabled = false;
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
