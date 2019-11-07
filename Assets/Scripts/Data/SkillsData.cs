using System;
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

    public string[] NormalSkills = { };
    public string[] MagicSkills = { };
    public string[] RareSkills = { };

    public Skill GetSkillFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        Type thisType = GetType();
        System.Reflection.MethodInfo theMethod = thisType.GetMethod("Get" + name.Replace(" ", ""));
        return (Skill)theMethod.Invoke(this, null);
    }

    public Skill GetShield() { return new SkillDoubleEdged(); }
    public Skill GetJump() { return new SkillJump(); }

    public Skill GetVampire() { return new SkillVampire(); }
    public Skill GetAvarice() { return new SkillAvarice(); }

    public Skill GetDash() { return new SkillDash(); }
    public Skill GetDoubleEdged() { return new SkillDoubleEdged(); }

    public Skill GetForge() { return new SkillForge(); }
    public Skill GetGrapplingHook() { return new SkillGrapplingHook(); }

    public Skill GetRoots() { return new SkillJump(); }
    public Skill GetRestoration() { return new SkillJump(); }
}
