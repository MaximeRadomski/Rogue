using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum RangeDirection
{
    Up,
    Left,
    Right,
    [Description("Diagonal Left")]
    DiagonalLeft,
    [Description("Diagonal Right")]
    DiagonalRight
}
