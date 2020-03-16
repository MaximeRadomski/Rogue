using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHeal : Skill
{
    public SkillHeal()
    {
        Name = SkillsData.NormalSkillsNames[2];
        Type = SkillType.NotRatial;
        Nature = SkillNature.Defensive;
        Effect = SkillEffect.None;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 3;
        PaNeeded = 3;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,0 };
        IconId = 12;
        EffectId = 3;
        BasePrice = 100;

        Description = "Heal the user for <material=\"LongRed\">50 HP</material> + 10% per user levels";
    }

    private float _floatHealAmount;

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            CharacterBhv.Instantiator.NewEffect(InventoryItemType.Skill, CharacterBhv.transform.position, null, EffectId, Constants.GridMax - CharacterBhv.Y);
            _floatHealAmount = 50.0f * Helper.MultiplierFromPercent(1, 10 * (CharacterBhv.Character.Level - 1));
            CharacterBhv.GainHp((int)_floatHealAmount);
            AfterActivation();
            return true;
        }));
        
    }
}
