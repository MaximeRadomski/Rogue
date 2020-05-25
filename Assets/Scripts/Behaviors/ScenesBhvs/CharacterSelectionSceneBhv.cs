using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionSceneBhv : SceneBhv
{
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        SetPrivates();
        SetButtons();
    }

    protected override void SetPrivates()
    {
        base.SetPrivates();
    }

    private void SetButtons()
    {
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
