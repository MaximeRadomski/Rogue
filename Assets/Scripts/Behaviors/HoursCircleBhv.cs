using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoursCircleBhv : MonoBehaviour
{
    private Vector3 _targetRotation = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3? _targetRotation2 = null;
    private List<TMPro.TextMeshPro> _hours;

    private void SetPrivates()
    {
        _hours = new List<TMPro.TextMeshPro>();
        for (int i = 1; i <= 12; ++i)
        {
            _hours.Add(transform.Find("Hours"+i).GetComponent<TMPro.TextMeshPro>());
        }
    }

    public void Rotate(Vector3 targetRotation)
    {
        if (targetRotation.z > transform.eulerAngles.z)
            _targetRotation = targetRotation;
        else
        {
            _targetRotation = new Vector3(0.0f, 0.0f, 359.9f);
            _targetRotation2 = targetRotation;
        }
        if (_hours == null)
            SetPrivates();
    }

    void Update()
    {
        if (transform.eulerAngles.z != _targetRotation.z)
        {
            if (_hours == null)
                SetPrivates();
            if (_targetRotation2 == null)
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, _targetRotation, 0.1f);
            else
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, _targetRotation, 0.2f);
            for (int i = 0; i < 12; ++i)
            {
                if (_hours[i].transform.position.y > transform.position.y - 0.1f)
                {
                    _hours[i].gameObject.SetActive(true);
                    var difference = 0.67f;
                    var tmpColor = _hours[i].GetComponent<TMPro.TextMeshPro>().color;
                    var range = (_hours[i].transform.position.y - transform.position.y);
                    var transparency = range / difference;
                    _hours[i].GetComponent<TMPro.TextMeshPro>().color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, transparency);
                }
                else
                    _hours[i].gameObject.SetActive(false);
            }
        }
        if (transform.eulerAngles.z >= 359.0f)
        {
            transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            _targetRotation = _targetRotation2 ?? transform.eulerAngles;
            _targetRotation2 = null;

        }
    }
}
