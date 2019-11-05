using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBhv : MonoBehaviour
{
    public int X;
    public int Y;
    public bool IsMoving = false;
    public int Turn = 0;
    public int Pm;
    public int Pa;
    public Character Character;
    public bool IsPlayer = false;

    private CharacterBhv _opponentBhv;
    private FightSceneBhv _fightSceneBhv;
    private Instantiator _instantiator;
    private GridBhv _gridBhv;
    private int _cellToReachX;
    private int _cellToReachY;
    private List<Vector2> _pathfindingSteps = new List<Vector2>();
    private List<RangePos> _pathfindingPos = new List<RangePos>();
    private SpriteRenderer _spriteRenderer;

    public void SetPrivates()
    {
        _fightSceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>();
        _gridBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GridBhv>();
        if (IsPlayer)
            _opponentBhv = GameObject.Find(Constants.GoOpponentName).GetComponent<CharacterBhv>();
        else
            _opponentBhv = GameObject.Find(Constants.GoPlayerName).GetComponent<CharacterBhv>();
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<Instantiator>();
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        for (int i = 0; i < Character.Skills.Count; ++i)
        {
            if (Character.Skills[i] != null)
                Character.Skills[i].Init(this, _opponentBhv, _gridBhv, i);
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
                skill.OnTakeDamage(damages);
        }
        _instantiator.PopText(damages.ToString(), transform.position, TextType.Life);
        Character.Hp -= damages;
    }

    public int AttackWithWeapon(int weaponId, CharacterBhv opponentBhv, Map map)
    {
        foreach (var skill in Character.Skills)
        {
            if (skill != null)
                skill.OnStartAttack();
        }
        var tmpWeapon = Character.Weapons[weaponId];

        float baseDamages = tmpWeapon.BaseDamage * Helper.MultiplierFromPercent(1, Random.Range(-tmpWeapon.DamageRangePercentage, tmpWeapon.DamageRangePercentage));
        if (tmpWeapon.Type != Character.FavWeapons[0] && tmpWeapon.Type != Character.FavWeapons[1])
            baseDamages = baseDamages * Helper.MultiplierFromPercent(0, RacesData.NotRaceWeaponDamagePercent);

        float multiplier = 1.0f;
        if (opponentBhv.Character.Race == Character.StrongAgainst)
            multiplier = Helper.MultiplierFromPercent(multiplier, RacesData.StrongAgainstDamagePercent);
        if (map.Type == Character.StrongIn)
            multiplier = Helper.MultiplierFromPercent(multiplier, RacesData.StrongInDamagePercent);
        if (Character.Gender == CharacterGender.Female)
            multiplier = Helper.MultiplierFromPercent(multiplier, -RacesData.GenderDamage);
        else
            multiplier = Helper.MultiplierFromPercent(multiplier, RacesData.GenderDamage);

        float criticalMultiplier = 1.0f;
        int criticalPercent = Random.Range(0, 100);
        if (criticalPercent < tmpWeapon.CritChancePercent)
        {
            criticalMultiplier = Helper.MultiplierFromPercent(multiplier, tmpWeapon.CritMultiplierPercent);
            if (Character.Gender == CharacterGender.Male)
                criticalMultiplier = Helper.MultiplierFromPercent(criticalMultiplier, -RacesData.GenderCritical);
            else
                criticalMultiplier = Helper.MultiplierFromPercent(criticalMultiplier, RacesData.GenderCritical);
        }

        int resultIn = (int)((baseDamages * multiplier) * criticalMultiplier);
        Debug.Log("Final Damages = " + resultIn);
        Pa -= tmpWeapon.PaNeeded;
        _instantiator.PopText(tmpWeapon.PaNeeded.ToString(), transform.position, TextType.Pa);
        return resultIn;
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
        Pm -= visitedIndex;
        _instantiator.PopText(visitedIndex.ToString(), transform.position, TextType.Pm);
        int x = _cellToReachX;
        int y = _cellToReachY;
        while (visitedIndex > 0)
        {
            if (LookForLowerIndex(x, y + 1, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x, y + 1, _opponentBhv))
                ++y;
            else if (LookForLowerIndex(x + 1, y, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x + 1, y, _opponentBhv))
                ++x;
            else if (LookForLowerIndex(x, y - 1, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x, y - 1, _opponentBhv))
                --y;
            else if (LookForLowerIndex(x - 1, y, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x - 1, y, _opponentBhv))
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
            }
        }
    }

    public void Spawn(int x, int y)
    {
        X = x;
        Y = y;
        _spriteRenderer.sortingOrder = Constants.GridMax - y;
        transform.position = _gridBhv.Cells[x, y].transform.position;
        if (IsPlayer)
            _fightSceneBhv.AfterPlayerSpawn();
    }
}
