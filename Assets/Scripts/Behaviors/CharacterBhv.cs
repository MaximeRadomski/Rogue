using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBhv : MonoBehaviour
{
    public int OrderId;
    public int Initiative;
    public int X;
    public int Y;
    public bool IsMoving = false;
    public bool IsMovingFirstPathStep = false;
    public int IsAttacking = 0;
    public int Turn = 0;
    public int Pm;
    public int Pa;
    public Character Character;
    public AiBhv Ai;
    public Instantiator Instantiator;
    public List<CharacterBhv> OpponentBhvs;
    public List<SkillEffect> UnderEffects;

    private FightSceneBhv _fightSceneBhv;
    private GridBhv _gridBhv;
    private int _cellToReachX;
    private int _cellToReachY;
    private List<Vector2> _pathfindingSteps = new List<Vector2>();
    private List<RangePos> _pathfindingPos = new List<RangePos>();
    private SkinContainerBhv _skinContainer;
    private CharacterBhv _attackedOpponent;
    private OrbBhv _orbPa;
    private OrbBhv _orbPm;

    public delegate void ActionDelegate();
    public ActionDelegate AfterMouvementDelegate;

    public void SetPrivates()
    {
        _fightSceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>();
        _gridBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GridBhv>();
        OpponentBhvs = new List<CharacterBhv>();
        if (Character.IsPlayer)
        {
            int nbOpponents = PlayerPrefs.GetInt(Constants.PpNbOpponents);
            for (int i = 0; i < nbOpponents; ++i)
            {
                OpponentBhvs.Add(GameObject.Find(Constants.GoOpponentName + i).GetComponent<CharacterBhv>());
            }
        }
        else
        {
            OpponentBhvs.Add(GameObject.Find(Constants.GoPlayerName).GetComponent<CharacterBhv>());
            Ai = gameObject.AddComponent<AiBhv>();
            Ai.SetPrivates();
        }
            
        Instantiator = _fightSceneBhv.Instantiator;
        _skinContainer = transform.Find("SkinContainer").GetComponent<SkinContainerBhv>();
        for (int i = 0; i < Character.Skills.Count; ++i)
        {
            if (Character.Skills[i] != null)
                Character.Skills[i].Init(this, OpponentBhvs, _gridBhv, i);
        }
        if (Character.IsPlayer)
        {
            _orbPa = GameObject.Find("Pa")?.GetComponent<OrbBhv>();
            _orbPm = GameObject.Find("Pm")?.GetComponent<OrbBhv>();
        }
    }

    void Update()
    {
        if (IsMoving)
            Move();
        else if (IsMovingFirstPathStep)
            MoveToFirstPathStep();
        if (IsAttacking > 0)
            Attack();
    }

    public void TakeDamages(int damages)
    {
        Instantiator.PopText("-" + Character.TakeDamages(damages).ToString(), transform.position, TextType.Hp);
    }

    public void GainHp(int amount)
    {
        Instantiator.PopText("+" + Character.GainHp(amount).ToString(), transform.position, TextType.Hp);
    }

    public void LosePa(int amount)
    {
        int amountToRemove = amount;
        if (Pa - amountToRemove < 0)
            amountToRemove = Pa;
        Pa -= amountToRemove;
        _orbPa?.UpdateContent(Pa, Character.PaMax, Instantiator, TextType.Pa, -amountToRemove);
        Instantiator.PopText("-" + amountToRemove.ToString(), transform.position, TextType.Pa);
    }

    public void LosePm(int amount)
    {
        int amountToRemove = amount;
        if (Pm - amountToRemove < 0)
            amountToRemove = Pm;
        Pm -= amountToRemove;
        _orbPm?.UpdateContent(Pm, Character.PmMax, Instantiator, TextType.Pm, -amountToRemove);
        Instantiator.PopText("-" + amountToRemove.ToString(), transform.position, TextType.Pm);
    }

    public void GainSkillEffect(SkillEffect effect)
    {
        if (UnderEffects == null)
            UnderEffects = new List<SkillEffect>();
        UnderEffects.Add(effect);
    }

    public void LoseSkillEffect(SkillEffect effect)
    {
        if (UnderEffects == null)
            UnderEffects = new List<SkillEffect>();
        if (UnderEffects.Contains(effect))
            UnderEffects.Remove(effect);
    }

    public void ClearAllSkillEffects(bool isDebuff = false)
    {
        if (UnderEffects == null)
            UnderEffects = new List<SkillEffect>();
        UnderEffects.Clear();
        if (isDebuff)
        {
            foreach (var skill in Character.Skills)
            {
                skill.Debuff();
            }
        }
    }

    public int AttackWithWeapon(int weaponId, CharacterBhv opponentBhv, Map map, bool usePa = true)
    {
        var tmpWeapon = Character.Weapons[weaponId];

        float baseDamages = tmpWeapon.BaseDamage * Helper.MultiplierFromPercent(1, Random.Range(-tmpWeapon.DamageRangePercentage, tmpWeapon.DamageRangePercentage + 1));
        baseDamages *= Character.GetDamageMultiplier();

        float weaponHandlingMultiplier = 1.0f;
        if (tmpWeapon.Type != Character.FavWeapons[0] && tmpWeapon.Type != Character.FavWeapons[1])
            weaponHandlingMultiplier = Helper.MultiplierFromPercent(weaponHandlingMultiplier, - (RacesData.NotRaceWeaponDamagePercent + Character.NotRaceWeaponDamagePercent));
        else
            weaponHandlingMultiplier = Helper.MultiplierFromPercent(weaponHandlingMultiplier, Character.RaceWeaponDamagePercent);

        float raceGenderMultiplier = 1.0f;
        if (opponentBhv != null && opponentBhv.Character.Race == Character.StrongAgainst)
            raceGenderMultiplier = Helper.MultiplierFromPercent(raceGenderMultiplier, RacesData.StrongAgainstDamagePercent);
        if (map?.Type == Character.StrongIn)
            raceGenderMultiplier = Helper.MultiplierFromPercent(raceGenderMultiplier, RacesData.StrongInDamagePercent);
        if (Character.Gender == CharacterGender.Female)
            raceGenderMultiplier = Helper.MultiplierFromPercent(raceGenderMultiplier, -RacesData.GenderDamage);
        else
            raceGenderMultiplier = Helper.MultiplierFromPercent(raceGenderMultiplier, RacesData.GenderDamage);

        float skillMultiplier = 1.0f;
        foreach (var skill in Character.Skills)
        {
            if (skill != null)
                skillMultiplier = Helper.MultiplierFromPercent(skillMultiplier, skill.OnStartAttack());
        }

        float criticalMultiplier = 1.0f;
        int criticalPercent = Random.Range(0, 100);
        if (criticalPercent < tmpWeapon.CritChancePercent)
        {
            criticalMultiplier = Helper.MultiplierFromPercent(raceGenderMultiplier, tmpWeapon.CritMultiplierPercent);
            if (Character.Gender == CharacterGender.Male)
                criticalMultiplier = Helper.MultiplierFromPercent(criticalMultiplier, -RacesData.GenderCritical);
            else
                criticalMultiplier = Helper.MultiplierFromPercent(criticalMultiplier, RacesData.GenderCritical);
        }

        int resultInt = (int)(baseDamages * weaponHandlingMultiplier * raceGenderMultiplier * skillMultiplier * criticalMultiplier);
        //Debug.Log("Final Damages = " + resultInt);
        if (usePa)
        {
            LosePa(tmpWeapon.PaNeeded);
            IsAttacking = 1;
            _attackedOpponent = opponentBhv;
        }
        foreach (var skill in Character.Skills)
        {
            if (skill != null)
                skill.OnEndAttack(resultInt, opponentBhv);
        }
        return resultInt;
    }

    private void Attack()
    {
        if (_attackedOpponent == null)
        {
            IsAttacking = 0;
        }

        if (IsAttacking == 1)
            transform.position = Vector2.Lerp(transform.position, _attackedOpponent.transform.position, 0.2f);
        else
            transform.position = Vector2.Lerp(transform.position, _gridBhv.Cells[X, Y].transform.position, 0.7f);
        if (IsAttacking == 1 && Vector2.Distance(_gridBhv.Cells[X, Y].transform.position, transform.position) > 0.1f)
        {
            IsAttacking = 2;
        }
        else if (IsAttacking == 2 && (Vector2)transform.position == (Vector2)_gridBhv.Cells[X, Y].transform.position)
        {
            IsAttacking = 0;
            if (!Character.IsPlayer)
                Ai.AfterAction();
        }
    }

    public void MoveToPosition(int x, int y, bool usePm = true)
    {
        _cellToReachX = x;
        _cellToReachY = y;
        if (usePm)
            SetPath(x, y);
        else
        {
            _pathfindingSteps.Clear();
            _pathfindingPos.Clear();
            _pathfindingSteps.Add(_gridBhv.Cells[_cellToReachX, _cellToReachY].transform.position);
            _pathfindingPos.Add(new RangePos(_cellToReachX, _cellToReachY));
        }
        IsMoving = true;
    }

    public void SetPath(int xToReach, int yToReach, bool usePm = true)
    {
        _pathfindingSteps.Clear();
        _pathfindingPos.Clear();
        _pathfindingSteps.Add(_gridBhv.Cells[xToReach, yToReach].transform.position);
        _pathfindingPos.Add(new RangePos(xToReach, yToReach));
        var visitedIndex = _gridBhv.Cells[xToReach, yToReach].GetComponent<CellBhv>().Visited;
        if (usePm)
            LosePm(visitedIndex);
        int x = xToReach;
        int y = yToReach;
        while (visitedIndex > 0)
        {
            if (LookForLowerIndex(x, y + 1, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x, y + 1, OpponentBhvs))
                ++y;
            else if (LookForLowerIndex(x + 1, y, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x + 1, y, OpponentBhvs))
                ++x;
            else if (LookForLowerIndex(x, y - 1, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x, y - 1, OpponentBhvs))
                --y;
            else if (LookForLowerIndex(x - 1, y, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x - 1, y, OpponentBhvs))
                --x;
            _pathfindingSteps.Insert(0, _gridBhv.Cells[x, y].transform.position);
            _pathfindingPos.Insert(0, new RangePos(x, y));
            --visitedIndex;
        }
    }

    private bool LookForLowerIndex(int x, int y, int visitedIndex)
    {
        if (!Helper.IsPosValid(x, y))
            return false;
        if (_gridBhv.Cells[x, y].GetComponent<CellBhv>().Visited == visitedIndex)
            return true;
        return false;
    }

    public void MoveToFirstPathStep()
    {
        IsMovingFirstPathStep = true;
        transform.position = Vector2.Lerp(transform.position, _pathfindingSteps[0], 0.7f);
        if ((Vector2)transform.position == _pathfindingSteps[0])
        {
            _skinContainer.SetSkinContainerSortingLayerOrder(Constants.GridMax - _pathfindingPos[0].Y);
            IsMovingFirstPathStep = false;
            X = _pathfindingPos[0].X;
            Y = _pathfindingPos[0].Y;
            if (!Character.IsPlayer)
                Ai.AfterMovement();
        }
    }

    public void Move()
    {
        transform.position = Vector2.Lerp(transform.position, _pathfindingSteps[0], 0.7f);
        if ((Vector2)transform.position == _pathfindingSteps[0])
        {
            _skinContainer.SetSkinContainerSortingLayerOrder(Constants.GridMax - _pathfindingPos[0].Y);
            _pathfindingPos.RemoveAt(0);
            _pathfindingSteps.RemoveAt(0);
            if (_pathfindingSteps.Count == 0)
            {
                IsMoving = false;
                X = _cellToReachX;
                Y = _cellToReachY;
                if (Character.IsPlayer)
                    _fightSceneBhv.AfterPlayerMovement();
                else if (!Character.IsPlayer)
                    Ai.AfterMovement();
                if (AfterMouvementDelegate != null)
                    AfterMouvementDelegate();
            }
        }
    }

    public void Spawn(int x, int y)
    {
        X = x;
        Y = y;
        _skinContainer.SetSkinContainerSortingLayerOrder(Constants.GridMax - y);
        transform.position = _gridBhv.Cells[x, y].transform.position;
    }
}
