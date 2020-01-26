using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SkillsData
{
    public string[] HumanSkillsNames = { "Shield", "Jump" };
    public string[] GoblinSkillsNames = { "Vampire", "Avarice" };
    public string[] ElfSkillsNames = { "Dash", "Double Edged" };
    public string[] DwarfSkillsNames = { "Forge", "Grappling Hook" };
    public string[] OrcSkillsNames = { "Roots", "Restoration" };

    public string[] NormalSkillsNames = { "Teleportation", "Push", "Heal", "Clarity" };
    public string[] MagicalSkillsNames = { "Smite", "Great Heal" };
    public string[] RareSkillsNames = { "TripleEdged" };

    public static int RareSkillAppearancePercent = 5;
    public static int MagicalSkillAppearancePercent = 15;

    public Skill GetRandomSkill(bool isPlayer = false)
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

    public Skill GetRandomSkillFromRarity(Rarity rarity)
    {
        if (rarity == Rarity.Rare)
            return GetSkillFromName(RareSkillsNames[UnityEngine.Random.Range(0, RareSkillsNames.Length)]);
        else if (rarity == Rarity.Magical)
            return GetSkillFromName(MagicalSkillsNames[UnityEngine.Random.Range(0, MagicalSkillsNames.Length)]);
        return GetSkillFromName(NormalSkillsNames[UnityEngine.Random.Range(0, NormalSkillsNames.Length)]);
    }

    public Skill GetSkillFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        Type thisType = GetType();
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
