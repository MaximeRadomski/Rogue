using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDoubleEdged : Skill
{
    public SkillDoubleEdged()
    {
        Name = RacesData.SkillsData.ElfSkillsNames[1];
        Type = SkillType.Racial;
        Nature = SkillNature.Offensive;
        Race = CharacterRace.Elf;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.OnceAFight;
        CooldownMax = -1;
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

    public override int OnStartAttack()
    {
        if (Cooldown == CooldownMax)
            return 100;
        return base.OnStartAttack();
    }

    public override int OnTakeDamage(int damages)
    {
        if (Cooldown == CooldownMax)
            return damages * 2;
        return damages;

    }
}
