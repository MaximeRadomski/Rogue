using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RacesData
{
    //  RACES  //
    public static string[] OrcNames = { "Mnorg", "Mator", "Grapt", "Zogg" };

    public static string[] MaleHumanNames = { "Jean", "Pierre", "Paul", "Jacques" };
    public static string[] MaleGoblinNames = { "Gnarf", "Gerf", "Eritoff", "Pif" };
    public static string[] MaleElfNames = { "Liuv", "Mesenir", "Phalafel", "Gadriel" };
    public static string[] MaleDwarfNames = { "Fluktur", "Parcastre", "Merkandriar", "Jeff" };
    public static string[][] MaleCharacterRaceNames = { MaleHumanNames, MaleGoblinNames, MaleElfNames, MaleDwarfNames, OrcNames };

    public static string[] FemaleHumanNames = { "Juliette", "Sophie", "Carmen", "Jacqueline" };
    public static string[] FemaleGoblinNames = { "Gnirf", "Gerurf", "Waganama", "Pliaf" };
    public static string[] FemaleElfNames = { "Liuv", "Mesenir", "Phalafel", "Gadriel" };
    public static string[] FemaleDwarfNames = { "Fluktur", "Parcastre", "Merkandriar", "Jeff" };
    public static string[][] FemaleCharacterRaceNames = { FemaleHumanNames, FemaleGoblinNames, FemaleElfNames, FemaleDwarfNames, OrcNames };

    // BODY PARTS ///
    public const string Head = "Head";
    public const string Hair = "Hair";
    public const string NakedFace = "NakedFace";
    public const string FrontHand = "FrontHand";
    public const string FrontArm = "FrontArm";
    public const string NakedFrontArmHand = "NakedFrontArmHand";
    public const string Waist = "Waist";
    public const string Torso = "Torso";
    public const string NakedTorsoNeck = "NakedTorsoNeck";
    public const string Feet = "Feet";
    public const string Legs = "Legs";
    public const string BackHand = "BackHand";
    public const string BackArm = "BackArm";
    public const string NakedLegsFeet = "NakedLegsFeet";
    public const string NakedBackArmHand = "NakedBackArmHand";
    public static string[] BodyParts = { NakedBackArmHand, NakedLegsFeet, BackArm, BackHand, Legs, Feet, NakedTorsoNeck, Torso, Waist, NakedFrontArmHand, FrontArm, FrontHand, NakedFace, Hair, Head};
    public const int NbSkinTemplates = 4;
    public const int NbBodyTemplates = 8;
    public const int NbHairTemplates = 50;

    //  SHARED STATS  //
    public static int LevelOneXpNeeded = 100;
    public static int NotRaceWeaponDamagePercent = 30; //To Substract
    public static int StrongAgainstDamagePercent = 20;
    public static int StrongInDamagePercent = 10;
    public static int TransgenderChancePercentage = 5;
    public static int GenderDamage = 5;
    public static int GenderCritical = 15;

    public static int InitiativeLevel = 100;
    public static int InitiativeWeapon = 50;
    public static int InitiativeSkill = 50;
    public static int InitiativeStrongIn = 25;

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
        string name = GetRandomNameFromRaceAndGender(race, nameGender);
        Character tmpCharacter = null;
        switch (race)
        {
            case CharacterRace.Human:
                tmpCharacter = GetBaseHuman(gender, isPlayer);
                break;
            case CharacterRace.Gobelin:
                tmpCharacter = GetBaseGobelin(gender, isPlayer);
                break;
            case CharacterRace.Elf:
                tmpCharacter = GetBaseElf(gender, isPlayer);
                break;
            case CharacterRace.Dwarf:
                tmpCharacter = GetBaseDwarf(gender, isPlayer);
                break;
            case CharacterRace.Orc:
                tmpCharacter = GetBaseOrc(gender, isPlayer);
                break;
        }
        tmpCharacter.IsPlayer = isPlayer;
        tmpCharacter.Skills.Add(SkillsData.GetRandomSkill(isPlayer));
        tmpCharacter.Gender = gender;
        tmpCharacter.Name = name;
        tmpCharacter.Level = level;
        tmpCharacter.HpMax = Helper.MaxHpFromLevelOne(tmpCharacter.HpMax, level, tmpCharacter.LevelingHealthPercent);
        tmpCharacter.Gold = Random.Range(0,25*tmpCharacter.Level);
        //DEBUG
        if (race == CharacterRace.Dwarf || race == CharacterRace.Orc)
            tmpCharacter.Gold = 7777;
        tmpCharacter.Experience = 0;
        tmpCharacter.Hp = tmpCharacter.HpMax;
        return tmpCharacter;
    }

    public static string GetRandomNameFromRaceAndGender(CharacterRace race, CharacterGender nameGender)
    {
        if (nameGender == CharacterGender.Male)
            return MaleCharacterRaceNames[race.GetHashCode()][Random.Range(0, MaleCharacterRaceNames[race.GetHashCode()].Length)];
        else
            return FemaleCharacterRaceNames[race.GetHashCode()][Random.Range(0, FemaleCharacterRaceNames[race.GetHashCode()].Length)];
    }

    //  HUMAN  //
    public static Character GetBaseHuman(CharacterGender gender, bool isPlayer = false)
    {
        return new Character()
        {
            Race = CharacterRace.Human,
            StrongAgainst = CharacterRace.Gobelin,
            StrongIn = MapType.City,
            Diet = Diet.Omnivorous,
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
                                                                 SkillsData.GetSkillFromName(SkillsData.HumanSkillsNames[1])},
            BodyParts = CreateBodyPartsFromRace(CharacterRace.Human, gender),
            Inventory = new List<InventoryItem> { ItemsData.GetRandomItem(),
                                                  WeaponsData.GetWeaponFromType(WeaponType.GreatSword),
                                                  SkillsData.GetSkillFromName(SkillsData.RareSkillsNames[0]) },
            InventoryPlace = 4,
            WeightLimit = 40,
            SleepHoursNeeded = 8,
            SleepRestorationPercent = 50
        };
    }

    //  GOBELIN  //
    public static Character GetBaseGobelin(CharacterGender gender, bool isPlayer = false)
    {
        return new Character()
        {
            Race = CharacterRace.Gobelin,
            StrongAgainst = CharacterRace.Elf,
            StrongIn = MapType.Sewers,
            Diet = Diet.Carnivorous,
            HpMax = 250,
            PaMax = 6,
            PmMax = 3,
            LevelingHealthPercent = 10,
            LevelingDamagePercent = 10,
            FavWeapons = new List<WeaponType> { WeaponType.Club, WeaponType.Knife },
            Weapons = new List<Weapon> { WeaponsData.GetWeaponFromType(WeaponType.Club, isPlayer),
                                         WeaponsData.GetWeaponFromType(WeaponType.Knife, isPlayer) },
            SkillsTypes = new List<SkillType> { SkillType.Racial, SkillType.NotRatial },
            Skills = new List<Skill> { Random.Range(0, 2) == 0 ? SkillsData.GetSkillFromName(SkillsData.GoblinSkillsNames[0]) :
                                                                 SkillsData.GetSkillFromName(SkillsData.GoblinSkillsNames[1])},
            BodyParts = CreateBodyPartsFromRace(CharacterRace.Human, gender),
            Inventory = new List<InventoryItem> { ItemsData.GetRandomItem(),
                                                  WeaponsData.GetWeaponFromType(WeaponType.Daggers),
                                                  WeaponsData.GetRandomWeapon(),
                                                  WeaponsData.GetRandomWeapon(),
                                                  SkillsData.GetSkillFromName(SkillsData.HumanSkillsNames[0]),
                                                  SkillsData.GetSkillFromName(SkillsData.MagicalSkillsNames[Random.Range(0, SkillsData.MagicalSkillsNames.Length)])},
            InventoryPlace = 6,
            WeightLimit = 20,
            SleepHoursNeeded = 2,
            SleepRestorationPercent = 25
        };
    }

    //  ELF  //
    public static Character GetBaseElf(CharacterGender gender, bool isPlayer = false)
    {
        return new Character()
        {
            Race = CharacterRace.Elf,
            StrongAgainst = CharacterRace.Dwarf,
            StrongIn = MapType.Forest,
            Diet = Diet.Omnivorous,
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
                                                                 SkillsData.GetSkillFromName(SkillsData.ElfSkillsNames[1])},
            BodyParts = CreateBodyPartsFromRace(CharacterRace.Human, gender),
            Inventory = new List<InventoryItem> { ItemsData.GetRandomItem() },
            InventoryPlace = 2,
            WeightLimit = 20,
            SleepHoursNeeded = 10,
            SleepRestorationPercent = 25
        };
    }


    //  DWARF  //
    public static Character GetBaseDwarf(CharacterGender gender, bool isPlayer = false)
    {
        return new Character()
        {
            Race = CharacterRace.Dwarf,
            StrongAgainst = CharacterRace.Orc,
            StrongIn = MapType.Mines,
            Diet = Diet.Omnivorous,
            HpMax = 350,
            PaMax = 6,
            PmMax = 2,
            LevelingHealthPercent = 10,
            LevelingDamagePercent = 20,
            FavWeapons = new List<WeaponType> { WeaponType.Hammer, WeaponType.Axe },
            Weapons = new List<Weapon> { WeaponsData.GetWeaponFromType(WeaponType.Hammer, isPlayer),
                                         WeaponsData.GetWeaponFromType(WeaponType.Axe, isPlayer) },
            SkillsTypes = new List<SkillType> { SkillType.Racial, SkillType.NotRatial },
            Skills = new List<Skill> { Random.Range(0, 2) == 0 ? SkillsData.GetSkillFromName(SkillsData.DwarfSkillsNames[0]) :
                                                                 SkillsData.GetSkillFromName(SkillsData.DwarfSkillsNames[1])},
            BodyParts = CreateBodyPartsFromRace(CharacterRace.Human, gender),
            Inventory = new List<InventoryItem> { ItemsData.GetRandomItem(),
                                                  ItemsData.GetRandomItemFromRarity(Rarity.Magical) },
            InventoryPlace = 3,
            WeightLimit = 60,
            SleepHoursNeeded = 7,
            SleepRestorationPercent = 75
        };
    }


    //  ORC  //
    public static Character GetBaseOrc(CharacterGender gender, bool isPlayer = false)
    {
        return new Character()
        {
            Race = CharacterRace.Orc,
            StrongAgainst = CharacterRace.Human,
            StrongIn = MapType.Mountains,
            Diet = Diet.Herbivorous,
            HpMax = 500,
            PaMax = 6,
            PmMax = 1,
            LevelingHealthPercent = 20,
            LevelingDamagePercent = 10,
            FavWeapons = new List<WeaponType> { WeaponType.GreatSword, WeaponType.Gauntlets },
            Weapons = new List<Weapon> { WeaponsData.GetWeaponFromType(WeaponType.GreatSword, isPlayer),
                                         WeaponsData.GetWeaponFromType(WeaponType.Gauntlets, isPlayer) },
            SkillsTypes = new List<SkillType> { SkillType.Racial, SkillType.NotRatial },
            Skills = new List<Skill> { Random.Range(0, 2) == 0 ? SkillsData.GetSkillFromName(SkillsData.OrcSkillsNames[0]) :
                                                                 SkillsData.GetSkillFromName(SkillsData.OrcSkillsNames[1])},
            BodyParts = CreateBodyPartsFromRace(CharacterRace.Human, gender),
            Inventory = new List<InventoryItem> { ItemsData.GetRandomItemFromRarity(Rarity.Magical) },
            InventoryPlace = 3,
            WeightLimit = 80,
            SleepHoursNeeded = 12,
            SleepRestorationPercent = 100
        };
    }

    public static List<string> CreateBodyPartsFromRace(CharacterRace race, CharacterGender gender)
    {
        List<string> bodyParts = new List<string>();
        int skinColor = Random.Range(0, NbSkinTemplates);
        int armTemplateId = Random.Range(0, NbBodyTemplates);
        int handTemplateId = Random.Range(0, NbBodyTemplates);
        for (int i = 0; i < BodyParts.Length; ++i)
        {
            if (BodyParts[i].Contains("Naked"))
                bodyParts.Add("Sprites/" + race + "/" + race + BodyParts[i] + "_" + skinColor);
            else if (BodyParts[i].Contains("Hair"))
                bodyParts.Add("Sprites/" + race + "/" + race + gender + BodyParts[i] + "_" + Random.Range(0, NbHairTemplates));
            else if (BodyParts[i].Contains("Arm"))
                bodyParts.Add("Sprites/" + race + "/" + race + BodyParts[i] + "_" + armTemplateId);
            else if (BodyParts[i].Contains("Hand"))
                bodyParts.Add("Sprites/" + race + "/" + race + BodyParts[i] + "_" + handTemplateId);
            else
                bodyParts.Add("Sprites/" + race + "/" + race + BodyParts[i] + "_" + Random.Range(0, NbBodyTemplates));
        }
        return bodyParts;
    }
}

