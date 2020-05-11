using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoulTreeSceneBhv : SceneBhv
{
    void Start()
    {
        SetPrivates();
        SetButtons();
    }

    protected override void SetPrivates()
    {
        base.SetPrivates();
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonStart").GetComponent<ButtonBhv>().EndActionDelegate = GoToRaceChoiceScene;
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
