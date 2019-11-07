using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillForge : Skill
{
    public SkillForge()
    {
        Name = RacesData.SkillsData.DwarfSkillsNames[0];
        Type = SkillType.Racial;
        Race = CharacterRace.Dwarf;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 4;
        Cooldown = 0;
        PaNeeded = 4;
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

    public override float OnStartAttack()
    {
        if (Cooldown > CooldownMax - 3)
            return 0.75f;
        return base.OnStartAttack();
    }
}
