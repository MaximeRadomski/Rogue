using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string Name;
    public WeaponType Type;
    public WeaponRarity Rarity;
    public WeaponRangeType RangeType;
    public int MinRange;
    public int MaxRange;
    public List<RangePos> RangePositions;
    public int BaseDamage;
    public int DamageRangePercentage;
    public int CritChance;
    public int FailChance;

    public enum WeaponType
    {
        Sword = 0,
        Spear = 1,
        Club = 2,
        Knife = 3,
        Bow = 4,
        Dagger = 5,
        Hammer = 6,
        Axe = 7,
        LongSword = 8,
        Gauntet = 9
    }

    public enum WeaponRarity
    {
        Normal = 0,
        Magical = 1,
        Rare = 2,
        Set = 3,
        Legendary = 4
    }

    public enum WeaponRangeType
    {
        Line,
        Diagonal,
        Circle
    }

    public class RangePos
    {
        public int x;
        public int y;
    }
}
