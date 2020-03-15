using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : InventoryItem
{
    public WeaponType Type;
    public int BaseDamage;
    public int DamageRangePercentage;
    public int CritChancePercent;
    public int CritMultiplierPercent;
    public int FailChancePercent;
    public int PaNeeded;
    public int MinRange;
    public int MaxRange;
    public string SpecificityTitle;
    public string Specificity;
    public List<int> RangePositions;
    public List<RangeDirection> RangeZones;

    public bool UnequipLock;
    public int AmountSharpened;

    public int NbSkinParts;
    public List<string> WeaponParts;
    public int EffectId;

    public Weapon()
    {
        InventoryItemType = InventoryItemType.Weapon;
    }
}
