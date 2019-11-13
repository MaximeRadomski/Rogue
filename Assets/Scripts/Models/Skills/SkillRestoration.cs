﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRestoration : Skill
{
    public SkillRestoration()
    {
        Name = RacesData.SkillsData.OrcSkillsNames[1];
        Type = SkillType.Racial;
        Race = CharacterRace.Orc;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Passive;
        CooldownMax = 0;
        Cooldown = 0;
        PaNeeded = 0;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.NoRange;
    }

    public override void OnStartTurn()
    {
        float hpToRestore = (CharacterBhv.Character.HpMax / 10);
        hpToRestore *= Helper.MultiplierFromPercent(1.0f, Random.Range(0, 51));
        CharacterBhv.GainHp((int)hpToRestore);
    }
}