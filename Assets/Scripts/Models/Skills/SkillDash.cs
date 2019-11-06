using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDash : Skill
{
    public SkillDash()
    {
        Name = RacesData.SkillsData.ElfSkillsNames[0];
        Type = SkillType.Racial;
        Race = CharacterRace.Elf;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 4;
        Cooldown = 0;
        PaNeeded = 4;
        MinRange = 1;
        MaxRange = 2;
        RangeType = RangeType.OverWalls;
        RangePositions = new List<int> { 0,1, 1,1, 1,0, 1,-1, 0,-1, -1,-1, -1,0, -1,1 };
    }

    private int _currentTargetX;
    private int _currentTargetY;

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        _currentTargetX = x;
        _currentTargetY = y;
    }

    public override int OnTakeDamage(int damages)
    {
        if (Cooldown == CooldownMax)
        {
            CharacterBhv.MoveToPosition(_currentTargetX, _currentTargetY, false);
            return 0;
        }
        return damages;

    }
}
