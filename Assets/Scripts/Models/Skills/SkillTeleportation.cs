using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTeleportation : Skill
{
    public SkillTeleportation()
    {
        Name = SkillsData.MagicalSkillsNames[2];
        Type = SkillType.NotRatial;
        Nature = SkillNature.Movement;
        Effect = SkillEffect.None;
        Rarity = Rarity.Rare;
        CooldownType = CooldownType.Normal;
        CooldownMax = 5;
        PaNeeded = 4;
        MinRange = 99;
        MaxRange = 99;
        RangeType = RangeType.FullRange;
        IconId = 10;
        BasePrice = 150;

        Description = "Teleport on any cell on the map";
    }

    public override void OnClick()
    {
        if (!IsUnderCooldown())
            GridBhv.ShowSkillRange(RangeType, CharacterBhv, Id, OpponentBhvs, true);
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {

            CharacterBhv.MoveToPosition(x, y, false);
            return true;
        }));
    }
}
