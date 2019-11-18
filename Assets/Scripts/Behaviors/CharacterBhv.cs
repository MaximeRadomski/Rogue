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
    public int Turn = 0;
    public int Pm;
    public int Pa;
    public Character Character;
    public bool IsPlayer = false;
    public Instantiator Instantiator;
    public List<CharacterBhv> OpponentBhvs;
    public List<SkillEffect> UnderEffects;

    private FightSceneBhv _fightSceneBhv;
    private GridBhv _gridBhv;
    private int _cellToReachX;
    private int _cellToReachY;
    private List<Vector2> _pathfindingSteps = new List<Vector2>();
    private List<RangePos> _pathfindingPos = new List<RangePos>();
    private SpriteRenderer _spriteRenderer;

    public delegate void ActionDelegate();
    public ActionDelegate AfterMouvementDelegate;

    public void SetPrivates()
    {
        _fightSceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>();
        _gridBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GridBhv>();
        OpponentBhvs = new List<CharacterBhv>();
        if (IsPlayer)
        {
            int nbOpponents = PlayerPrefs.GetInt(Constants.PpNbOpponents);
            for (int i = 0; i < nbOpponents; ++i)
            {
                OpponentBhvs.Add(GameObject.Find(Constants.GoOpponentName + i).GetComponent<CharacterBhv>());
            }
        }
        else
            OpponentBhvs.Add(GameObject.Find(Constants.GoPlayerName).GetComponent<CharacterBhv>());
        Instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<Instantiator>();
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        for (int i = 0; i < Character.Skills.Count; ++i)
        {
            if (Character.Skills[i] != null)
                Character.Skills[i].Init(this, OpponentBhvs, _gridBhv, i);
        }
    }

    void Update()
    {
        if (IsMoving)
            Move();
    }

    public void TakeDamages(int damages)
    {
        foreach (var skill in Character.Skills)
        {
            if (skill != null)
                damages = skill.OnTakeDamage(damages);
        }
        Character.Hp -= damages;
        Instantiator.PopText("-" + damages.ToString(), transform.position, TextType.Hp);
    }

    public void GainHp(int amount)
    {
        int amountToAdd = amount;
        if (Character.Hp + amountToAdd > Character.HpMax)
            amountToAdd = Character.HpMax - Character.Hp;
        Character.Hp += amountToAdd;
        Instantiator.PopText("+" + amountToAdd.ToString(), transform.position, TextType.Hp);
    }

    public void LosePa(int amount)
    {
        int amountToRemove = amount;
        if (Pa - amountToRemove < 0)
            amountToRemove = Pa;
        Pa -= amountToRemove;
        Instantiator.PopText("-" + amountToRemove.ToString(), transform.position, TextType.Pa);
    }

    public void LosePm(int amount)
    {
        int amountToRemove = amount;
        if (Pm - amountToRemove < 0)
            amountToRemove = Pm;
        Pm -= amountToRemove;
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

    public int AttackWithWeapon(int weaponId, CharacterBhv opponentBhv, Map map = null)
    {
        var tmpWeapon = Character.Weapons[weaponId];

        float baseDamages = tmpWeapon.BaseDamage * Helper.MultiplierFromPercent(1, Random.Range(-tmpWeapon.DamageRangePercentage, tmpWeapon.DamageRangePercentage + 1));
        if (tmpWeapon.Type != Character.FavWeapons[0] && tmpWeapon.Type != Character.FavWeapons[1])
            baseDamages = baseDamages * Helper.MultiplierFromPercent(0, RacesData.NotRaceWeaponDamagePercent);

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

        int resultInt = (int)(baseDamages * raceGenderMultiplier * skillMultiplier * criticalMultiplier);
        Debug.Log("Final Damages = " + resultInt);
        LosePa(tmpWeapon.PaNeeded);
        foreach (var skill in Character.Skills)
        {
            if (skill != null)
                skill.OnEndAttack(resultInt, opponentBhv);
        }
        return resultInt;
    }

    public void MoveToPosition(int x, int y, bool usePm = true)
    {
        _cellToReachX = x;
        _cellToReachY = y;
        if (usePm)
            SetPath();
        else
        {
            _pathfindingSteps.Clear();
            _pathfindingPos.Clear();
            _pathfindingSteps.Add(_gridBhv.Cells[_cellToReachX, _cellToReachY].transform.position);
            _pathfindingPos.Add(new RangePos(_cellToReachX, _cellToReachY));
        }
        IsMoving = true;
    }

    private void SetPath()
    {
        _pathfindingSteps.Clear();
        _pathfindingPos.Clear();
        _pathfindingSteps.Add(_gridBhv.Cells[_cellToReachX, _cellToReachY].transform.position);
        _pathfindingPos.Add(new RangePos(_cellToReachX, _cellToReachY));
        var visitedIndex = _gridBhv.Cells[_cellToReachX, _cellToReachY].GetComponent<CellBhv>().Visited;
        LosePm(visitedIndex);
        int x = _cellToReachX;
        int y = _cellToReachY;
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
            _pathfindingPos.Insert(0, new RangePos(_cellToReachX, _cellToReachY));
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

    public void Move()
    {
        transform.position = Vector2.Lerp(transform.position, _pathfindingSteps[0], 0.7f);
        if ((Vector2)transform.position == _pathfindingSteps[0])
        {
            _spriteRenderer.sortingOrder = Constants.GridMax - _pathfindingPos[0].Y;
            _pathfindingPos.RemoveAt(0);
            _pathfindingSteps.RemoveAt(0);
            if (_pathfindingSteps.Count == 0)
            {
                IsMoving = false;
                X = _cellToReachX;
                Y = _cellToReachY;
                if (IsPlayer)
                    _fightSceneBhv.AfterPlayerMovement();
                if (AfterMouvementDelegate != null)
                    AfterMouvementDelegate();
            }
        }
    }

    public void Spawn(int x, int y)
    {
        X = x;
        Y = y;
        _spriteRenderer.sortingOrder = Constants.GridMax - y;
        transform.position = _gridBhv.Cells[x, y].transform.position;
    }
}
