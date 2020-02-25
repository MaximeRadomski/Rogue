using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillClarity : Skill
{
    public SkillClarity()
    {
        Name = SkillsData.NormalSkillsNames[3];
        Type = SkillType.NotRatial;
        Nature = SkillNature.Debuff;
        Effect = SkillEffect.None;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 4;
        Cooldown = 0;
        PaNeeded = 2;
        MinRange = 1;
        MaxRange = 1;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,1, 1,0, 0,-1, -1,0 };
        IconId = 13;
        BasePrice = 100;

        Description = "Clear all positive and negative effects";
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        Debuff(GridBhv.IsOpponentOnCell(x, y));
        AfterActivation();
    }

    public void Debuff(CharacterBhv debuffedOpponentBhv)
    {
        if (debuffedOpponentBhv == null)
            return;
        debuffedOpponentBhv.ClearAllSkillEffects(isDebuff:true);
    }
}
