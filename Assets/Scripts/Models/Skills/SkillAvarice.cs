using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAvarice : Skill
{
    public SkillAvarice()
    {
        Name = SkillsData.GoblinSkillsNames[1];
        Type = SkillType.Racial;
        Nature = SkillNature.Buff;
        Effect = SkillEffect.None;
        Race = CharacterRace.Gobelin;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Passive;
        CooldownMax = 0;
        Cooldown = 0;
        PaNeeded = 0;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.NoRange;
        IconId = 3;
        BasePrice = 100;

        Description = "Steal <material=\"LongGold\">Gold</material> on each hit";
    }

    public override void OnEndAttack(int damages, CharacterBhv opponentBhv)
    {
        base.OnEndAttack(damages, opponentBhv);
    }
}
