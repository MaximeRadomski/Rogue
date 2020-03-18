using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShield : Skill
{
    public SkillShield()
    {
        Name = SkillsData.HumanSkillsNames[0];
        Type = SkillType.Racial;
        Nature = SkillNature.Defensive;
        Effect = SkillEffect.Immuned;
        Race = CharacterRace.Human;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 2;
        EffectDurationMax = 1;
        PaNeeded = 2;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,0 };
        IconId = 0;
        EffectId = 1;
        BasePrice = 100;

        Description = "Block the first next turn hit";
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            AfterActivation();
            return true;
        }));
    }

    public override int OnTakeDamage(int damages)
    {
        if (IsApplyingEffect())
        {
            EffectDuration = 0;
            CharacterBhv.Instantiator.NewEffect(InventoryItemType.Skill, CharacterBhv.transform.position, null, EffectId, Constants.GridMax - CharacterBhv.Y);
            CharacterBhv.LoseSkillEffect(Effect);
            return 0;
        }
        return damages;

    }
}
