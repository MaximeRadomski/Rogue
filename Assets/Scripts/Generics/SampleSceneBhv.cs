using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        SetSampleButton("ButtonTopRight");
        SetSampleButton("ButtonTopMid");
        SetSampleButton("ButtonFloatingBotLeft");
    }

    private void SetSampleButton(string name)
    {
        var tmpButtonBhv = GameObject.Find(name).GetComponent<ButtonBhv>();
        tmpButtonBhv.BeginAction = BeginAction;
        tmpButtonBhv.DoAction = DoAction;
        tmpButtonBhv.EndAction = EndAction;
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
}
