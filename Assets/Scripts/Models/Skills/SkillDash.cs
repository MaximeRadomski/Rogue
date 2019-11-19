﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDash : Skill
{
    public SkillDash()
    {
        Name = RacesData.SkillsData.ElfSkillsNames[0];
        Type = SkillType.Racial;
        Nature = SkillNature.MovementFar;
        Effect = SkillEffect.Immuned;
        Race = CharacterRace.Elf;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 4;
        Cooldown = 0;
        EffectDuration = 0;
        PaNeeded = 4;
        MinRange = 1;
        MaxRange = 2;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,1, 1,1, 1,0, 1,-1, 0,-1, -1,-1, -1,0, -1,1 };
    }

    private int _currentTargetX;
    private int _currentTargetY;

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        _currentTargetX = x;
        _currentTargetY = y;
        CharacterBhv.GainSkillEffect(SkillEffect.Immuned);
        GridBhv.ShowPm(CharacterBhv, OpponentBhvs);
    }

    public override int OnTakeDamage(int damages)
    {
        if (IsApplyingEffect())
        {
            if (!Helper.IsPosValid(_currentTargetX, _currentTargetX) || GridBhv.IsOpponentOnCell(_currentTargetX, _currentTargetY))
                CharacterBhv.MoveToPosition(_currentTargetX, _currentTargetY, false);
            CharacterBhv.LoseSkillEffect(Effect);
            return 0;
        }
        return damages;
    }
}
