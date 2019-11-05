using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsData
{
    public string[] HumanSkillsNames = { "Shield", "Jump" };
    public string[] GoblinSkillsNames = { "Vampire", "Avarice" };
    public string[] ElfSkillsNames = { "Dash", "Trap" };
    public string[] DwarfSkillsNames = { "Forge", "Grappling Hook" };
    public string[] OrcSkillsNames = { "Roots", "Restoration" };

    public Skill GetSkillFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        Type thisType = GetType();
        System.Reflection.MethodInfo theMethod = thisType.GetMethod("Get" + name.Replace(" ", ""));
        return (Skill)theMethod.Invoke(this, null);
    }

    public Skill GetShield() { return new SkillShield(); }

    public Skill GetJump() { return new SkillJump(); }
}
