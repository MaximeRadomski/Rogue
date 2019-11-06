using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAvarice : Skill
{
    public SkillAvarice()
    {
        Name = RacesData.SkillsData.GoblinSkillsNames[1];
        Type = SkillType.Racial;
        Race = CharacterRace.Gobelin;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Passive;
        CooldownMax = 0;
        Cooldown = 0;
        PaNeeded = 0;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.NoRange;
    }

    public override void OnEndAttack(int damages)
    {
        base.OnEndAttack(damages);
    }
}
