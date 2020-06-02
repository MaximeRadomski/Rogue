using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSceneBhv : SceneBhv
{
    private float _soulStatOriginX = -1.25f;
    private float _soulStatOriginY = 2.25f;
    private float _soulStatWidth = 1.25f;
    private float _soulStatHeight = 1.0f;

    void Start()
    {
        SetPrivates();
        SetButtons();
    }

    protected override void SetPrivates()
    {
        base.SetPrivates();
        OnRootPreviousScene = Constants.SwipeScene;
        Soul = PlayerPrefsHelper.GetSoul();
    }

    private void SetButtons()
    {
        int nbSoulStats = Soul.SoulStats.Length;
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPreviousScene;
        int nbStats = 0;
        for (int i = 0; i < nbSoulStats; ++i)
        {
            var stat = Soul.SoulStats[i];
            int statLevel = (int)Soul.GetFieldValue(stat + "_Level");
            if (statLevel <= 0)
                continue;
            int statId = (int)Soul.GetFieldValue(stat + "_Id");
            int statAdd = (int)Soul.GetFieldValue(stat + "_Add");
            string statUnit = Soul.SoulStatsUnit[statId];
            var desc = "+" + (statAdd * statLevel) + " " + statUnit;
            //Plural
            if (statUnit.Length > 3 && statUnit[0] != '<') //Check '<' because of custom materials
            {
                if (statAdd * statLevel > 1)
                    desc += "s";
            }
            var soulStat = Instantiator.NewSoulStat(
                new Vector3(_soulStatOriginX + (nbStats % 3) * _soulStatWidth,
                            _soulStatOriginY - (nbStats / 3) * _soulStatHeight,
                            0.0f), statLevel, statId, desc);
            soulStat.GetComponent<ButtonBhv>().EndActionDelegate = DisplayStat;
            soulStat.name = stat;
            ++nbStats;
        }
    }

    private void DisplayStat()
    {
        var stat = Constants.LastEndActionClickedName;
        int statId = (int)Soul.GetFieldValue(stat + "_Id");
        int statLevel = (int)Soul.GetFieldValue(stat + "_Level");
        int statAdd = (int)Soul.GetFieldValue(stat + "_Add");
        string statName = Soul.SoulStatsNames[statId];
        string statDescription = Soul.SoulStatsDescriptions[statId];
        string statUnit = Soul.SoulStatsUnit[statId];
        var fullTitle = statName + (statLevel > 0 ? "  " + statLevel.ToString() : string.Empty);
        var currentDescription = statDescription + MakeContent("Current:", " +" + (statAdd * statLevel) + " " + statUnit);
        //Plural
        if (statUnit.Length > 3 && statUnit[0] != '<') //Check '<' because of custom materials
        {
            if (statAdd * statLevel > 1)
                currentDescription += "s";
        }
        Instantiator.NewPopupYesNo(fullTitle, currentDescription, null, "Ok", AfterDisplayStat);

        object AfterDisplayStat(bool result)
        {
            return result;
        }
    }

    protected string MakeContent(string libelle, string content)
    {
        return "\n<material=\"LongGreyish\">" + libelle + "</material><material=\"LongWhite\">" + content + "</material>";
    }

    private void GoToPreviousScene()
    {
        NavigationService.OverBlendPreviousScene(OnRootPreviousScene);
    }
}
