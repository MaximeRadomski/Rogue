using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRoots : Skill
{
    public SkillRoots()
    {
        Name = RacesData.SkillsData.OrcSkillsNames[0];
        Type = SkillType.Racial;
        Nature = SkillNature.Debuff;
        Effect = SkillEffect.None;
        Race = CharacterRace.Orc;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Passive;
        CooldownMax = 0;
        Cooldown = 0;
        PaNeeded = 0;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.NoRange;
        IconId = 8;

        Description = "Adds a chance to remove <material=\"LongGreen\">0</material> to <material=\"LongGreen\">3 MP</material> each hit";
    }

    public override void OnEndAttack(int damages, CharacterBhv opponentBhv)
    {
        int tmp = Random.Range(0, 100);
        int pmToRemove;
        if (tmp < 5)
            pmToRemove = 0;
        else if (tmp < 50)
            pmToRemove = 1;
        else if (tmp < 95)
            pmToRemove = 2;
        else
            pmToRemove = 3;
        CharacterBhv.LosePm(pmToRemove);
    }
}
