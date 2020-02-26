using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsBhv : MonoBehaviour
{
    private ButtonBhv _button;
    private TMPro.TextMeshPro _textMesh;

    private List<int> _remainingTips;

    void Start()
    {
        _button = GetComponent<ButtonBhv>();
        _textMesh = transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();

        SetRemainingTips();
        _button.BeginActionDelegate = ChangeTip;
        ChangeTip();
    }

    private void SetRemainingTips()
    {
        if (_remainingTips == null)
            _remainingTips = new List<int>();
        for (int i = 0; i < Constants.TipsAndTricks.Length; ++i)
        {
            _remainingTips.Add(i);
        }
        Debug.Log(_remainingTips);
    }

    public void ChangeTip()
    {
        if (_remainingTips.Count == 0)
            SetRemainingTips();
        int id = Random.Range(0, _remainingTips.Count);
        _textMesh.text = Constants.TipsAndTricks[_remainingTips[id]];
        _remainingTips.RemoveAt(id);
        Debug.Log(_remainingTips);
    }
}
