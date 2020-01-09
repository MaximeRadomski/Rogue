﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputBhv : MonoBehaviour
{
    public int Layer = 0;

    public abstract void BeginAction(Vector2 initialTouchPosition);
    public abstract void DoAction(Vector2 touchPosition);
    public abstract void EndAction(Vector2 lastTouchPosition);
    public abstract void CancelAction();
}