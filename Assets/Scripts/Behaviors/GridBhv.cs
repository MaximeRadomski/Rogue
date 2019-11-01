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
        GameObject.Find("MapName").GetComponent<UnityEngine.UI.Text>().text = _map.Name;
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
        Cells[x, y] = cellInstance;
    }

    #endregion

    #region Spawn

    public void SpawnOpponent(GameObject opponent)
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
        opponent.GetComponent<CharacterBhv>().MoveToPosition(preciseCharId % Constants.GridMax, preciseCharId / Constants.GridMax, false);
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

    public void ShowPm(GameObject character, GameObject opponent)
    {
        ResetAllCellsVisited();
        ResetAllCellsDisplay();
        var nbPm = character.GetComponent<CharacterBhv>().Pm;
        int x = character.GetComponent<CharacterBhv>().X;
        int y = character.GetComponent<CharacterBhv>().Y;
        if (nbPm <= 0 || IsAdjacentOpponent(x, y, opponent))
            return;
        SpreadPmStart(x, y, nbPm, character, opponent);
    }

    private void SpreadPmStart(int x, int y, int nbPm, GameObject character, GameObject opponent)
    {
        SpreadPm(x, y + 1, nbPm, 1, character, opponent);
        SpreadPm(x + 1, y, nbPm, 1, character, opponent);
        SpreadPm(x, y - 1, nbPm, 1, character, opponent);
        SpreadPm(x - 1, y, nbPm, 1, character, opponent);
    }

    private void SpreadPm(int x, int y, int nbPm, int spentPm, GameObject character, GameObject opponent)
    {
        if (!Helpers.IsPosValid(x, y))
            return;
        var cell = Cells[x, y];
        if (cell == null || (x == character.GetComponent<CharacterBhv>().X && y == character.GetComponent<CharacterBhv>().Y))
            return;
        if (cell.GetComponent<CellBhv>().Type == CellType.On
            && (spentPm < cell.GetComponent<CellBhv>().Visited || cell.GetComponent<CellBhv>().Visited == -1)
            && !(x == opponent.GetComponent<CharacterBhv>().X && y == opponent.GetComponent<CharacterBhv>().Y))
        {
            cell.GetComponent<CellBhv>().ShowPm();
            cell.GetComponent<CellBhv>().Visited = spentPm;
            //DEBUG//
            //cell.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = cell.GetComponent<CellBhv>().Visited.ToString();
        }
        if (cell.GetComponent<CellBhv>().Type == CellType.On && --nbPm > 0 && !IsAdjacentOpponent(x, y, opponent))
        {
            SpreadPm(x, y + 1, nbPm, spentPm + 1, character, opponent);
            SpreadPm(x + 1, y, nbPm, spentPm + 1, character, opponent);
            SpreadPm(x, y - 1, nbPm, spentPm + 1, character, opponent);
            SpreadPm(x - 1, y, nbPm, spentPm + 1, character, opponent);
        }
    }

    public bool IsAdjacentOpponent(int x, int y, GameObject opponent)
    {
        int xOpponent = opponent.GetComponent<CharacterBhv>().X;
        int yOpponent = opponent.GetComponent<CharacterBhv>().Y;
        if (x == xOpponent && y + 1 == yOpponent)
            return true;
        else if (x + 1 == xOpponent && y == yOpponent)
            return true;
        else if (x == xOpponent && y - 1 == yOpponent)
            return true;
        else if (x - 1 == xOpponent && y == yOpponent)
            return true;
        return false;
    }

    #endregion

    #region Weapon

    public void ShowWeaponRange(CharacterBhv characterBhv, int weaponId)
    {
        ResetAllCellsDisplay();
        _currentCharacterBhv = characterBhv;
        _currentWeaponId = weaponId;
        var character = characterBhv.Character;
        for (int i = 0; i < character.Weapons[weaponId].RangePositions.Count; ++i)
        {
            var x = character.Weapons[weaponId].RangePositions[i].X + characterBhv.X;
            var y = character.Weapons[weaponId].RangePositions[i].Y + characterBhv.Y;
            if (!Helpers.IsPosValid(x, y))
                continue;
            var cell = Cells[x, y].GetComponent<CellBhv>();
            if (cell.Type == CellType.On && cell.State == CellState.None && !IsWallBetween(characterBhv.X, characterBhv.Y, x, y))
                cell.ShowWeaponRange();
        }
    }

    public void ShowWeaponZone(int x, int y)
    {
        var character = _currentCharacterBhv.Character;
        foreach (var tmpDirection in character.Weapons[_currentWeaponId].RangeZones)
        {
            var tmpPos = Helpers.DetermineRangePosFromRangeDirection(x - _currentCharacterBhv.X, y - _currentCharacterBhv.Y, tmpDirection);
            var tmpX = tmpPos.X + x;
            var tmpY = tmpPos.Y + y;
            if (!Helpers.IsPosValid(tmpX, tmpY))
                continue;
            var cell = Cells[tmpX, tmpY].GetComponent<CellBhv>();
            if (cell.Type == CellType.On && cell.State == CellState.None)
                cell.ShowWeaponZone();
        }
    }

    private bool IsWallBetween(int x1, int y1, int x2, int y2)
    {
        for (int i = x1 + 1; i < x2; ++i)
        {
            if (Cells[i, y1].GetComponent<CellBhv>().Type == CellType.Off)
                return true;
        }
        for (int i = y1 + 1; i < y2; ++i)
        {
            if (Cells[x1, i].GetComponent<CellBhv>().Type == CellType.Off)
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
