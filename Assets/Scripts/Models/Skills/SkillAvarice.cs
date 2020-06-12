using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAvarice : Skill
{
    public SkillAvarice()
    {
        Name = SkillsData.GoblinSkillsNames[1];
        Type = SkillType.Racial;
        Nature = SkillNature.Buff;
        Effect = SkillEffect.None;
        Race = CharacterRace.Gobelin;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Passive;
        CooldownMax = 0;
        PaNeeded = 0;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.NoRange;
        IconId = 3;
        EffectId = 3;
        BasePrice = 100;

        Description = "Steal <material=\"LongGold\">" + _percentToSteal + "%</material> of your current <material=\"LongGold\">Gold</material> amount on each hit";
    }

    private int _percentToSteal = 5;

    public override void OnEndAttack(int damages, CharacterBhv opponentBhv)
    {
        CharacterBhv.Instantiator.NewEffect(InventoryItemType.Skill, CharacterBhv.transform.position, null, EffectId, Constants.GridMax - CharacterBhv.Y);
        float goldToSteal = CharacterBhv.Character.Gold * Helper.MultiplierFromPercent(0, _percentToSteal);
        CharacterBhv.Character.GainGold((int)goldToSteal);
    }
}
