using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGreatHeal : Skill
{
    public SkillGreatHeal()
    {
        Name = SkillsData.MagicalSkillsNames[1];
        Type = SkillType.NotRatial;
        Nature = SkillNature.Defensive;
        Effect = SkillEffect.None;
        Rarity = Rarity.Magical;
        CooldownType = CooldownType.Normal;
        CooldownMax = 6;
        PaNeeded = 5;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,0 };
        IconId = 15;
        EffectId = 3;
        BasePrice = 200;

        Description = "Heal the user for <material=\"LongRed\">100 HP</material> + 25% per user levels";
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            CharacterBhv.Instantiator.NewEffect(InventoryItemType.Skill, CharacterBhv.transform.position, null, EffectId, Constants.GridMax - CharacterBhv.Y);
            var floatAmount = 100.0f * Helper.MultiplierFromPercent(1, 25 * (CharacterBhv.Character.Level - 1));
            CharacterBhv.GainHp((int)floatAmount);
            AfterActivation();
            return true;
        }));
    }
}
