using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public string Name;
    public SkillType Type;
    public Rarity Rarity;
    public CooldownType CooldownType;
    public int CoolDown;
    public int BaseDamage;
    public int BaseSelfDamage;
    public int DamageRangePercentage;
    public int PaNeeded;
    public bool Activated;
    public bool NullifiyNextDamage;
    public bool MoveOnPosition;
    public bool FullRange;
    public List<int> RangePositions;
    public List<RangeDirection> RangeZones;
}
