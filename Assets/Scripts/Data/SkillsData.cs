using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public static class SkillsData
{
    public static string[] HumanSkillsNames = { "Shield", "Jump" };
    public static string[] GoblinSkillsNames = { "Vampire", "Avarice" };
    public static string[] ElfSkillsNames = { "Dash", "Double Edged" };
    public static string[] DwarfSkillsNames = { "Forge", "Grappling Hook" };
    public static string[] OrcSkillsNames = { "Roots", "Restoration" };

    public static string[] NormalSkillsNames = { "Clarity", "Push", "Heal" };
    public static string[] MagicalSkillsNames = { "Smite", "Great Heal", "Teleportation" };
    public static string[] RareSkillsNames = { "TripleEdged" };

    public static int RareSkillAppearancePercent = 5;
    public static int MagicalSkillAppearancePercent = 15;

    public static Skill GetRandomSkill(bool isPlayer = false)
    {
        if (isPlayer)
            return GetRandomSkillFromRarity(Rarity.Normal);
        int rarityPercent = UnityEngine.Random.Range(0, 100);
        if (rarityPercent < RareSkillAppearancePercent)
            return GetRandomSkillFromRarity(Rarity.Rare);
        else if (rarityPercent < MagicalSkillAppearancePercent)
            return GetRandomSkillFromRarity(Rarity.Magical);
        return GetRandomSkillFromRarity(Rarity.Normal);
    }

    public static Skill GetRandomSkillFromRarity(Rarity rarity)
    {
        if (rarity == Rarity.Rare)
            return GetSkillFromName(RareSkillsNames[UnityEngine.Random.Range(0, RareSkillsNames.Length)]);
        else if (rarity == Rarity.Magical)
            return GetSkillFromName(MagicalSkillsNames[UnityEngine.Random.Range(0, MagicalSkillsNames.Length)]);
        return GetSkillFromName(NormalSkillsNames[UnityEngine.Random.Range(0, NormalSkillsNames.Length)]);
    }

    public static Skill GetSkillFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        //Debug.Log("Get" + name.Replace(" ", ""));
        //System.Reflection.MethodInfo theMethod = thisType.GetMethod("Get" + name.Replace(" ", ""));
        var instance = Activator.CreateInstance(Type.GetType("Skill" + name.Replace(" ", "")));
        return (Skill)instance;
        //return (Skill)theMethod.Invoke(this, null);
    }

    // RACIAL SKILLS
    //public Skill GetShield() { return new SkillShield(); }
    //public Skill GetJump() { return new SkillJump(); }

    //public Skill GetVampire() { return new SkillVampire(); }
    //public Skill GetAvarice() { return new SkillAvarice(); }

    //public Skill GetDash() { return new SkillDash(); }
    //public Skill GetDoubleEdged() { return new SkillDoubleEdged(); }

    //public Skill GetForge() { return new SkillForge(); }
    //public Skill GetGrapplingHook() { return new SkillGrapplingHook(); }

    //public Skill GetRoots() { return new SkillRoots(); }
    //public Skill GetRestoration() { return new SkillRestoration(); }


    // NORMAL SKILLS
    //public Skill GetTeleportation() { return new SkillTeleportation(); }
    //public Skill GetPush() { return new SkillPush(); }
    //public Skill GetHeal() { return new SkillHeal(); }

    //public Skill GetClarity() { return new SkillClarity(); }

    // MAGICAL SKILLS
    //public Skill GetSmite() { return new SkillSmite(); }
    //public Skill GetGreatHeal() { return new SkillGreatHeal(); }

    // RARE SKILLS
    //public Skill GetTripleEdged() { return new SkillTripleEdged(); }
}
