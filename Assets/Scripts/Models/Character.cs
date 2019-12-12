using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public CharacterGender Gender;
    public string Name;
    public CharacterRace Race;
    public CharacterRace StrongAgainst;
    public MapType StrongIn;
    public int Level;
    public int LevelingHealthPercent;
    public int LevelingDamagePercent;
    public int Experience;
    public int Gold;
    public int HpMax;
    public int Hp;
    public int PaMax;
    public int PmMax;
    public List<WeaponType> FavWeapons;
    public List<Weapon> Weapons;
    public List<SkillType> SkillsTypes;
    public List<Skill> Skills;
    public int Score;

    public List<string> BodyParts;
}
