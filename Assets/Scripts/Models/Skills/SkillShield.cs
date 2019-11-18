using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShield : Skill
{
    public SkillShield()
    {
        Name = RacesData.SkillsData.HumanSkillsNames[0];
        Type = SkillType.Racial;
        Nature = SkillNature.Defensive;
        Effect = SkillEffect.Immuned;
        Race = CharacterRace.Human;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 2;
        Cooldown = 0;
        PaNeeded = 2;
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

    public override int OnTakeDamage(int damages)
    {
        if (IsDebuffed)
            return damages;
        if (Cooldown == CooldownMax - EffectDuration)
        {
            CharacterBhv.LoseSkillEffect(Effect);
            return 0;
        }
        return damages;

    }
}
