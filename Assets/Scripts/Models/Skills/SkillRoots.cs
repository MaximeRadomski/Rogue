﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRoots : Skill
{
    public SkillRoots()
    {
        Name = RacesData.SkillsData.OrcSkillsNames[0];
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

    public override void OnEndAttack(int damages, CharacterBhv opponentBhv)
    {
        int tmp = Random.Range(0, 100);
        int pmToRemove;
        if (tmp < 5)
            pmToRemove = 0;
        else if (tmp < 50)
            pmToRemove = 1;
        else if (tmp < 95)
            pmToRemove = 2;
        else
            pmToRemove = 3;
        CharacterBhv.LoosePm(pmToRemove);
    }
}