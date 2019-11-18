using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBhv : MonoBehaviour
{
    private CharacterBhv _characterBhv;
    private CharacterBhv _opponentBhv;
    private GridBhv _gridBhv;

    private int AttackWeight;
    private int DefenseWeight;
    private int BuffWeight;
    private int GetCloseWeight;
    private int GetFarWeight;
    private int[] WeaponsWeight = { 0, 0 };
    private int[] SkillsWeight = { 0, 0 };

    #region Init

    public void SetPrivates()
    {
        _characterBhv = GetComponent<CharacterBhv>();
        _opponentBhv = _characterBhv.OpponentBhvs[0];
        _gridBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GridBhv>();
    }

    private void ResetWeight()
    {
        AttackWeight = 0;
        DefenseWeight = 0;
        BuffWeight = 0;
        GetCloseWeight = 0;
        GetFarWeight = 0;
        WeaponsWeight[0] = 0;
        WeaponsWeight[1] = 0;
        SkillsWeight[0] = 0;
        SkillsWeight[1] = 0;
    }

    #endregion

    public void StartThinking()
    {
        Think();
    }
    private void Think() //What do I do
    {
        ResetWeight();
        SetAttackWeight();
        SetDefenseWeight();
        SetBuffWeight();
    }

    #region Attack

    private void SetAttackWeight()
    {
        int canIWeapon = 0;
        int canISkill = 0;
        int shouldIWeapon = 0;
        int shouldISkill = 0;

        canIWeapon += CanIWeaponThePlayer();
        canISkill += CanIAttackSkillThePlayer();
        if (canIWeapon > 0)
            shouldIWeapon += ShouldIWeaponThePlayer();
        if (canISkill > 0)
            shouldISkill += ShouldIAttackSkillThePlayer();
        AttackWeight = canIWeapon + shouldIWeapon + canISkill + shouldISkill;
    }

    private int CanIWeaponThePlayer()
    {
        int canI = 0;
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Pa >= _characterBhv.Character.Weapons[i].PaNeeded &&
            _gridBhv.IsOpponentInWeaponRangeAndZone(_characterBhv, i, _characterBhv.OpponentBhvs))
            {
                WeaponsWeight[i] += 10;
                canI += WeaponsWeight[i];
            }
        }
        return canI;
    }

    private int CanIAttackSkillThePlayer()
    {
        int canI = 0;
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Character.Skills[i].Cooldown == 0 &&
                _characterBhv.Pa >= _characterBhv.Character.Skills[i].PaNeeded &&
                _gridBhv.IsOpponentInSkillRange(_characterBhv, i, _characterBhv.OpponentBhvs))
            {
                SkillsWeight[i] += 10;
                canI += SkillsWeight[i];
            }
        }
        return canI;
    }

    private int ShouldIWeaponThePlayer()
    {
        int shouldI = 0;
        foreach (var skillEffect in _opponentBhv.UnderEffects.OrEmptyIfNull())
        {
            if (skillEffect == SkillEffect.Immuned)
                shouldI -= 100;
            else if (skillEffect == SkillEffect.DefenseUp)
                shouldI -= 10;
            else
                shouldI += 10;
        }
        for (int i = 0; i < 2; ++i)
        {
            if (WeaponsWeight[i] <= 0)
                continue;
            float AttackRatioHp = (float)_characterBhv.AttackWithWeapon(i, _opponentBhv, _gridBhv.Map) / _opponentBhv.Character.Hp;
            float AttackRatioMaxHp = (float)_characterBhv.AttackWithWeapon(i, _opponentBhv, _gridBhv.Map) / _opponentBhv.Character.HpMax;
            if (AttackRatioHp > 1.15f)
                shouldI += 100;
            else if (AttackRatioHp >= 1.0f)
                shouldI += 50;
            else if (AttackRatioMaxHp >= 0.5f)
                shouldI += 30;
            else if (AttackRatioMaxHp >= 0.33f)
                shouldI += 10;
        }
        return shouldI;
    }

    private int ShouldIAttackSkillThePlayer()
    {
        int shouldI = 0;
        foreach (var skillEffect in _opponentBhv.UnderEffects.OrEmptyIfNull())
        {
            if (skillEffect == SkillEffect.Immuned)
                shouldI -= 100;
            else if (skillEffect == SkillEffect.DefenseUp)
                shouldI -= 10;
            else
                shouldI += 10;
        }
        return shouldI;
    }

    #endregion

    #region Defense

    private void SetDefenseWeight()
    {
        int canI;
        int shouldI = 0;

        canI = CanIDefendMyself();
        if (canI > 0)
            shouldI = ShouldIDefendMyself();
        DefenseWeight = canI + shouldI;
    }

    private int CanIDefendMyself()
    {
        int canI = 0;
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Character.Skills[i].Cooldown == 0 &&
                _characterBhv.Pa >= _characterBhv.Character.Skills[i].PaNeeded)
            {
                if (_characterBhv.Character.Skills[i].Effect == SkillEffect.Immuned)
                    WeaponsWeight[i] += 100;
                else if (_characterBhv.Character.Skills[i].Nature == SkillNature.Defensive)
                    WeaponsWeight[i] += 10;
                canI += WeaponsWeight[i];
            }
        }
        return canI;
    }

    private int ShouldIDefendMyself()
    {
        int shouldI = 0;
        if (_characterBhv.Character.Hp < _characterBhv.Character.HpMax * 0.2f)
            shouldI += 50;
        else if (_characterBhv.Character.Hp < _characterBhv.Character.HpMax * 0.5f)
            shouldI += 30;
        else if (_characterBhv.Character.Hp == _characterBhv.Character.HpMax)
            shouldI -= 100;
        return shouldI;
    }

    #endregion

    #region Buff

    private void SetBuffWeight()
    {
        int canI;
        int shouldI = 0;

        canI = CanIBuffMyself();
        if (canI > 0)
            shouldI = ShouldIBuffMyself();
        DefenseWeight = canI + shouldI;
    }

    private int CanIBuffMyself()
    {
        int canI = 0;
        return canI;
    }

    private int ShouldIBuffMyself()
    {
        int shouldI = 0;
        return shouldI;
    }

    #endregion
}

/*


    What do I do ?
        Should I Skill ?
            Is my skill offensive ?
                Am I in the right position ?
            Is my skill defensive ?
            Is my skill movement ?
            Is my skill control ?
            Is my skill buff ?
            Is my skill debuff ?
        Should I attack ?
            Do I have enough Pa ?
            Do I reach the player ?
        Should I Get Close ?
        Should I Get Far ?


    */
