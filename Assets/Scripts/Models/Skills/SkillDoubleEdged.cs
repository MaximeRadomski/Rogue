﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDoubleEdged : Skill
{
    public SkillDoubleEdged()
    {
        Name = RacesData.SkillsData.ElfSkillsNames[1];
        Type = SkillType.Racial;
        Nature = SkillNature.Buff;
        Effect = SkillEffect.AttackUp;
        Race = CharacterRace.Elf;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.OnceAFight;
        CooldownMax = -1;
        Cooldown = 0;
        EffectDuration = 0;
        PaNeeded = 2;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,0 };
        IconId = 5;
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        AfterActivation();
    }

    public override int OnStartAttack()
    {
        if (IsApplyingEffect())
            return 100;
        return base.OnStartAttack();
    }

    public override int OnTakeDamage(int damages)
    {
        if (IsApplyingEffect())
            return damages * 2;
        return damages;

    }
}
