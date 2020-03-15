using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTripleEdged : Skill
{
    public SkillTripleEdged()
    {
        Name = SkillsData.RareSkillsNames[0];
        Type = SkillType.NotRatial;
        Nature = SkillNature.Buff;
        Effect = SkillEffect.AttackUp;
        Rarity = Rarity.Rare;
        CooldownType = CooldownType.OnceAFight;
        CooldownMax = 0;
        EffectDurationMax = 2;
        PaNeeded = 6;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,0 };
        IconId = 16;
        BasePrice = 300;

        Description = "Make and receive triple damages during two turns";
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        AfterActivation();
    }

    public override int OnStartAttack()
    {
        if (IsApplyingEffect())
            return 200;
        return base.OnStartAttack();
    }

    public override int OnTakeDamage(int damages)
    {
        if (IsApplyingEffect())
            return damages * 3;
        return damages;

    }
}
