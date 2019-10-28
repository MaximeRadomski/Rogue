﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleSceneBhv : MonoBehaviour
{
    private UnityEngine.UI.Text _sampleText;

    void Start()
    {
        SetPrivates();
        SetButtons();
    }

    private void SetPrivates()
    {
        _sampleText = GameObject.Find("SampleText").GetComponent<UnityEngine.UI.Text>();
    }

    private void SetButtons()
    {
        SetSampleButton("ButtonBotMid");
        SetSampleButton("ButtonBotLeft");
        SetSampleButton("ButtonBotRight");
        SetSampleButton("ButtonFloatingTopRight");
        SetSampleButton("ButtonTopMid");
        GameObject.Find("ButtonTopRight").GetComponent<ButtonBhv>().EndActionDelegate = GoToSampleGridScene;
        GameObject.Find("ButtonFloatingDislike").GetComponent<ButtonBhv>().EndActionDelegate = GameObject.Find("TemplateCard").GetComponent<GrabbableCardBhv>().Dislike;
        GameObject.Find("ButtonFloatingLike").GetComponent<ButtonBhv>().EndActionDelegate = GameObject.Find("TemplateCard").GetComponent<GrabbableCardBhv>().Like;
    }

    private void SetSampleButton(string name)
    {
        var tmpButtonBhv = GameObject.Find(name).GetComponent<ButtonBhv>();
        tmpButtonBhv.BeginActionDelegate = BeginAction;
        tmpButtonBhv.DoActionDelegate = DoAction;
        tmpButtonBhv.EndActionDelegate = EndAction;
    }

    public void BeginAction()
    {
        _sampleText.text = "Start\n";
    }

    public void DoAction()
    {
        _sampleText.text += "|";
    }

    public void EndAction()
    {
        _sampleText.text += "\nEnd";
    }

    public void GoToSampleGridScene()
    {
        SceneManager.LoadScene(Constants.SampleGridScene);
    }
}