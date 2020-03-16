using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : InventoryItem
{
    public SkillType Type;
    public SkillNature Nature;
    public SkillEffect Effect;
    public CharacterRace Race;
    public CooldownType CooldownType;
    public int CooldownMax;
    public int Cooldown;
    public int EffectDuration;
    public int EffectDurationMax;
    public int PaNeeded;
    public int MinRange;
    public int MaxRange;
    public RangeType RangeType;
    public List<int> RangePositions;
    public List<RangeDirection> RangeZones;
    public int IconId;
    public int EffectId;

    public CharacterBhv CharacterBhv;
    public List<CharacterBhv> OpponentBhvs;
    public GridBhv GridBhv;
    public int Id;

    private bool _isDebuffed;

    public Skill()
    {
        InventoryItemType = InventoryItemType.Skill;
        Weight = 1;
    }

    public bool IsApplyingEffect()
    {
        if (_isDebuffed || Effect == SkillEffect.None)
            return false;
        return EffectDuration > 0;
    }

    public bool ShouldLoseSkillEffect()
    {
        if (_isDebuffed || Effect == SkillEffect.None)
            return false;
        return EffectDuration == 0;
    }

    public bool IsUnderCooldown()
    {
        return Cooldown != 0;
    }

    public void Debuff()
    {
        _isDebuffed = true;
    }

    public virtual void Init(CharacterBhv characterBhv, List<CharacterBhv> opponentBhvs, GridBhv gridBhv, int id)
    {
        CharacterBhv = characterBhv;
        OpponentBhvs = opponentBhvs;
        GridBhv = gridBhv;
        Id = id;
    }

    public virtual void Activate(int x, int y)
    {
        Debug.Log("Activate:" + Name);
        CharacterBhv.Instantiator.PopIcon(Helper.GetSpriteFromSpriteSheet("Sprites/IconsSkill_" + IconId), CharacterBhv.transform.position);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            CharacterBhv.LosePa(PaNeeded);
            return true;
        }));
        _isDebuffed = false;
        if (CooldownType == CooldownType.Normal)
        {
            Cooldown = CooldownMax + 1;
            EffectDuration = EffectDurationMax;
            if (Effect != SkillEffect.None)
                CharacterBhv.GainSkillEffect(Effect);
        }            
        else if (CooldownType == CooldownType.OnceAFight)
        {
            Cooldown = -1;
            EffectDuration = EffectDurationMax;
            if (Effect != SkillEffect.None)
                CharacterBhv.GainSkillEffect(Effect);
        }
        CharacterBhv.SkinContainer.OrientToTarget(CharacterBhv.X - x);
        GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>().ManagePlayerButtons();
    }

    public void AfterActivation()
    {
        if (!CharacterBhv.Character.IsPlayer)
            CharacterBhv.Ai.AfterAction();
        else
            GridBhv.ShowPm(CharacterBhv, OpponentBhvs);
    }

    public virtual void Destruct()
    {
        
    }

    public virtual void OnClick()
    {
        if (RangeType != RangeType.NoRange && !IsUnderCooldown())
            GridBhv.ShowSkillRange(RangeType, CharacterBhv, Id, OpponentBhvs);
        else if (CooldownType == CooldownType.Passive)
            GridBhv.ShowPm(CharacterBhv, OpponentBhvs);

    }

    public virtual void OnStartTurn()
    {
        if ((CooldownType == CooldownType.Normal && IsUnderCooldown()) ||
            (CooldownType == CooldownType.OnceAFight && IsUnderCooldown()))
            --Cooldown;
        if (Effect != SkillEffect.None)
        {
            --EffectDuration;
            if (ShouldLoseSkillEffect())
                CharacterBhv.LoseSkillEffect(Effect);
        }
    }

    public virtual void OnEndTurn()
    {

    }

    public virtual int OnStartAttack()
    {
        return 0;
    }

    public virtual void OnEndAttack(int damages, CharacterBhv opponentBhv)
    {

    }

    public virtual int OnTakeDamage(int damages)
    {
        return damages;
    }
}
