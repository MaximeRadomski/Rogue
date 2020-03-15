using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponsData
{
    //  WEAPONS //
    public static string[] SwordNames = { "Sword", "Short Sword", "Bastard Sword", "Claymore", "Sabre" };
    public static string[] SpearNames = { "Spear", "Trident", "Pike", "Lance", "Halberd" };
    public static string[] ClubNames = { "Club", "Spiked Club", "Crowbar", "Pickaxe", "Baton" };
    public static string[] KnifeNames = { "Knife", "Kris", "Bayonet", "Cleaver", "Shiv" };
    public static string[] BowNames = { "Bow", "Short Bow", "Long Bow", "Composite Bow", "Hunting Bow" };
    public static string[] DaggersNames = { "Daggers", "Dirks", "Poignards", "Stilettos", "Rondels" };
    public static string[] HammerNames = { "Hammer", "Maul", "Morning Star", "Flail", "War Hammer" };
    public static string[] AxeNames = { "Axe", "Hatchet", "Broad Axe", "Great Axe", "Ono" };
    public static string[] GreatSwordNames = { "Great Sword", "Long Sword", "Zweihander", "Flamberge", "Reaver" };
    public static string[] GauntletsNames = { "Gauntlets", "Cestus", "Fangs", "Claws", "Knuckles" };
    public static string[][] WeaponTypeNames = { SwordNames, SpearNames, ClubNames, KnifeNames, BowNames, DaggersNames, HammerNames, AxeNames, GreatSwordNames, GauntletsNames };
    public static string[] NormalNames = { "", "Classic", "Usual", "Unoriginal", "Average", "Child", "Weird", "Redneck", "Common", "So Common" };
    public static string[] MagicalNames = { "Magical", "Enchanted", "Lost", "Knight", "Mythical", "Ok", "Good", "Handmade", "Exciting" };
    public static string[] RareNames = { "Your mom's", "|of Doom", "Enormous", "Super", "|2.0", "|of Death", "|of Truth", "|of Fertility", "El Famoso", "|of Legend", "|of Love" };
    public static int BaseInitDamageRangePercentage = 15;
    public static int ChanceNotRaceWeaponPercent = 10;
    public static int RareWeaponAppearancePercent = 5;
    public static int MagicalWeaponAppearancePercent = 15;
    public static int MaxSharpenedAmount = 3;

    public static Weapon GetRandomWeapon()
    {
        return GetWeaponFromType((WeaponType)Random.Range(0, Helper.EnumCount<WeaponType>()));
    }

    public static Weapon GetWeaponFromType(WeaponType type, bool isBase = false)
    {
        int notAskedType = Random.Range(0, 100);
        if (!isBase && notAskedType < ChanceNotRaceWeaponPercent)
        {
            var tmpType = (WeaponType)Random.Range(0, System.Enum.GetNames(typeof(WeaponType)).Length);
            if (tmpType != WeaponType.GreatSword) //Because GreatSwords can only be equipped with small weapons.
                type = tmpType;
        }
        Rarity rarity = Rarity.Normal;
        if (!isBase)
        {
            if (Random.Range(0, 100) < RareWeaponAppearancePercent)
                rarity = Rarity.Rare;
            else if (Random.Range(0, 100) < MagicalWeaponAppearancePercent)
                rarity = Rarity.Magical;
        }
        var subType = Random.Range(0, WeaponTypeNames[type.GetHashCode()].Length);
        string name = GetWeaponNameFromRarity(type.GetHashCode(), subType, rarity/*, isBase*/);
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
        var baseDamageFloat = tmpWeapon.BaseDamage * Helper.MultiplierFromPercent(1, Random.Range(-BaseInitDamageRangePercentage, BaseInitDamageRangePercentage + 1));
        tmpWeapon.BaseDamage = (int)baseDamageFloat;
        tmpWeapon.WeaponParts = CreateWeaponPartsFromTypeSubType(type, subType, tmpWeapon.NbSkinParts);
        return tmpWeapon;
    }

    private static string GetWeaponNameFromRarity(int type, int subType, Rarity rarity, bool isBase = false)
    {
        var weaponTypeName = WeaponTypeNames[type][subType];
        if (isBase)
            return weaponTypeName;
        switch (rarity)
        {
            case Rarity.Normal:
                return NormalNames[Random.Range(0, NormalNames.Length)] + " " + weaponTypeName;
            case Rarity.Magical:
                return MagicalNames[Random.Range(0, MagicalNames.Length)] + " " + weaponTypeName;
            case Rarity.Rare:
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

    public static List<string> CreateWeaponPartsFromTypeSubType(WeaponType type, int subType, int nbSkinParts)
    {
        List<string> weaponParts = new List<string>();
        for (int i = 0; i < nbSkinParts; ++i)
        {
            //         (    subTypeTripleLine    ) + (               line            ) + (column)
            var rand = (subType * nbSkinParts * 3) + (Random.Range(0, 3) * nbSkinParts + i);
            weaponParts.Add("Sprites/Weapons/" + type + "_" + rand);
        }
        return weaponParts;
    }

    public static bool IsSmallWeapon(WeaponType type)
    {
        return (type == WeaponType.Knife || type == WeaponType.Daggers || type == WeaponType.Gauntlets);
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
            RangePositions = new List<int> { 0,1, 1,0, 0,-1, -1,0 },
            RangeZones = null,
            NbSkinParts = 4,
            Weight = Random.Range(3, 6 + 1),
            BasePrice = 100,
            EffectId = 1
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
            RangePositions = new List<int> { 0,2, 2,0, 0,-2, -2,0 },
            RangeZones = null,
            NbSkinParts = 4,
            SpecificityTitle = "Straightforward",
            Specificity = "Only in line.",
            Weight = Random.Range(3, 6 + 1),
            BasePrice = 90,
            EffectId = 1
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
            RangePositions = new List<int> { 0,1, 1,0, 0,-1, -1,0 },
            RangeZones = new List<RangeDirection> { RangeDirection.Up },
            NbSkinParts = 4,
            Weight = Random.Range(2, 4 + 1),
            BasePrice = 40,
            EffectId = 2
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
            RangePositions = new List<int> { -1,1, 1,1, 1,-1, -1,-1 },
            RangeZones = null,
            NbSkinParts = 4,
            SpecificityTitle = "Curvy",
            Specificity = "Only diagonally",
            Weight = Random.Range(1, 2 + 1),
            BasePrice = 20,
            EffectId = 1
        };
    }

    //  BOW  //
    public static int[] BowDamage = { 80, 120, 160, 200 };
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
            RangePositions = new List<int> { /*0,2,*/ 0,3, 0,4, 0,5,
                                             1,2, 1,3, 1,4, 2,2, 2,3, 3,2, 2,1, 3,1, 4,1,
                                             /*2,0,*/ 3,0, 4,0, 5,0,
                                             2,-1, 3,-1, 4,-1, -2,2, -2,3, -3,2, 1,-2, 1,-3, 1,-4,
                                             /*0,-2,*/ 0,-3, 0,-4, 0,-5,
                                             -1,-2, -1,-3, -1,-4, -2,-2, -2,-3, -3,-2, -2,-1, -3,-1, -4,-1,
                                             /*-2,0,*/ -3,0, -4,0, -5,0,
                                             -2,1, -3,1, -4,1, 2,-2, 2,-3, 3,-2, -1,2, -1,3, -1,4, },
            RangeZones = null,
            NbSkinParts = 3,
            Weight = Random.Range(3, 6 + 1),
            BasePrice = 150,
            EffectId = 5
        };
    }

    //  DAGGERS  //
    public static int[] DaggersDamage = { 30, 50, 80, 150 };
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
            RangePositions = new List<int> { 0,1, 1,0, 0,-1, -1,0 },
            RangeZones = null,
            NbSkinParts = 4,
            Weight = Random.Range(2, 4 + 1),
            BasePrice = 120,
            EffectId = 1
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
            RangePositions = new List<int> { 0,1, 1,0, 0,-1, -1,0 },
            RangeZones = new List<RangeDirection> { RangeDirection.Left, RangeDirection.Right },
            NbSkinParts = 5,
            SpecificityTitle = "Dwarf Craftsmanship",
            Specificity = "Fixed Damage Range",
            Weight = Random.Range(10, 20 + 1),
            BasePrice = 200,
            EffectId = 3
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
            RangePositions = new List<int> { 0,1, 1,0, 0,-1, -1,0 },
            RangeZones = null,
            NbSkinParts = 5,
            SpecificityTitle = "Dwarf Craftsmanship",
            Specificity = "Fixed Damage Range.",
            Weight = Random.Range(5, 10 + 1),
            BasePrice = 200,
            EffectId = 1
        };
    }

    //  GREAT SWORD  //
    public static int[] GreatSwordDamage = { 170, 280, 390, 500 };
    public static Weapon GetBaseGreatSword()
    {
        return new Weapon()
        {
            Type = WeaponType.GreatSword,
            DamageRangePercentage = 20,
            PaNeeded = 6,
            CritChancePercent = 5,
            CritMultiplierPercent = 70,
            MinRange = 1,
            MaxRange = 1,
            RangePositions = new List<int> { 0,1, 1,0, 0,-1, -1,0 },
            RangeZones = new List<RangeDirection> { RangeDirection.Up, RangeDirection.Left, RangeDirection.Right },
            NbSkinParts = 5,
            SpecificityTitle = "Heavy Weapon",
            Specificity = "Can only be equipped with small weapons (knives, daggers, gauntlets).",
            Weight = Random.Range(10, 20 + 1),
            BasePrice = 250,
            EffectId = 4
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
            RangePositions = new List<int> { -1,1, 0,1, 1,1, 1,0, 1,-1, 0,-1, -1,-1, -1,0 },
            RangeZones = null,
            NbSkinParts = 2,
            SpecificityTitle = "Handy",
            Specificity = "Hits in all directions",
            Weight = Random.Range(4, 8 + 1),
            BasePrice = 80,
            EffectId = 1
        };
    }
}
