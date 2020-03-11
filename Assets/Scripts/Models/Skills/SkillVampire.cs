using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillVampire : Skill
{
    public SkillVampire()
    {
        Name = SkillsData.GoblinSkillsNames[0];
        Type = SkillType.Racial;
        Nature = SkillNature.Buff;
        Effect = SkillEffect.None;
        Race = CharacterRace.Gobelin;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Passive;
        CooldownMax = 0;
        Cooldown = 0;
        PaNeeded = 0;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.NoRange;
        IconId = 2;
        BasePrice = 100;

        Description = "Steal <material=\"LongRed\">" + _percentToSteal + "%</material> of the damages done as <material=\"LongRed\">HP</material>";
    }

    private int _percentToSteal = 20;

    public override void OnEndAttack(int damages, CharacterBhv opponentBhv)
    {
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            float damagesToSteal = damages * Helper.MultiplierFromPercent(0, _percentToSteal);
            CharacterBhv.GainHp((int)damagesToSteal);
            //CharacterBhv.GainHp((int)damagesToSteal);
            //CharacterBhv.GainHp((int)damagesToSteal);
            //CharacterBhv.GainHp((int)damagesToSteal);
            //CharacterBhv.GainHp((int)damagesToSteal);
            //CharacterBhv.GainHp((int)damagesToSteal);
            //CharacterBhv.GainHp((int)damagesToSteal);
            return true;
        }));
    }
}
