﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTeleportation : Skill
{
    public SkillTeleportation()
    {
        Name = RacesData.SkillsData.NormalSkillsNames[0];
        Type = SkillType.NotRatial;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 5;
        Cooldown = 0;
        PaNeeded = 4;
        MinRange = 99;
        MaxRange = 99;
        RangeType = RangeType.FullRange;
    }

    public override void OnClick()
    {
        if (Cooldown == 0)
            GridBhv.ShowSkillRange(RangeType, CharacterBhv, Id, OpponentBhvs, true);
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        CharacterBhv.MoveToPosition(x, y, false);
    }
}