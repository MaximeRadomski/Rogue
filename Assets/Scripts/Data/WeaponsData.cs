using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponsData
{
    //  WEAPONS //
    public static int BaseInitDamageRangePercentage = 15;
    public static string[] NormalNames = { "", "Classic", "Usual", "Unoriginal", "Average", "Child", "Weird", "Redneck" };
    public static string[] MagicalNames = { "Magical", "Enchanted", "Lost", "Knight", "Your mom's", "Mythical", "Ok" };
    public static string[] RareNames = { "|Of Doom", "Enormous", "Super", "|2.0", "|of Death", "|of Truth", "|of Fertility", "El Famoso" };

    //  SWORD  //
    public static int[] SwordDamage = { 100, 150, 220, 300 };
    public static Weapon BaseSword = new Weapon()
    {
        Type = WeaponType.Sword,
        DamageRangePercentage = 10,
        UsePerTurn = 1,
        CritChancePercent = 5,
        CritMultiplierPercent = 120,
        MinRange = 1,
        MaxRange = 1,
        RangePositions = { new RangePos(0, 1), new RangePos(1, 0), new RangePos(0, -1), new RangePos(-1, 0) },
        RangeZones = null
    };

    //  SPEAR  //
    public static int[] SpearDamage = { 120, 180, 260, 350 };
    public static Weapon BaseSpear = new Weapon()
    {
        Type = WeaponType.Spear,
        DamageRangePercentage = 5,
        UsePerTurn = 1,
        CritChancePercent = 1,
        CritMultiplierPercent = 200,
        MinRange = 2,
        MaxRange = 2,
        RangePositions = { new RangePos(0, 2), new RangePos(2, 0), new RangePos(0, -2), new RangePos(-2, 0) },
        RangeZones = null
    };

    //  CLUB  //
    public static int[] ClubDamage = { 75, 100, 150, 225 };
    public static Weapon BaseClub = new Weapon()
    {
        Type = WeaponType.Club,
        DamageRangePercentage = 15,
        UsePerTurn = 1,
        CritChancePercent = 10,
        CritMultiplierPercent = 111,
        MinRange = 1,
        MaxRange = 1,
        RangePositions = { new RangePos(0, 1), new RangePos(1, 0), new RangePos(0, -1), new RangePos(-1, 0) },
        RangeZones = { RangeDirection.Up }
    };

    //  KNIFE  //
    public static int[] KnifeDamage = { 30, 40, 65, 100 };
    public static Weapon BaseKnife = new Weapon()
    {
        Type = WeaponType.Knife,
        DamageRangePercentage = 50,
        UsePerTurn = 2,
        CritChancePercent = 33,
        CritMultiplierPercent = 170,
        MinRange = 2,
        MaxRange = 2,
        RangePositions = { new RangePos(-1, 1), new RangePos(1, 1), new RangePos(1, -1), new RangePos(-1, -1) },
        RangeZones = null
    };

    //  BOW  //
    public static int[] BowDamage = { 60, 100, 140, 180 };
    public static Weapon BaseBow = new Weapon()
    {
        Type = WeaponType.Bow,
        DamageRangePercentage = 5,
        UsePerTurn = 1,
        CritChancePercent = 1,
        CritMultiplierPercent = 200,
        MinRange = 3,
        MaxRange = 5,
        RangePositions = { new RangePos(0, 3), new RangePos(0, 4), new RangePos(0, 5),
                           new RangePos(3, 0), new RangePos(4, 0), new RangePos(5, 0),
                           new RangePos(0, -3), new RangePos(0, -4), new RangePos(0, -5),
                           new RangePos(-3, 0), new RangePos(-4, 0), new RangePos(-5, 0)},
        RangeZones = null
    };

    //  DAGGERS  //
    public static int[] DaggerSDamage = { 20, 40, 80, 180 };
    public static Weapon BaseDaggerS = new Weapon()
    {
        Type = WeaponType.Daggers,
        DamageRangePercentage = 10,
        UsePerTurn = 3,
        CritChancePercent = 50,
        CritMultiplierPercent = 130,
        MinRange = 1,
        MaxRange = 1,
        RangePositions = { new RangePos(-1, 1), new RangePos(1, 1), new RangePos(1, -1), new RangePos(-1, -1) },
        RangeZones = null
    };

    //  HAMMER  //
    public static int[] HammerDamage = { 90, 130, 190, 260 };
    public static Weapon BaseHammer = new Weapon()
    {
        Type = WeaponType.Hammer,
        DamageRangePercentage = 0,
        UsePerTurn = 1,
        CritChancePercent = 15,
        CritMultiplierPercent = 120,
        MinRange = 1,
        MaxRange = 1,
        RangePositions = { new RangePos(0, 1), new RangePos(1, 0), new RangePos(0, -1), new RangePos(-1, 0) },
        RangeZones = { RangeDirection.Left, RangeDirection.Right }
    };

    //  AXE  //
    public static int[] AxeDamage = { 130, 200, 290, 390 };
    public static Weapon BaseAxe = new Weapon()
    {
        Type = WeaponType.Axe,
        DamageRangePercentage = 0,
        UsePerTurn = 1,
        CritChancePercent = 15,
        CritMultiplierPercent = 120,
        MinRange = 1,
        MaxRange = 1,
        RangePositions = { new RangePos(0, 1), new RangePos(1, 0), new RangePos(0, -1), new RangePos(-1, 0) },
        RangeZones = null
    };

    //  LONG SWORD  //
    public static int[] LongSwordDamage = { 170, 280, 390, 500 };
    public static Weapon BaseLongSword = new Weapon()
    {
        Type = WeaponType.LongSword,
        DamageRangePercentage = 20,
        UsePerTurn = 1,
        CritChancePercent = 5,
        CritMultiplierPercent = 120,
        MinRange = 1,
        MaxRange = 1,
        RangePositions = { new RangePos(0, 1), new RangePos(1, 0), new RangePos(0, -1), new RangePos(-1, 0) },
        RangeZones = { RangeDirection.Up, RangeDirection.Left, RangeDirection.Right, RangeDirection.DiagonalLeft, RangeDirection.DiagonalRight}
    };

    //  GAUNTLET  //
    public static int[] GauntletDamage = { 80, 120, 180, 250 };
    public static Weapon BaseGauntlets = new Weapon()
    {
        Type = WeaponType.Gauntets,
        DamageRangePercentage = 30,
        UsePerTurn = 1,
        CritChancePercent = 1,
        CritMultiplierPercent = 170,
        MinRange = 2,
        MaxRange = 2,
        RangePositions = { new RangePos(-1, 1), new RangePos(1, 1), new RangePos(1, -1), new RangePos(-1, -1) },
        RangeZones = null
    };
}
