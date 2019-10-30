using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string Name;
    public WeaponType Type;
    public WeaponRarity Rarity;
    public int BaseDamage;
    public int DamageRangePercentage;
    public int CritChancePercent;
    public int CritMultiplierPercent;
    public int FailChancePercent;
    public int PaNeeded;
    public int MinRange;
    public int MaxRange;
    public List<RangePos> RangePositions;
    public List<RangeDirection> RangeZones;
}
