using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBhv : MonoBehaviour
{
    public int X;
    public int Y;
    public bool IsMoving;
    public int Turn;

    public string Name;
    public CharacterRace Race;
    public int Level;
    public int Experience;
    public int Gold;
    public int HpMax;
    public int Hp;
    public int PmMax;
    public int Pm;
    public List<Weapon> Weapons;

    private SampleGridSceneBhv _sampleGridSceneBhv;
    private Vector2 _cellToReach;
    private int _cellToReachX;
    private int _cellToReachY;
    private List<Vector2> _pathfindingCorders;

    void Start()
    {
        SetVariables();
    }

    private void SetVariables()
    {
        Turn = 0;
        IsMoving = false;
        _cellToReach = transform.position;
        _pathfindingCorders = new List<Vector2>();
        _sampleGridSceneBhv = GameObject.Find("Canvas").GetComponent<SampleGridSceneBhv>();
    }

    void Update()
    {
        if (IsMoving)
            Move();
    }

    public void MoveToPosition(Vector2 cellToReach, int x, int y)
    {
        _cellToReach = cellToReach;
        _cellToReachX = x;
        _cellToReachY = y;
        SetPath();
        IsMoving = true;
    }

    private void SetPath()
    {
        _pathfindingCorders.Clear();
        _pathfindingCorders.Add(_sampleGridSceneBhv.Cells[_cellToReachX, _cellToReachY].transform.position);
        var visitedIndex = _sampleGridSceneBhv.Cells[_cellToReachX, _cellToReachY].GetComponent<CellBhv>().Visited;
        int x = _cellToReachX;
        int y = _cellToReachY;
        while (visitedIndex > 0)
        {
            if (LookForLowerIndex(x, y + 1, visitedIndex - 1))
                ++y;
            else if (LookForLowerIndex(x + 1, y, visitedIndex - 1))
                ++x;
            else if (LookForLowerIndex(x, y - 1, visitedIndex - 1))
                --y;
            else if (LookForLowerIndex(x - 1, y, visitedIndex - 1))
                --x;
            _pathfindingCorders.Insert(0, _sampleGridSceneBhv.Cells[x, y].transform.position);
            --visitedIndex;
        }
    }

    private bool LookForLowerIndex(int x, int y, int visitedIndex)
    {
        if (_sampleGridSceneBhv.Cells[_cellToReachX, _cellToReachY].GetComponent<CellBhv>().Visited == visitedIndex)
            return true;
        return false;
    }

    public void Move()
    {
        transform.position = Vector2.Lerp(transform.position, _pathfindingCorders[0], 0.2f);
        if ((Vector2)transform.position == _pathfindingCorders[0])
        {
            _pathfindingCorders.RemoveAt(0);
            if (_pathfindingCorders.Count == 0)
            {
                IsMoving = false;
                X = _cellToReachX;
                Y = _cellToReachY;
                _sampleGridSceneBhv.AfterPlayerMoved();
            }
        }
    }

    public enum CharacterRace
    {
        Human = 0,
        Gobelin = 1,
        Elf = 2,
        Dwarf = 3,
        Orc = 4
    }

    #region PathFinding

    

    #endregion
}
