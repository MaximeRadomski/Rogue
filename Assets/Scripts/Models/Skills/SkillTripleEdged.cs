using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTripleEdged : Skill
{
    public SkillTripleEdged()
    {
        Name = RacesData.SkillsData.RareSkillsNames[0];
        Type = SkillType.NotRatial;
        Rarity = Rarity.Rare;
        CooldownType = CooldownType.OnceAFight;
        CooldownMax = -1;
        Cooldown = 0;
        PaNeeded = 6;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,0 };
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        GridBhv.ShowPm(CharacterBhv, OpponentBhvs);
    }

    public override int OnStartAttack()
    {
        if (Cooldown >= CooldownMax - 1)
            return 200;
        return base.OnStartAttack();
    }

    public override int OnTakeDamage(int damages)
    {
        if (Cooldown >= CooldownMax - 1)
            return damages * 3;
        return damages;

    }
}
