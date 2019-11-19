using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillJump : Skill
{
    public SkillJump()
    {
        Name = RacesData.SkillsData.HumanSkillsNames[1];
        Type = SkillType.Racial;
        Nature = SkillNature.MovementBoth;
        Effect = SkillEffect.None;
        Race = CharacterRace.Human;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 2;
        Cooldown = 0;
        PaNeeded = 2;
        MinRange = 1;
        MaxRange = 2;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,1, 0,2, 1,1, 1,0, 2,0, 1,-1, 0,-1, 0,-2, -1,-1, -1,0, -2,0, -1,1 };
    }

    public override void OnClick()
    {
        if (!IsUnderCooldown())
            GridBhv.ShowSkillRange(RangeType, CharacterBhv, Id, OpponentBhvs, true);
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        CharacterBhv.MoveToPosition(x, y, false);

    }
}
