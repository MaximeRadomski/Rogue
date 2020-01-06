using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsBhv : MonoBehaviour
{
    private ButtonBhv _button;
    private TMPro.TextMeshPro _textMesh;
    private int _currentTip;

    void Start()
    {
        _button = GetComponent<ButtonBhv>();
        _textMesh = transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();

        _currentTip = Random.Range(0, Constants.TipsAndTricks.Length);
        _button.BeginActionDelegate = ChangeTip;
        ChangeTip();
    }

    public void ChangeTip()
    {
        ++_currentTip;
        if (_currentTip == Constants.TipsAndTricks.Length)
            _currentTip = 0;
        _textMesh.text = Constants.TipsAndTricks[_currentTip];
    }
}
