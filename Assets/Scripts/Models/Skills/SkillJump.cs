﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillJump : Skill
{
    public SkillJump()
    {
        Name = SkillsData.HumanSkillsNames[1];
        Type = SkillType.Racial;
        Nature = SkillNature.Movement;
        Effect = SkillEffect.None;
        Race = CharacterRace.Human;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 2;
        PaNeeded = 2;
        MinRange = 1;
        MaxRange = 2;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,1, 0,2, 1,1, 1,0, 2,0, 1,-1, 0,-1, 0,-2, -1,-1, -1,0, -2,0, -1,1 };
        IconId = 1;
        EffectId = 2;
        BasePrice = 100;

        Description = "Jump in or out of the fight";
    }

    public override void OnClick()
    {
        if (!IsUnderCooldown())
            GridBhv.ShowSkillRange(RangeType, CharacterBhv, Id, OpponentBhvs, true);
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            CharacterBhv.Instantiator.NewEffect(InventoryItemType.Skill, GridBhv.Cells[x, y].transform.position, null, EffectId, Constants.GridMax - y);
            CharacterBhv.MoveToPosition(x, y, false);
            return true;
        }));
    }
}
