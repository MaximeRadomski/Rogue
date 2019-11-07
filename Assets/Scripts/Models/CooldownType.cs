using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum CooldownType
{
    Normal = 0,
    [Description("Once a fight")]
    OnceAFight = 1,
    Passive = 4
}
