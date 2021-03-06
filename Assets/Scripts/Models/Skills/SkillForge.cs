﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillForge : Skill
{
    public SkillForge()
    {
        Name = SkillsData.DwarfSkillsNames[0];
        Type = SkillType.Racial;
        Nature = SkillNature.Buff;
        Effect = SkillEffect.AttackUp;
        Race = CharacterRace.Dwarf;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 4;
        EffectDurationMax = 2;
        PaNeeded = 4;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,0 };
        IconId = 6;
        EffectId = 3;
        BasePrice = 100;

        Description = "Empower your next hits by 75% for the next two turns";
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            CharacterBhv.Instantiator.NewEffect(InventoryItemType.Skill, CharacterBhv.transform.position, null, EffectId, Constants.GridMax - CharacterBhv.Y);
            AfterActivation();
            return true;
        }));
    }

    public override int OnStartAttack()
    {
        if (IsApplyingEffect())
            return 75;
        return base.OnStartAttack();
    }
}
