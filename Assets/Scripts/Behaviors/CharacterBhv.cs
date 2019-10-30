using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBhv : MonoBehaviour
{
    public int X;
    public int Y;
    public bool IsMoving;
    public int Turn;
    public int Pm;
    public int Pa;
    public Character Character;

    private GridSceneBhv _gridSceneBhv;
    private int _cellToReachX;
    private int _cellToReachY;
    private List<Vector2> _pathfindingSteps;

    void Start()
    {
        SetVariables();
    }

    private void SetVariables()
    {
        Turn = 0;
        IsMoving = false;
        _pathfindingSteps = new List<Vector2>();
        _gridSceneBhv = GameObject.Find("Canvas").GetComponent<GridSceneBhv>();
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
            _pathfindingSteps.Add(_gridSceneBhv.Cells[_cellToReachX, _cellToReachY].transform.position);
        }
        IsMoving = true;
    }

    private void SetPath()
    {
        _pathfindingSteps.Clear();
        _pathfindingSteps.Add(_gridSceneBhv.Cells[_cellToReachX, _cellToReachY].transform.position);
        var visitedIndex = _gridSceneBhv.Cells[_cellToReachX, _cellToReachY].GetComponent<CellBhv>().Visited;
        Pm -= visitedIndex;
        int x = _cellToReachX;
        int y = _cellToReachY;
        while (visitedIndex > 0)
        {
            if (LookForLowerIndex(x, y + 1, visitedIndex - 1) && !_gridSceneBhv.IsAdjacentOpponent(x, y + 1))
                ++y;
            else if (LookForLowerIndex(x + 1, y, visitedIndex - 1) && !_gridSceneBhv.IsAdjacentOpponent(x + 1, y))
                ++x;
            else if (LookForLowerIndex(x, y - 1, visitedIndex - 1) && !_gridSceneBhv.IsAdjacentOpponent(x, y - 1))
                --y;
            else if (LookForLowerIndex(x - 1, y, visitedIndex - 1) && !_gridSceneBhv.IsAdjacentOpponent(x - 1, y))
                --x;
            _pathfindingSteps.Insert(0, _gridSceneBhv.Cells[x, y].transform.position);
            --visitedIndex;
        }
    }

    private bool LookForLowerIndex(int x, int y, int visitedIndex)
    {
        if (x >= Constants.GridMax || y >= Constants.GridMax || x < 0 || y < 0)
            return false;
        if (_gridSceneBhv.Cells[x, y].GetComponent<CellBhv>().Visited == visitedIndex)
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
                _gridSceneBhv.AfterPlayerAction();
            }
        }
    }
}
