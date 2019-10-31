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

    private GameObject _opponent;
    private FightSceneBhv _fightSceneBhv;
    private GridBhv _gridBhv;
    private int _cellToReachX;
    private int _cellToReachY;
    private List<Vector2> _pathfindingSteps = new List<Vector2>();

    private void GetFightScene()
    {
        
    }

    public void SetPrivates()
    {
        _fightSceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>();
        _gridBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GridBhv>();
        if (IsPlayer)
            _opponent = GameObject.Find(Constants.GoOpponentName);
        else
            _opponent = GameObject.Find(Constants.GoPlayerName);
    }

    void Update()
    {
        if (IsMoving)
            Move();
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
            _pathfindingSteps.Add(_gridBhv.Cells[_cellToReachX, _cellToReachY].transform.position);
        }
        IsMoving = true;
    }

    private void SetPath()
    {
        _pathfindingSteps.Clear();
        _pathfindingSteps.Add(_gridBhv.Cells[_cellToReachX, _cellToReachY].transform.position);
        var visitedIndex = _gridBhv.Cells[_cellToReachX, _cellToReachY].GetComponent<CellBhv>().Visited;
        Pm -= visitedIndex;
        int x = _cellToReachX;
        int y = _cellToReachY;
        while (visitedIndex > 0)
        {
            if (LookForLowerIndex(x, y + 1, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x, y + 1, _opponent))
                ++y;
            else if (LookForLowerIndex(x + 1, y, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x + 1, y, _opponent))
                ++x;
            else if (LookForLowerIndex(x, y - 1, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x, y - 1, _opponent))
                --y;
            else if (LookForLowerIndex(x - 1, y, visitedIndex - 1) && !_gridBhv.IsAdjacentOpponent(x - 1, y, _opponent))
                --x;
            _pathfindingSteps.Insert(0, _gridBhv.Cells[x, y].transform.position);
            --visitedIndex;
        }
    }

    private bool LookForLowerIndex(int x, int y, int visitedIndex)
    {
        if (!Helpers.IsPosValid(x, y))
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
        transform.position = _gridBhv.Cells[x, y].transform.position;
        if (IsPlayer)
            _fightSceneBhv.AfterPlayerSpawn();
    }
}
