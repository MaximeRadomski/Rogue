using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBhv : MonoBehaviour
{
    public GameObject[,] Cells = new GameObject[Constants.GridMax, Constants.GridMax];

    private Map _map;
    private Grid _grid;
    private FightSceneBhv _fightSceneBhv;
    private CharacterBhv _currentCharacterBhv;
    private CharacterBhv _currentOpponentBhv;
    private int _currentWeaponId;

    public void SetPrivates()
    {
        _map = MapsData.EasyMaps[Random.Range(0, MapsData.EasyMaps.Count)];
        _grid = GameObject.Find("Grid").GetComponent<Grid>();
        _fightSceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>();
    }

    #region Init

    public void InitGrid()
    {
        for (int y = 0; y < Constants.GridMax; ++y)
        {
            for (int x = 0; x < Constants.GridMax; ++x)
            {
                InitCell(x, y, _map.Cells[Constants.GridMax * y + x]);
            }
        }
    }

    private void InitCell(int x, int y, char c)
    {
        var cellGameObject = Resources.Load<GameObject>("Prefabs/TemplateCell");
        var cellInstance = Instantiate(cellGameObject, cellGameObject.transform.position, cellGameObject.transform.rotation);
        cellInstance.transform.parent = _grid.transform;
        cellInstance.transform.position = new Vector3(x * _grid.cellSize.x, y * _grid.cellSize.y, 0.0f) + _grid.transform.position;
        cellInstance.gameObject.name = "Cell" + x + y;
        var cellBhv = cellInstance.GetComponent<CellBhv>();
        cellBhv.X = x;
        cellBhv.Y = y;
        cellBhv.Type = (CellType)int.Parse(c.ToString(), System.Globalization.NumberStyles.Integer);
        if (cellBhv.Type == CellType.Spawn || cellBhv.Type == CellType.OpponentSpawn)
            cellBhv.State = CellState.Spawn;
        else
            cellBhv.State = CellState.None;
        cellInstance.GetComponent<SpriteRenderer>().sortingOrder = Constants.GridMax - y;
        Cells[x, y] = cellInstance;
    }

    #endregion

    #region Spawn

    public void SpawnOpponent(CharacterBhv opponentBhv)
    {
        int nbOpponentSpawns = 0;
        char spawnChar = CellType.OpponentSpawn.GetHashCode().ToString()[0];
        foreach (char c in _map.Cells)
        {
            if (c == spawnChar)
                ++nbOpponentSpawns;
        }
        int opponentSpawn = Random.Range(0, nbOpponentSpawns);
        int preciseCharId = -1;
        for (int i = 0; i <= opponentSpawn; ++i)
        {
            preciseCharId = _map.Cells.IndexOf(spawnChar, preciseCharId + 1);
        }
        opponentBhv.MoveToPosition(preciseCharId % Constants.GridMax, preciseCharId / Constants.GridMax, false);
    }

    public void SpawnPlayer()
    {
        foreach (var cell in Cells)
        {
            cell.GetComponent<CellBhv>().ShowPlayerSpawn();
        }
    }

    #endregion

    #region Mouvement

    public void ShowPm(CharacterBhv characterBhv, CharacterBhv opponentBhv)
    {
        ResetAllCellsVisited();
        ResetAllCellsDisplay();
        var nbPm = characterBhv.GetComponent<CharacterBhv>().Pm;
        int x = characterBhv.GetComponent<CharacterBhv>().X;
        int y = characterBhv.GetComponent<CharacterBhv>().Y;
        if (nbPm <= 0 || IsAdjacentOpponent(x, y, opponentBhv))
            return;
        SpreadPmStart(x, y, nbPm, characterBhv, opponentBhv);
    }

    private void SpreadPmStart(int x, int y, int nbPm, CharacterBhv characterBhv, CharacterBhv opponentBhv)
    {
        SpreadPm(x, y + 1, nbPm, 1, characterBhv, opponentBhv);
        SpreadPm(x + 1, y, nbPm, 1, characterBhv, opponentBhv);
        SpreadPm(x, y - 1, nbPm, 1, characterBhv, opponentBhv);
        SpreadPm(x - 1, y, nbPm, 1, characterBhv, opponentBhv);
    }

    private void SpreadPm(int x, int y, int nbPm, int spentPm, CharacterBhv characterBhv, CharacterBhv opponentBhv)
    {
        if (!Helper.IsPosValid(x, y))
            return;
        var cell = Cells[x, y];
        if (cell == null || (x == characterBhv.X && y == characterBhv.Y))
            return;
        if (cell.GetComponent<CellBhv>().Type == CellType.On
            && (spentPm < cell.GetComponent<CellBhv>().Visited || cell.GetComponent<CellBhv>().Visited == -1)
            && !(x == opponentBhv.X && y == opponentBhv.Y))
        {
            cell.GetComponent<CellBhv>().ShowPm();
            cell.GetComponent<CellBhv>().Visited = spentPm;
            //DEBUG//
            //cell.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = cell.GetComponent<CellBhv>().Visited.ToString();
        }
        if (cell.GetComponent<CellBhv>().Type == CellType.On && --nbPm > 0 && !IsAdjacentOpponent(x, y, opponentBhv))
        {
            SpreadPm(x, y + 1, nbPm, spentPm + 1, characterBhv, opponentBhv);
            SpreadPm(x + 1, y, nbPm, spentPm + 1, characterBhv, opponentBhv);
            SpreadPm(x, y - 1, nbPm, spentPm + 1, characterBhv, opponentBhv);
            SpreadPm(x - 1, y, nbPm, spentPm + 1, characterBhv, opponentBhv);
        }
    }

    public bool IsAdjacentOpponent(int x, int y, CharacterBhv opponentBhv)
    {
        if (x == opponentBhv.X && y + 1 == opponentBhv.Y)
            return true;
        else if (x + 1 == opponentBhv.X && y == opponentBhv.Y)
            return true;
        else if (x == opponentBhv.X && y - 1 == opponentBhv.Y)
            return true;
        else if (x - 1 == opponentBhv.X && y == opponentBhv.Y)
            return true;
        return false;
    }

    #endregion

    #region Weapon

    public void ShowWeaponRange(CharacterBhv characterBhv, int weaponId, CharacterBhv opponentBhv)
    {
        ResetAllCellsDisplay();
        _currentCharacterBhv = characterBhv;
        _currentOpponentBhv = opponentBhv;
        _currentWeaponId = weaponId;
        var character = characterBhv.Character;
        for (int i = 0; i < character.Weapons[weaponId].RangePositions.Count; ++i)
        {
            var x = character.Weapons[weaponId].RangePositions[i].X + characterBhv.X;
            var y = character.Weapons[weaponId].RangePositions[i].Y + characterBhv.Y;
            if (!Helper.IsPosValid(x, y))
                continue;
            var cell = Cells[x, y].GetComponent<CellBhv>();
            if (cell.Type == CellType.On && cell.State == CellState.None && !IsAnythingBetween(characterBhv.X, characterBhv.Y, x, y, opponentBhv))
                cell.ShowWeaponRange();
        }
    }

    public void ShowWeaponZone(int x, int y)
    {
        var character = _currentCharacterBhv.Character;
        if (character.Weapons[_currentWeaponId].RangeZones == null)
            return;
        foreach (var tmpDirection in character.Weapons[_currentWeaponId].RangeZones)
        {
            var tmpPos = Helper.DetermineRangePosFromRangeDirection(x - _currentCharacterBhv.X, y - _currentCharacterBhv.Y, tmpDirection);
            var tmpX = tmpPos.X + x;
            var tmpY = tmpPos.Y + y;
            if (!Helper.IsPosValid(tmpX, tmpY))
                continue;
            var cell = Cells[tmpX, tmpY].GetComponent<CellBhv>();
            if (cell.Type == CellType.On && cell.State == CellState.None)
                cell.ShowWeaponZone();
        }
    }

    private bool IsAnythingBetween(int x1, int y1, int x2, int y2, CharacterBhv opponentBhv)
    {
        for (int i = x1 + 1; i < x2; ++i)
        {
            if (Cells[i, y1].GetComponent<CellBhv>().Type == CellType.Off || (opponentBhv.X == i && opponentBhv.Y == y1))
                return true;
        }
        for (int i = y1 + 1; i < y2; ++i)
        {
            if (Cells[x1, i].GetComponent<CellBhv>().Type == CellType.Off || (opponentBhv.X == x1 && opponentBhv.Y == i))
                return true;
        }
        return false;
    }

    public void CheckIfOpponentInRangeOrZone(int x, int y)
    {
        if ((_currentOpponentBhv.X == x && _currentOpponentBhv.Y == y) || IsOpponentInZone(x, y))
        {
            _fightSceneBhv.AfterPlayerAttack(_currentWeaponId, true);
            return;
        }
        _fightSceneBhv.AfterPlayerAttack(_currentWeaponId, false);
    }

    private bool IsOpponentInZone(int x, int y)
    {
        var character = _currentCharacterBhv.Character;
        if (character.Weapons[_currentWeaponId].RangeZones == null)
            return false;
        foreach (var tmpDirection in character.Weapons[_currentWeaponId].RangeZones)
        {
            var tmpPos = Helper.DetermineRangePosFromRangeDirection(x - _currentCharacterBhv.X, y - _currentCharacterBhv.Y, tmpDirection);
            var tmpX = tmpPos.X + x;
            var tmpY = tmpPos.Y + y;
            if (!Helper.IsPosValid(tmpX, tmpY))
                continue;
            var cell = Cells[tmpX, tmpY].GetComponent<CellBhv>();
            if (cell.Type == CellType.On && _currentOpponentBhv.X == tmpX && _currentOpponentBhv.Y == tmpY)
                return true;
        }
        return false;
    }

    #endregion

    #region Resets

    public void ResetAllCellsDisplay()
    {
        foreach (var cell in Cells)
        {
            cell.GetComponent<CellBhv>().ResetDisplay();
        }
    }

    public void ResetAllCellsVisited()
    {
        foreach (var cell in Cells)
        {
            cell.GetComponent<CellBhv>().ResetVisited();
        }
    }

    public void ResetAllCellsSpawn()
    {
        foreach (var cell in Cells)
        {
            cell.GetComponent<CellBhv>().ResetSpawn();
        }
    }

    public void ResetAllCellsZone()
    {
        foreach (var cell in Cells)
        {
            cell.GetComponent<CellBhv>().ResetZone();
        }
    }

    #endregion
}
