using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDash : Skill
{
    public SkillDash()
    {
        Name = SkillsData.ElfSkillsNames[0];
        Type = SkillType.Racial;
        Nature = SkillNature.Defensive;
        Effect = SkillEffect.Immuned;
        Race = CharacterRace.Elf;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 4;
        EffectDurationMax = 1;
        PaNeeded = 4;
        MinRange = 1;
        MaxRange = 2;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,1, 1,1, 1,0, 1,-1, 0,-1, -1,-1, -1,0, -1,1 };
        IconId = 4;
        EffectId = 2;
        BasePrice = 100;

        Description = "Avoid the first next turn hit (consume all your remaining mouvement points)";
    }

    private int _currentTargetX;
    private int _currentTargetY;

    public override void OnClick()
    {
        if (!IsUnderCooldown())
            GridBhv.ShowSkillRange(RangeType, CharacterBhv, Id, OpponentBhvs, true);
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        _currentTargetX = x;
        _currentTargetY = y;
        //CharacterBhv.GainSkillEffect(SkillEffect.Immuned);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            CharacterBhv.LosePm(CharacterBhv.Character.PmMax);
            AfterActivation();
            return true;
        }));
    }

    public override int OnTakeDamage(int damages)
    {
        if (IsApplyingEffect())
        {
            if (Helper.IsPosValid(_currentTargetX, _currentTargetX) || !GridBhv.IsOpponentOnCell(_currentTargetX, _currentTargetY, true))
                CharacterBhv.MoveToPosition(_currentTargetX, _currentTargetY, false);
            CharacterBhv.Instantiator.NewEffect(InventoryItemType.Skill, GridBhv.Cells[_currentTargetX, _currentTargetY].transform.position, null, EffectId, Constants.GridMax - _currentTargetY);
            EffectDuration = 0;
            CharacterBhv.LoseSkillEffect(Effect);
            return 0;
        }
        return damages;
    }
}
