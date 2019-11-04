﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponsData
{
    //  WEAPONS //
    public static string[] SwordNames = { "Sword", "Short Sword", "Bastard Sword", "Claymore", "Sabre" };
    public static string[] SpearNames = { "Spear", "Trident", "Pike", "Lance", "Halberd" };
    public static string[] ClubNames = { "Club", "Spiked Club", "Crowbar", "Pickaxe", "Baton" };
    public static string[] KnifeNames = { "Knife", "Kris", "Bayonet", "Cleaver", "Shiv" };
    public static string[] BowNames = { "Bow", "Short Bow", "Long Bow", "Composite Bow", "Hunter's Bow" };
    public static string[] DaggersNames = { "Daggers", "Dirk", "Poignard", "Stiletto", "Rondel" };
    public static string[] HammerNames = { "Hammer", "Maul", "Morning Star", "Flail", "War Hammer" };
    public static string[] AxeNames = { "Axe", "Hatchet", "Broad Axe", "Great Axe", "Ono" };
    public static string[] GreatSwordNames = { "Great Sword", "Long Sword", "Zweihander", "Flamberge", "Reaver" };
    public static string[] GauntletsNames = { "Gauntlets", "Cestus", "Fangs", "Claws", "Knuckles" };
    public static string[][] WeaponTypeNames = { SwordNames, SpearNames, ClubNames, KnifeNames, BowNames, DaggersNames, HammerNames, AxeNames, GreatSwordNames, GauntletsNames };
    public static string[] NormalNames = { "", "Classic", "Usual", "Unoriginal", "Average", "Child", "Weird", "Redneck" };
    public static string[] MagicalNames = { "Magical", "Enchanted", "Lost", "Knight", "Your mom's", "Mythical", "Ok" };
    public static string[] RareNames = { "|Of Doom", "Enormous", "Super", "|2.0", "|of Death", "|of Truth", "|of Fertility", "El Famoso" };
    public static int BaseInitDamageRangePercentage = 15;
    public static int ChanceNotRaceWeaponPercent = 10;
    public static int RareWeaponAppearancePercent = 5;
    public static int MagicalWeaponAppearancePercent = 15;

    public static Weapon GetWeaponFromType(WeaponType type, bool isBase = false)
    {
        int notTypeIfZero = Random.Range(0, 100 / ChanceNotRaceWeaponPercent);
        if (!isBase && notTypeIfZero == 0)
        {
            type = (WeaponType)Random.Range(0, System.Enum.GetNames(typeof(WeaponType)).Length);
        }
        WeaponRarity rarity = WeaponRarity.Normal;
        if (!isBase)
        {
            int rarityPercent = Random.Range(0, 100);
            if (rarityPercent < RareWeaponAppearancePercent)
                rarity = WeaponRarity.Rare;
            else if (rarityPercent < MagicalWeaponAppearancePercent)
                rarity = WeaponRarity.Magical;
        }
        string name = GetWeaponNameFromRarity(type, rarity, isBase);
        Weapon tmpWeapon = null;
        switch (type)
        {
            case WeaponType.Sword:
                tmpWeapon = GetBaseSword();
                tmpWeapon.BaseDamage = SwordDamage[rarity.GetHashCode()];
                break;
            case WeaponType.Spear:
                tmpWeapon = GetBaseSpear();
                tmpWeapon.BaseDamage = SpearDamage[rarity.GetHashCode()];
                break;
            case WeaponType.Club:
                tmpWeapon = GetBaseClub();
                tmpWeapon.BaseDamage = ClubDamage[rarity.GetHashCode()];
                break;
            case WeaponType.Knife:
                tmpWeapon = GetBaseKnife();
                tmpWeapon.BaseDamage = KnifeDamage[rarity.GetHashCode()];
                break;
            case WeaponType.Bow:
                tmpWeapon = GetBaseBow();
                tmpWeapon.BaseDamage = BowDamage[rarity.GetHashCode()];
                break;
            case WeaponType.Daggers:
                tmpWeapon = GetBaseDaggers();
                tmpWeapon.BaseDamage = DaggersDamage[rarity.GetHashCode()];
                break;
            case WeaponType.Hammer:
                tmpWeapon = GetBaseHammer();
                tmpWeapon.BaseDamage = HammerDamage[rarity.GetHashCode()];
                break;
            case WeaponType.Axe:
                tmpWeapon = GetBaseAxe();
                tmpWeapon.BaseDamage = AxeDamage[rarity.GetHashCode()];
                break;
            case WeaponType.GreatSword:
                tmpWeapon = GetBaseGreatSword();
                tmpWeapon.BaseDamage = GreatSwordDamage[rarity.GetHashCode()];
                break;
            case WeaponType.Gauntlets:
                tmpWeapon = GetBaseGauntlets();
                tmpWeapon.BaseDamage = GauntletsDamage[rarity.GetHashCode()];
                break;
        }
        tmpWeapon.Name = name;
        tmpWeapon.Rarity = rarity;
        var baseDamageFloat = tmpWeapon.BaseDamage * Helper.MultiplierFromPercent(1, Random.Range(-BaseInitDamageRangePercentage, BaseInitDamageRangePercentage));
        tmpWeapon.BaseDamage = (int)baseDamageFloat;
        return tmpWeapon;
    }

    private static string GetWeaponNameFromRarity(WeaponType type, WeaponRarity rarity, bool isBase = false)
    {
        var weaponTypeName = WeaponTypeNames[type.GetHashCode()][Random.Range(0, WeaponTypeNames[type.GetHashCode()].Length)];
        if (isBase)
            return weaponTypeName;
        switch (rarity)
        {
            case WeaponRarity.Normal:
                return NormalNames[Random.Range(0, NormalNames.Length)] + " " + weaponTypeName;
            case WeaponRarity.Magical:
                return MagicalNames[Random.Range(0, MagicalNames.Length)] + " " + weaponTypeName;
            case WeaponRarity.Rare:
                var tmpName = MagicalNames[Random.Range(0, MagicalNames.Length)] + " " + weaponTypeName;
                var rareNameId = Random.Range(0, RareNames.Length);
                if (RareNames[rareNameId][0] == '|')
                {
                    return tmpName + " " + RareNames[rareNameId].Substring(1);
                }
                return RareNames[rareNameId] + " " + tmpName;
        }
        return weaponTypeName;
    }

    public static List<RangePos> GetWeaponRangeFromType(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword:
                return new List<RangePos> { new RangePos(0, 1), new RangePos(1, 0), new RangePos(0, -1), new RangePos(-1, 0) };
            case WeaponType.Spear:
                return new List<RangePos> { new RangePos(0, 2), new RangePos(2, 0), new RangePos(0, -2), new RangePos(-2, 0) };
            case WeaponType.Club:
                return new List<RangePos> { new RangePos(0, 1), new RangePos(1, 0), new RangePos(0, -1), new RangePos(-1, 0) };
            case WeaponType.Knife:
                return new List<RangePos> { new RangePos(-1, 1), new RangePos(1, 1), new RangePos(1, -1), new RangePos(-1, -1) };
            case WeaponType.Bow:
                return new List<RangePos> { new RangePos(0, 2), new RangePos(0, 3), new RangePos(0, 4), new RangePos(0, 5),
                                            new RangePos(2, 0), new RangePos(3, 0), new RangePos(4, 0), new RangePos(5, 0),
                                            new RangePos(0, -2), new RangePos(0, -3), new RangePos(0, -4), new RangePos(0, -5),
                                            new RangePos(-2, 0), new RangePos(-3, 0), new RangePos(-4, 0), new RangePos(-5, 0)};
            case WeaponType.Daggers:
                return new List<RangePos> { new RangePos(0, 1), new RangePos(1, 0), new RangePos(0, -1), new RangePos(-1, 0) };
            case WeaponType.Hammer:
                return new List<RangePos> { new RangePos(0, 1), new RangePos(1, 0), new RangePos(0, -1), new RangePos(-1, 0) };
            case WeaponType.Axe:
                return new List<RangePos> { new RangePos(0, 1), new RangePos(1, 0), new RangePos(0, -1), new RangePos(-1, 0) };
            case WeaponType.GreatSword:
                return new List<RangePos> { new RangePos(0, 1), new RangePos(1, 0), new RangePos(0, -1), new RangePos(-1, 0) };
            case WeaponType.Gauntlets:
                return new List<RangePos> { new RangePos(-1, 1), new RangePos(1, 1), new RangePos(1, -1), new RangePos(-1, -1) };
        }
        return null;
    }

    //  SWORD  //
    public static int[] SwordDamage = { 100, 150, 220, 300 };
    public static Weapon GetBaseSword()
    {
        return new Weapon()
        {
            Type = WeaponType.Sword,
            DamageRangePercentage = 10,
            PaNeeded = 4,
            CritChancePercent = 5,
            CritMultiplierPercent = 50,
            MinRange = 1,
            MaxRange = 1,
            RangePositions = GetWeaponRangeFromType(WeaponType.Sword),
            RangeZones = null
        };
    }

    //  SPEAR  //
    public static int[] SpearDamage = { 120, 180, 260, 350 };
    public static Weapon GetBaseSpear()
    {
        return new Weapon()
        {
            Type = WeaponType.Spear,
            DamageRangePercentage = 5,
            PaNeeded = 4,
            CritChancePercent = 1,
            CritMultiplierPercent = 150,
            MinRange = 2,
            MaxRange = 2,
            RangePositions = GetWeaponRangeFromType(WeaponType.Spear),
            RangeZones = null
        };
    }

    //  CLUB  //
    public static int[] ClubDamage = { 75, 100, 150, 225 };
    public static Weapon GetBaseClub()
    {
        return new Weapon()
        {
            Type = WeaponType.Club,
            DamageRangePercentage = 15,
            PaNeeded = 4,
            CritChancePercent = 10,
            CritMultiplierPercent = 30,
            MinRange = 1,
            MaxRange = 1,
            RangePositions = GetWeaponRangeFromType(WeaponType.Club),
            RangeZones = new List<RangeDirection> { RangeDirection.Up }
        };
    }

    //  KNIFE  //
    public static int[] KnifeDamage = { 30, 40, 65, 100 };
    public static Weapon GetBaseKnife()
    {
        return new Weapon()
        {
            Type = WeaponType.Knife,
            DamageRangePercentage = 50,
            PaNeeded = 3,
            CritChancePercent = 33,
            CritMultiplierPercent = 40,
            MinRange = 2,
            MaxRange = 2,
            RangePositions = GetWeaponRangeFromType(WeaponType.Knife),
            RangeZones = null
        };
    }

    //  BOW  //
    public static int[] BowDamage = { 60, 100, 140, 180 };
    public static Weapon GetBaseBow()
    {
        return new Weapon()
        {
            Type = WeaponType.Bow,
            DamageRangePercentage = 5,
            PaNeeded = 4,
            CritChancePercent = 1,
            CritMultiplierPercent = 150,
            MinRange = 3,
            MaxRange = 5,
            RangePositions = GetWeaponRangeFromType(WeaponType.Bow),
            RangeZones = null
        };
    }

    //  DAGGERS  //
    public static int[] DaggersDamage = { 20, 40, 80, 180 };
    public static Weapon GetBaseDaggers()
    {
        return new Weapon()
        {
            Type = WeaponType.Daggers,
            DamageRangePercentage = 10,
            PaNeeded = 2,
            CritChancePercent = 50,
            CritMultiplierPercent = 50,
            MinRange = 1,
            MaxRange = 1,
            RangePositions = GetWeaponRangeFromType(WeaponType.Daggers),
            RangeZones = null
        };
    }

    //  HAMMER  //
    public static int[] HammerDamage = { 90, 130, 190, 260 };
    public static Weapon GetBaseHammer()
    {
        return new Weapon()
        {
            Type = WeaponType.Hammer,
            DamageRangePercentage = 0,
            PaNeeded = 5,
            CritChancePercent = 15,
            CritMultiplierPercent = 40,
            MinRange = 1,
            MaxRange = 1,
            RangePositions = GetWeaponRangeFromType(WeaponType.Hammer),
            RangeZones = new List<RangeDirection> { RangeDirection.Left, RangeDirection.Right }
        };
    }

    //  AXE  //
    public static int[] AxeDamage = { 130, 200, 290, 390 };
    public static Weapon GetBaseAxe()
    {
        return new Weapon()
        {
            Type = WeaponType.Axe,
            DamageRangePercentage = 0,
            PaNeeded = 4,
            CritChancePercent = 15,
            CritMultiplierPercent = 40,
            MinRange = 1,
            MaxRange = 1,
            RangePositions = GetWeaponRangeFromType(WeaponType.Axe),
            RangeZones = null
        };
    }

    //  LONG SWORD  //
    public static int[] GreatSwordDamage = { 170, 280, 390, 500 };
    public static Weapon GetBaseGreatSword()
    {
        return new Weapon()
        {
            Type = WeaponType.GreatSword,
            DamageRangePercentage = 20,
            PaNeeded = 5,
            CritChancePercent = 5,
            CritMultiplierPercent = 70,
            MinRange = 1,
            MaxRange = 1,
            RangePositions = GetWeaponRangeFromType(WeaponType.GreatSword),
            RangeZones = new List<RangeDirection> { RangeDirection.Up, RangeDirection.Left, RangeDirection.Right, RangeDirection.DiagonalLeft, RangeDirection.DiagonalRight }
        };
    }

    //  GAUNTLETS  //
    public static int[] GauntletsDamage = { 80, 120, 180, 250 };
    public static Weapon GetBaseGauntlets()
    {
        return new Weapon()
        {
            Type = WeaponType.Gauntlets,
            DamageRangePercentage = 30,
            PaNeeded = 4,
            CritChancePercent = 1,
            CritMultiplierPercent = 130,
            MinRange = 2,
            MaxRange = 2,
            RangePositions = GetWeaponRangeFromType(WeaponType.Gauntlets),
            RangeZones = null
        };
    }
}
