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
    public static int NotRaceWeaponDamagePercent = 80;
    public static int StrongAgainstDamagePercent = 20;
    public static int StringInDamagePercent = 10;
    public static int TransgenderChancePercentage = 5;

    public static Character GetCharacterFromRaceAndLevel(CharacterRace race, int level)
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
                tmpCharacter = GetBaseHuman();
                break;
        }
        tmpCharacter.Gender = gender;
        tmpCharacter.Name = name;
        return tmpCharacter;
    }

    //  HUMAN  //
    public static Character GetBaseHuman(bool IsPlayer = true)
    {
        return new Character()
        {
            Race = CharacterRace.Human,
            StrongAgainst = CharacterRace.Gobelin,
            StrongIn = MapType.city,
            HpMax = 300,
            PaMax = 6,
            PmMax = 2,
            LevelingHealthPercent = 15,
            LevelingDamagePercent = 15,
            FavWeapons = new List<WeaponType> { WeaponType.Sword, WeaponType.Spear },
            Weapons = new List<Weapon> { WeaponsData.GetWeaponFromType(WeaponType.Sword, IsPlayer),
                                         WeaponsData.GetWeaponFromType(WeaponType.Spear, IsPlayer) }
        };
    }
}

