using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHeal : Skill
{
    public SkillHeal()
    {
        Name = RacesData.SkillsData.NormalSkillsNames[2];
        Type = SkillType.NotRatial;
        Nature = SkillNature.Defensive;
        Effect = SkillEffect.None;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 3;
        Cooldown = 0;
        PaNeeded = 3;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,0 };
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        var floatAmount = 50.0f * Helper.MultiplierFromPercent(1, 10 * (CharacterBhv.Character.Level - 1));
        CharacterBhv.GainHp((int)floatAmount);
        AfterActivation();
    }
}
