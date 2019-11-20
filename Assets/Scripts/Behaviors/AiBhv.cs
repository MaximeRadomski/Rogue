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

        NewWeights(new List<int>() { _buffWeight, _attackWeight, _defenseWeight });
        //Action Weight
        //In the right Order
        bool actionResult = false;
        if (IsTheBiggest(_buffWeight))
            actionResult = Buff();
        else if (IsTheBiggest(_attackWeight))
            actionResult = Attack();
        else if (IsTheBiggest(_defenseWeight))
            actionResult = Defend();
        if (actionResult)
            return;
        //No Action Weight over 0
        //Now processing to movement
        bool moveResult = false;
        if (_getFarWeight > 0 && _getFarWeight > _getCloseWeight)
            moveResult = GetFar();
        else if (_getCloseWeight > 0 && _getCloseWeight > _getFarWeight)
            moveResult = GetClose();
        if (moveResult)
            return;
        _fightSceneBhv.PassTurn();
    }

    private void NewWeights(List<int> tmpList)
    {
        if (_weights == null)
            _weights = new List<int>();
        _weights.Clear();
        _weights = tmpList;
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
            if (!_characterBhv.Character.Skills[i].IsUnderCooldown() &&
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
            float AttackRatioHp = (float)_characterBhv.AttackWithWeapon(i, _opponentBhv, _gridBhv.Map, usePa:false) / _opponentBhv.Character.Hp;
            float AttackRatioMaxHp = (float)_characterBhv.AttackWithWeapon(i, _opponentBhv, _gridBhv.Map, usePa: false) / _opponentBhv.Character.HpMax;
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

    private bool Attack()
    {
        NewWeights(new List<int>() { _weaponsWeight[0], _weaponsWeight[1] });
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Character.Skills[i].Nature == SkillNature.Offensive)
                _weights.Add(_skillsWeight[i]);
        }
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Character.Skills[i].Nature == SkillNature.Offensive && IsTheBiggest(_skillsWeight[i]) && _skillsWeight[i] > 0)
            {
                _characterBhv.Character.Skills[i].Activate(_opponentBhv.X, _opponentBhv.Y);
                return true;
            }
        }
        for (int i = 0; i < 2; ++i)
        {
            if (IsTheBiggest(_weaponsWeight[i]) && _weaponsWeight[i] > 0)
            {
                _opponentBhv.TakeDamages(_characterBhv.AttackWithWeapon(i, _opponentBhv, _gridBhv.Map));
                return true;
            }
        }
        return false;
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
            if (!_characterBhv.Character.Skills[i].IsUnderCooldown() &&
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

    private bool Defend()
    {
        NewWeights(new List<int>());
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Character.Skills[i].Nature == SkillNature.Defensive)
                _weights.Add(_skillsWeight[i]);
        }
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Character.Skills[i].Nature == SkillNature.Defensive && IsTheBiggest(_skillsWeight[i]) && _skillsWeight[i] > 0)
            {
                _characterBhv.Character.Skills[i].Activate(_characterBhv.X, _characterBhv.Y);
                return true;
            }
        }
        return false;
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
            if (!_characterBhv.Character.Skills[i].IsUnderCooldown() &&
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

    private bool Buff()
    {
        NewWeights(new List<int>());
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Character.Skills[i].Nature == SkillNature.Buff)
                _weights.Add(_skillsWeight[i]);
        }
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Character.Skills[i].Nature == SkillNature.Buff && IsTheBiggest(_skillsWeight[i]) && _skillsWeight[i] > 0)
            {
                _characterBhv.Character.Skills[i].Activate(_characterBhv.X, _characterBhv.Y);
                return true;
            }
        }
        return false;
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

    private RangePos GetClosestSkillCellToPlayer()
    {
        float minDistance = float.PositiveInfinity;
        RangePos tmpPos = new RangePos(0,0);
        foreach (var cell in _gridBhv.Cells)
        {
            if (cell.GetComponent<CellBhv>().Visited == Constants.VisitedSkillValue)
            {
                float tmpDistance = Vector2.Distance(cell.transform.position, _opponentBhv.transform.position);
                if (tmpDistance < minDistance)
                {
                    minDistance = tmpDistance;
                    tmpPos.X = cell.GetComponent<CellBhv>().X;
                    tmpPos.Y = cell.GetComponent<CellBhv>().Y;
                }
            }
        }
        return tmpPos;
    }

    private RangePos GetFarestSkillCellToPlayer()
    {
        float maxDistance = 0.0f;
        RangePos tmpPos = new RangePos(0, 0);
        foreach (var cell in _gridBhv.Cells)
        {
            if (cell.GetComponent<CellBhv>().Visited == Constants.VisitedSkillValue)
            {
                float tmpDistance = Vector2.Distance(cell.transform.position, _opponentBhv.transform.position);
                if (tmpDistance > maxDistance)
                {
                    maxDistance = tmpDistance;
                    tmpPos.X = cell.GetComponent<CellBhv>().X;
                    tmpPos.Y = cell.GetComponent<CellBhv>().Y;
                }
            }
        }
        return tmpPos;
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

    public void AfterAction()
    {
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
        for (int i = 0; i < 2; ++i)
        {
            if (!_characterBhv.Character.Skills[i].IsUnderCooldown() &&
                _characterBhv.Pa >= _characterBhv.Character.Skills[i].PaNeeded)
            {
                if (_characterBhv.Character.Skills[i].Nature == SkillNature.Movement)
                    _skillsWeight[i] += 50;
                canI += _skillsWeight[i];
            }
        }
        if (_gridBhv.IsAdjacentOpponent(_characterBhv.X, _characterBhv.Y, _characterBhv.OpponentBhvs))
            canI -= 1000;
        return canI;
    }

    private int ShouldIGetClose()
    {
        int shouldI = 0;
        if (_characterBhv.Character.Hp == _characterBhv.Character.HpMax)
            shouldI += 20;
        return shouldI;
    }

    private bool GetClose()
    {
        SetPathToOpponent();
        if (_gridBhv.Cells[_opponentBhv.X, _opponentBhv.Y].GetComponent<CellBhv>().Visited > _characterBhv.Pm + 1)
        {
            NewWeights(new List<int>());
            for (int i = 0; i < 2; ++i)
            {
                if (_characterBhv.Character.Skills[i].Nature == SkillNature.Movement)
                    _weights.Add(_skillsWeight[i]);
            }
            for (int i = 0; i < 2; ++i)
            {
                if (_characterBhv.Character.Skills[i].Nature == SkillNature.Movement && IsTheBiggest(_skillsWeight[i]))
                {
                    _gridBhv.ShowSkillRange(_characterBhv.Character.Skills[i].RangeType, _characterBhv, i, _characterBhv.OpponentBhvs, true);
                    var tmpPos = GetClosestSkillCellToPlayer();
                    _characterBhv.Character.Skills[i].Activate(tmpPos.X, tmpPos.Y);
                    return true;
                }
            }
        }
        return MoveToNextCell();
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
        int nbSkillMovement = 0;
        for (int i = 0; i < 2; ++i)
        {
            if (!_characterBhv.Character.Skills[i].IsUnderCooldown() &&
                _characterBhv.Pa >= _characterBhv.Character.Skills[i].PaNeeded)
            {
                if (_characterBhv.Character.Skills[i].Nature == SkillNature.Movement)
                {
                    _skillsWeight[i] += 50;
                    ++nbSkillMovement;
                }
                canI += _skillsWeight[i];
            }
        }
        if (nbSkillMovement == 0 && _gridBhv.IsAdjacentOpponent(_characterBhv.X, _characterBhv.Y, _characterBhv.OpponentBhvs))
            canI -= 1000;
        return canI;
    }

    private int ShouldIGetFar()
    {
        int shouldI = 0;
        if (_characterBhv.Character.Hp <= _characterBhv.Character.HpMax)
            shouldI += 20;
        for (int i = 0; i < 2; ++i)
        {
            if (_characterBhv.Character.Weapons[i].Type == WeaponType.Bow &&
                !_gridBhv.IsAdjacentOpponent(_characterBhv.X, _characterBhv.Y, _characterBhv.OpponentBhvs))
            {
                shouldI += 30;
            }
        }
        return shouldI;
    }

    private bool GetFar()
    {
        SetPathToOpponent();
        if (_gridBhv.IsAdjacentOpponent(_characterBhv.X, _characterBhv.Y, _characterBhv.OpponentBhvs) || _characterBhv.Pm == 0)
        {
            NewWeights(new List<int>());
            for (int i = 0; i < 2; ++i)
            {
                if (_characterBhv.Character.Skills[i].Nature == SkillNature.Movement)
                    _weights.Add(_skillsWeight[i]);
            }
            for (int i = 0; i < 2; ++i)
            {
                if (_characterBhv.Character.Skills[i].Nature == SkillNature.Movement && IsTheBiggest(_skillsWeight[i]))
                {
                    _gridBhv.ShowSkillRange(_characterBhv.Character.Skills[i].RangeType, _characterBhv, i, _characterBhv.OpponentBhvs, true);
                    var tmpPos = GetFarestSkillCellToPlayer();
                    _characterBhv.Character.Skills[i].Activate(tmpPos.X, tmpPos.Y);
                    return true;
                }
            }
        }
        return MoveToNextCell();
    }

    #endregion
}
