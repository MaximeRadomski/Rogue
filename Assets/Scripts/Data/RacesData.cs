using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RacesData
{
    //  RACES  //
    public static string[] MaleHumanNames = { "Jean", "Pierre", "Paul", "Jacques" };
    public static string[] MaleGoblinNames = { "Gnarf", "Gerf", "Eritoff", "Pif" };
    public static string[] MaleElfNames = { "Liuv", "Mesenir", "Phalafel", "Gadriel" };
    public static string[] MaleDwarfNames = { "Fluktur", "Parcastre", "Merkandriar", "Jeff" };
    public static string[] MaleOrcNames = { "Mnorg", "Mator", "Grapt", "Zogg" };
    public static string[][] MaleCharacterRaceNames = { MaleHumanNames, MaleGoblinNames, MaleElfNames, MaleDwarfNames, MaleOrcNames };

    public static string[] FemaleHumanNames = { "Juliette", "Sophie", "Carmen", "Jacqueline" };
    public static string[] FemaleGoblinNames = { "Gnirf", "Gerurf", "Waganama", "Pliaf" };
    public static string[] FemaleElfNames = { "Liuv", "Mesenir", "Phalafel", "Gadriel" };
    public static string[] FemaleDwarfNames = { "Fluktur", "Parcastre", "Merkandriar", "Jeff" };
    public static string[] FemaleOrcNames = { "Mnorg", "Mator", "Grapt", "Zogg" };
    public static string[][] FemaleCharacterRaceNames = { FemaleHumanNames, FemaleGoblinNames, FemaleElfNames, FemaleDwarfNames, FemaleOrcNames };

    //  SHARED STATS  //
    public static int LevelOneXpNeeded = 100;
    public static int NotRaceWeaponDamagePercent = 60;
    public static int StrongAgainstDamagePercent = 20;
    public static int StrongInDamagePercent = 10;
    public static int TransgenderChancePercentage = 5;
    public static int GenderDamage = 5;
    public static int GenderCritical = 15;

    public static int LevelScore = 100;
    public static int[] RarityScore = { 10, 30, 70, 150 };

    public static SkillsData SkillsData = new SkillsData();

    public static Character GetCharacterFromRaceAndLevel(CharacterRace race, int level, bool isPlayer = false)
    {
        CharacterGender gender = (CharacterGender)Random.Range(0, 2);
        CharacterGender nameGender = gender;
        int genderPercent = Random.Range(0, 100);
        if (genderPercent < TransgenderChancePercentage)
        {
            gender = CharacterGender.Transgender;
            nameGender = (CharacterGender)Random.Range(0, 2);
        }
        string name;
        if (nameGender == CharacterGender.Male)
            name = MaleCharacterRaceNames[race.GetHashCode()][Random.Range(0, MaleCharacterRaceNames[race.GetHashCode()].Length)];
        else
            name = FemaleCharacterRaceNames[race.GetHashCode()][Random.Range(0, FemaleCharacterRaceNames[race.GetHashCode()].Length)];
        Character tmpCharacter = null;
        switch (race)
        {
            case CharacterRace.Human:
                tmpCharacter = GetBaseHuman(isPlayer);
                break;
            case CharacterRace.Gobelin:
                tmpCharacter = GetBaseGobelin(isPlayer);
                break;
            case CharacterRace.Elf:
                tmpCharacter = GetBaseElf(isPlayer);
                break;
            case CharacterRace.Dwarf:
                tmpCharacter = GetBaseDwarf(isPlayer);
                break;
            case CharacterRace.Orc:
                tmpCharacter = GetBaseOrc(isPlayer);
                break;
        }

        if (isPlayer)
            tmpCharacter.Skills.Add(SkillsData.GetRandomSkill(true));
        else
            tmpCharacter.Skills.Add(SkillsData.GetRandomSkill());
        tmpCharacter.Gender = gender;
        tmpCharacter.Name = name;
        tmpCharacter.Level = level;
        tmpCharacter.HpMax = Helper.MaxHpFromLevelOne(tmpCharacter.HpMax, level, tmpCharacter.LevelingHealthPercent);
        tmpCharacter.Gold = 0;
        tmpCharacter.Experience = 0;
        return tmpCharacter;
    }

    //  HUMAN  //
    public static Character GetBaseHuman(bool isPlayer = false)
    {
        return new Character()
        {
            Race = CharacterRace.Human,
            StrongAgainst = CharacterRace.Gobelin,
            StrongIn = MapType.City,
            HpMax = 300,
            PaMax = 6,
            PmMax = 2,
            LevelingHealthPercent = 15,
            LevelingDamagePercent = 15,
            FavWeapons = new List<WeaponType> { WeaponType.Sword, WeaponType.Spear },
            Weapons = new List<Weapon> { WeaponsData.GetWeaponFromType(WeaponType.Sword, isPlayer),
                                         WeaponsData.GetWeaponFromType(WeaponType.Spear, isPlayer) },
            SkillsTypes = new List<SkillType> { SkillType.Racial, SkillType.NotRatial },
            Skills = new List<Skill> { Random.Range(0, 2) == 0 ? SkillsData.GetSkillFromName(SkillsData.HumanSkillsNames[0]) :
                                                                 SkillsData.GetSkillFromName(SkillsData.HumanSkillsNames[1])}
        };
    }

    //  GOBELIN  //
    public static Character GetBaseGobelin(bool isPlayer = false)
    {
        return new Character()
        {
            Race = CharacterRace.Gobelin,
            StrongAgainst = CharacterRace.Elf,
            StrongIn = MapType.Sewers,
            HpMax = 250,
            PaMax = 6,
            PmMax = 3,
            LevelingHealthPercent = 10,
            LevelingDamagePercent = 20,
            FavWeapons = new List<WeaponType> { WeaponType.Club, WeaponType.Knife },
            Weapons = new List<Weapon> { WeaponsData.GetWeaponFromType(WeaponType.Club, isPlayer),
                                         WeaponsData.GetWeaponFromType(WeaponType.Knife, isPlayer) },
            SkillsTypes = new List<SkillType> { SkillType.Racial, SkillType.NotRatial },
            Skills = new List<Skill> { Random.Range(0, 2) == 0 ? SkillsData.GetSkillFromName(SkillsData.GoblinSkillsNames[0]) :
                                                                 SkillsData.GetSkillFromName(SkillsData.GoblinSkillsNames[1])}
        };
    }

    //  ELF  //
    public static Character GetBaseElf(bool isPlayer = false)
    {
        return new Character()
        {
            Race = CharacterRace.Elf,
            StrongAgainst = CharacterRace.Dwarf,
            StrongIn = MapType.Forest,
            HpMax = 200,
            PaMax = 6,
            PmMax = 4,
            LevelingHealthPercent = 5,
            LevelingDamagePercent = 25,
            FavWeapons = new List<WeaponType> { WeaponType.Bow, WeaponType.Daggers },
            Weapons = new List<Weapon> { WeaponsData.GetWeaponFromType(WeaponType.Bow, isPlayer),
                                         WeaponsData.GetWeaponFromType(WeaponType.Daggers, isPlayer) },
            SkillsTypes = new List<SkillType> { SkillType.Racial, SkillType.NotRatial },
            Skills = new List<Skill> { Random.Range(0, 2) == 0 ? SkillsData.GetSkillFromName(SkillsData.ElfSkillsNames[0]) :
                                                                 SkillsData.GetSkillFromName(SkillsData.ElfSkillsNames[1])}
        };
    }


    //  DWARF  //
    public static Character GetBaseDwarf(bool isPlayer = false)
    {
        return new Character()
        {
            Race = CharacterRace.Dwarf,
            StrongAgainst = CharacterRace.Orc,
            StrongIn = MapType.Mines,
            HpMax = 400,
            PaMax = 6,
            PmMax = 2,
            LevelingHealthPercent = 10,
            LevelingDamagePercent = 20,
            FavWeapons = new List<WeaponType> { WeaponType.Hammer, WeaponType.Axe },
            Weapons = new List<Weapon> { WeaponsData.GetWeaponFromType(WeaponType.Hammer, isPlayer),
                                         WeaponsData.GetWeaponFromType(WeaponType.Axe, isPlayer) },
            SkillsTypes = new List<SkillType> { SkillType.Racial, SkillType.NotRatial },
            Skills = new List<Skill> { Random.Range(0, 2) == 0 ? SkillsData.GetSkillFromName(SkillsData.DwarfSkillsNames[0]) :
                                                                 SkillsData.GetSkillFromName(SkillsData.DwarfSkillsNames[1])}
        };
    }


    //  ORC  //
    public static Character GetBaseOrc(bool isPlayer = false)
    {
        return new Character()
        {
            Race = CharacterRace.Orc,
            StrongAgainst = CharacterRace.Human,
            StrongIn = MapType.Mountains,
            HpMax = 500,
            PaMax = 6,
            PmMax = 2,
            LevelingHealthPercent = 20,
            LevelingDamagePercent = 10,
            FavWeapons = new List<WeaponType> { WeaponType.GreatSword, WeaponType.Gauntlets },
            Weapons = new List<Weapon> { WeaponsData.GetWeaponFromType(WeaponType.GreatSword, isPlayer),
                                         WeaponsData.GetWeaponFromType(WeaponType.Gauntlets, isPlayer) },
            SkillsTypes = new List<SkillType> { SkillType.Racial, SkillType.NotRatial },
            Skills = new List<Skill> { Random.Range(0, 2) == 0 ? SkillsData.GetSkillFromName(SkillsData.OrcSkillsNames[0]) :
                                                                 SkillsData.GetSkillFromName(SkillsData.OrcSkillsNames[1])}
        };
    }
}

