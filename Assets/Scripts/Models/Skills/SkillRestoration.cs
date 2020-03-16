using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRestoration : Skill
{
    public SkillRestoration()
    {
        Name = SkillsData.OrcSkillsNames[1];
        Type = SkillType.Racial;
        Nature = SkillNature.Buff;
        Effect = SkillEffect.None;
        Race = CharacterRace.Orc;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Passive;
        CooldownMax = 0;
        PaNeeded = 0;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.NoRange;
        IconId = 9;
        EffectId = 3;
        BasePrice = 100;

        Description = "Restore <material=\"LongRed\">5%</material> of your maximum health each turn";
    }

    public override void OnStartTurn()
    {
        CharacterBhv.Instantiator.PopIcon(Helper.GetSpriteFromSpriteSheet("Sprites/IconsSkill_" + IconId), CharacterBhv.transform.position);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            CharacterBhv.Instantiator.NewEffect(InventoryItemType.Skill, CharacterBhv.transform.position, null, EffectId, Constants.GridMax - CharacterBhv.Y);
            float hpToRestore = (CharacterBhv.Character.HpMax * 0.05f);
            //hpToRestore *= Helper.MultiplierFromPercent(1.0f, Random.Range(0, 51));
            CharacterBhv.GainHp((int)hpToRestore);
            return true;
        }));
    }
}
