using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsData
{
    public string[] SkillsNames = {
        "Shield", "Jump",
        "Vampire", "Avarice",
        "Dash", "Trap",
        "Forge", "Grappling Hook",
        "Roots", "Restoration"
    };

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
