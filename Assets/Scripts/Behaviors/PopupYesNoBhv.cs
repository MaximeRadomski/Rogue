﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupYesNoBhv : MonoBehaviour
{
    private System.Func<bool, object> _resultAction;

    public void SetPrivates(string title, string content, string negative, string positive,
        System.Func<bool, object> resultAction)
    {
        transform.position = Camera.main.transform.position;
        transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = title;
        transform.Find("Content").GetComponent<TMPro.TextMeshPro>().text = content;
        _resultAction = resultAction;

        var buttonPositive = transform.Find("ButtonPositive");
        buttonPositive.GetComponent<ButtonBhv>().EndActionDelegate = PositiveDelegate;
        buttonPositive.transform.Find("ButtonPositiveText").GetComponent<TMPro.TextMeshPro>().text = positive;

        var buttonNegative = transform.Find("ButtonNegative");
        buttonNegative.GetComponent<ButtonBhv>().EndActionDelegate = NegativeDelegate;
        buttonNegative.transform.Find("ButtonNegativeText").GetComponent<TMPro.TextMeshPro>().text = negative;
    }

    private void PositiveDelegate()
    {
        Constants.DecreaseInputLayer();
        _resultAction(true);
        Destroy(gameObject);
    }

    private void NegativeDelegate()
    {
        Constants.DecreaseInputLayer();
        _resultAction(false);
        Destroy(gameObject);
    }
}
