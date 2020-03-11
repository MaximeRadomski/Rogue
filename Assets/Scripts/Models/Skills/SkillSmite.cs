using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSmite : Skill
{
    public SkillSmite()
    {
        Name = SkillsData.MagicalSkillsNames[0];
        Type = SkillType.NotRatial;
        Nature = SkillNature.Offensive;
        Effect = SkillEffect.None;
        Rarity = Rarity.Magical;
        CooldownType = CooldownType.Normal;
        CooldownMax = 5;
        Cooldown = 0;
        PaNeeded = 7;
        MinRange = 3;
        MaxRange = 5;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,3, 0,4, 0,5,
                                         1,2, 1,3, 1,4, 2,2, 2,3, 3,2, 2,1, 3,1, 4,1,
                                         3,0, 4,0, 5,0,
                                         2,-1, 3,-1, 4,-1, -2,2, -2,3, -3,2, 1,-2, 1,-3, 1,-4,
                                         0,-3, 0,-4, 0,-5,
                                         -1,-2, -1,-3, -1,-4, -2,-2, -2,-3, -3,-2, -2,-1, -3,-1, -4,-1,
                                         -3,0, -4,0, -5,0,
                                         -2,1, -3,1, -4,1, 2,-2, 2,-3, 3,-2, -1,2, -1,3, -1,4, };
        IconId = 14;
        BasePrice = 250;

        Description = "Deal <material=\"LongRed\">100 HP</material> + Character Leveling Damages Percent per user levels";
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            Smite(GridBhv.IsOpponentOnCell(x, y));
            AfterActivation();
            return true;
        }));
    }

    public void Smite(CharacterBhv smitedOpponentBhv)
    {
        if (smitedOpponentBhv == null)
            return;
        var floatAmount = 100.0f * CharacterBhv.Character.GetDamageMultiplier();
        smitedOpponentBhv.TakeDamages(new Damage((int)floatAmount));
    }
}
