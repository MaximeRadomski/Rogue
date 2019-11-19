using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBhv : MonoBehaviour
{
    private CharacterBhv _characterBhv;
    private CharacterBhv _opponentBhv;
    private GridBhv _gridBhv;
    private FightSceneBhv _fightSceneBhv;

    private List<int> _weights;
    private int _attackWeight;
    private int _defenseWeight;
    private int _buffWeight;
    private int[] _weaponsWeight = { 0, 0 };
    private int[] _skillsWeight = { 0, 0 };
    private int _getCloseWeight;
    private int _getFarWeight;

    private int _nbCellsWalked;


    #region Init

    public void SetPrivates()
    {
        _characterBhv = GetComponent<CharacterBhv>();
        _opponentBhv = _characterBhv.OpponentBhvs[0];
        _gridBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GridBhv>();
        _fightSceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>();
    }

    private void ResetWeight()
    {
        if (_weights != null)
            _weights.Clear();
        _attackWeight = 0;
        _defenseWeight = 0;
        _buffWeight = 0;
        _getCloseWeight = 0;
        _getFarWeight = 0;
        _weaponsWeight[0] = 0;
        _weaponsWeight[1] = 0;
        _skillsWeight[0] = 0;
        _skillsWeight[1] = 0;
    }

    #endregion

    public void StartThinking()
    {
        _nbCellsWalked = 0;
        Think();
    }
    private void Think() //What do I do
    {
        ResetWeight();
        SetAttackWeight();
        SetDefenseWeight();
        SetBuffWeight();
        SetGetCloseWeight();
        SetGetFarWeight();

        PopulateWeights();
        //Action Weight
        //In the right Order
        if (IsTheBiggest(_buffWeight))
        {

        }
        else if (IsTheBiggest(_attackWeight))
        {

        }
        else if (IsTheBiggest(_defenseWeight))
        {

        }
        //No Action Weight over 0
        //Now processing to movement
        bool canThinkAgain;
        if (_getFarWeight >= _getCloseWeight
            && _getFarWeight != 0)
        {
            SetPathToOppositeCorner();
            canThinkAgain = MoveToNextCell();
        }
        else if (_getCloseWeight > _getFarWeight)
        {
            SetPathToOpponent();
            canThinkAgain = MoveToNextCell();
        }
        else
        {
            //No Movement over another
            //Nothing more to do
            canThinkAgain = false;
        }
        if (!canThinkAgain)
        _fightSceneBhv.PassTurn();
    }

    private void PopulateWeights()
    {
        if (_weights == null)
            _weights = new List<int>();
        _weights.Add(_buffWeight);
        _weights.Add(_attackWeight);
        _weights.Add(_defenseWeight);
    }

    private bool IsTheBiggest(int thisWeight)
    {
        int tmpBiggest = 0;
        foreach (var weight in _weights)
        {
            if (weight > tmpBiggest)
                tmpBiggest = weight;
        }
        if (tmpBiggest == 0)
            return false;
        if (thisWeight >= tmpBiggest)
            return true;
        return false;
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
        _attackWeight = canIWeapon + shouldIWeapon + canISkill + shouldISkill;
    }

    private int CanIWeaponThePlayer()
    {
        int canI = 0;
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Pa >= _characterBhv.Character.Weapons[i].PaNeeded &&
            _gridBhv.IsOpponentInWeaponRangeAndZone(_characterBhv, i, _characterBhv.OpponentBhvs))
            {
                _weaponsWeight[i] += 10;
                canI += _weaponsWeight[i];
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
                _skillsWeight[i] += 10;
                canI += _skillsWeight[i];
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
            if (_weaponsWeight[i] <= 0)
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
        _defenseWeight = canI + shouldI;
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
                    _skillsWeight[i] += 100;
                else if (_characterBhv.Character.Skills[i].Nature == SkillNature.Defensive)
                    _skillsWeight[i] += 10;
                canI += _skillsWeight[i];
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
        _buffWeight = canI + shouldI;
    }

    private int CanIBuffMyself()
    {
        int canI = 0;
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Character.Skills[i].Cooldown == 0 &&
                _characterBhv.Pa >= _characterBhv.Character.Skills[i].PaNeeded)
            {
                if (_characterBhv.Character.Skills[i].Nature == SkillNature.Buff)
                    _skillsWeight[i] += 50;
                canI += _skillsWeight[i];
            }
        }
        return canI;
    }

    private int ShouldIBuffMyself()
    {
        int shouldI = 0;
        return shouldI;
    }

    #endregion

    #region Movement

    private void SetPathToOpponent()
    {
        _gridBhv.ShowPm(_characterBhv, _characterBhv.OpponentBhvs, unlimitedPm:true);
        _characterBhv.SetPath(_opponentBhv.X, _opponentBhv.Y, usePm:false);
    }

    private void SetPathToOppositeCorner()
    {
        int x = 0;
        int y = 0;
        if (_opponentBhv.X < 3)
            x = Constants.GridMax - 1;
        if (_opponentBhv.Y <= 3)
            y = Constants.GridMax - 1;
        _gridBhv.ShowPm(_characterBhv, _characterBhv.OpponentBhvs, unlimitedPm: true);
        _characterBhv.SetPath(x, y, usePm: false);
    }

    private bool MoveToNextCell()
    {
        if (_gridBhv.IsAdjacentOpponent(_characterBhv.X, _characterBhv.Y, _characterBhv.OpponentBhvs))
            return false;
        _characterBhv.MoveToFirstPathStep();
        return true;
    }

    public void AfterMovement()
    {
        ++_nbCellsWalked;
        Think();
    }

    #endregion

    #region GetClose

    private void SetGetCloseWeight()
    {
        int canI;
        int shouldI = 0;

        canI = CanIGetClose();
        if (canI > 0)
            shouldI = ShouldIGetClose();
        _getCloseWeight = canI + shouldI;
    }

    private int CanIGetClose()
    {
        int canI = _characterBhv.Pm * 10;
        return canI;
    }

    private int ShouldIGetClose()
    {
        int shouldI = 0;
        if (_characterBhv.Character.Hp == _characterBhv.Character.HpMax)
            shouldI += 20;
        return shouldI;
    }

    #endregion

    #region GetFar

    private void SetGetFarWeight()
    {
        int canI;
        int shouldI = 0;

        canI = CanIGetFar();
        if (canI > 0)
            shouldI = ShouldIGetFar();
        _getFarWeight = canI + shouldI;
    }

    private int CanIGetFar()
    {
        int canI = _characterBhv.Pm * 10;
        if (_gridBhv.IsAdjacentOpponent(_characterBhv.X, _characterBhv.Y, _characterBhv.OpponentBhvs))
            canI -= 1000;
        return canI;
    }

    private int ShouldIGetFar()
    {
        int shouldI = 0;
        if (_characterBhv.Character.Hp <= _characterBhv.Character.HpMax)
            shouldI += 20;
        return shouldI;
    }

    #endregion
}
