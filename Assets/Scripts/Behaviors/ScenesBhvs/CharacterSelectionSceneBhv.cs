using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionSceneBhv : SceneBhv
{
    private int _nbCharChoice;
    private List<Character> _choices;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        SetPrivates();
        SetButtons();
    }

    protected override void SetPrivates()
    {
        base.SetPrivates();
        Soul = PlayerPrefsHelper.GetSoul();
        _nbCharChoice = Soul.GetStatCurrentValue(Soul.SoulStats[Soul.NbCharChoice_Id]);
        _choices = new List<Character>();
        for (int i = 0; i < _nbCharChoice; ++i)
        {
            _choices.Add(RacesData.GetCharacterFromRaceAndLevel((CharacterRace)Random.Range(0, Helper.EnumCount<CharacterRace>()),
                                                                Random.Range(1, Soul.GetStatCurrentValue(Soul.SoulStats[Soul.StartingLevel_Id]))));
        }
    }

    private void SetButtons()
    {
        for (int i = 0; i < Soul.NbCharChoice_Max; ++i)
        {
            if (i < _nbCharChoice)
            {

            }
            else
            {

            }
        }
        GameObject.Find("ButtonStart").GetComponent<ButtonBhv>().EndActionDelegate = GoToSwipeScene;
        UpdateButtons();
    }

    private void UpdateButtons()
    {

    }

    private void GoToSwipeScene()
    {

    }
}
