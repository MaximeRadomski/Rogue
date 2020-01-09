using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBhv : MonoBehaviour
{
    internal virtual void SetPrivates()
    {
        Application.targetFrameRate = 60;
    }
}
