using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RacesData
{
    //  RACES  //
    public static string[] HumanNames = { "Jean", "Pierre", "Paul", "Jacques" };
    public static string[] GoblinNames = { "Gnarf", "Gerf", "Eritoff", "Pif" };
    public static string[] ElfNames = { "Liuv", "Mesenir", "Phalafel", "Gadriel" };
    public static string[] DwarfNames = { "Fluktur", "Parcastre", "Merkandriar", "Jeff" };
    public static string[] OrcNames = { "Mnorg", "Mator", "Grapt", "Zogg" };

    //  HUMAN  //
    public static CharacterBhv BaseHuman = new CharacterBhv()
    {
        Race = CharacterRace.Human,
        HpMax = 300,
        PmMax = 2,
        LevelingHealthPercent = 15,
        LevelingDamagePercent = 15,
        FavWeapons = { WeaponType.Sword, WeaponType.Spear },
    };
}

