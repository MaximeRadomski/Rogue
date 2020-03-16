using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDoubleEdged : Skill
{
    public SkillDoubleEdged()
    {
        Name = SkillsData.ElfSkillsNames[1];
        Type = SkillType.Racial;
        Nature = SkillNature.Buff;
        Effect = SkillEffect.AttackUp;
        Race = CharacterRace.Elf;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.OnceAFight;
        CooldownMax = -1;
        EffectDurationMax = 1;
        PaNeeded = 2;
        MinRange = 0;
        MaxRange = 0;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,0 };
        IconId = 5;
        EffectId = 3;
        BasePrice = 100;

        Description = "Make and receive double damages until your next turn";
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            CharacterBhv.Instantiator.NewEffect(InventoryItemType.Skill, CharacterBhv.transform.position, null, EffectId, Constants.GridMax - CharacterBhv.Y);
            AfterActivation();
            return true;
        }));
    }

    public override int OnStartAttack()
    {
        if (IsApplyingEffect())
            return 100;
        return base.OnStartAttack();
    }

    public override int OnTakeDamage(int damages)
    {
        if (IsApplyingEffect())
            return damages * 2;
        return damages;

    }
}
