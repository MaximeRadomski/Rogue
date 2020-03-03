using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBhv : MonoBehaviour
{
    public GameObject[,] Cells = new GameObject[Constants.GridMax, Constants.GridMax];
    public Map Map;

    private Grid _grid;
    private FightSceneBhv _fightSceneBhv;
    private CharacterBhv _currentCharacterBhv;
    private List<CharacterBhv> _currentOpponentBhvs;
    private int _currentWeaponId;
    private int _currentSkillId;

    public void SetPrivates()
    {
        _grid = GameObject.Find("Grid").GetComponent<Grid>();
        _fightSceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>();
    }

    public bool CanStart()
    {
        foreach (var cell in Cells)
        {
            if (!cell.GetComponent<CellBhv>().ReadyToStart)
                return false;
        }
        return true;
    }

    #region Init

    public void InitGrid(Map map)
    {
        Map = map;
        for (int y = 0; y < Constants.GridMax; ++y)
        {
            for (int x = 0; x < Constants.GridMax; ++x)
            {
                InitCell(x, y, Map.Cells[Constants.GridMax * y + x]);
            }
        }
    }

    private void InitCell(int x, int y, char c)
    {
        var cellInstance = _fightSceneBhv.Instantiator.NewCell(x, y, c, _grid);
        Cells[x, y] = cellInstance;
    }

    #endregion

    #region Spawn

    public void SpawnOpponent(List<CharacterBhv> opponentBhvs)
    {
        List<RangePos> opponentSpawns = new List<RangePos>();
        char spawnChar = CellType.OpponentSpawn.GetHashCode().ToString()[0];
        for (int i = 0; i < Map.Cells.Length; ++i)
        {
            if (Map.Cells[i] == spawnChar)
                opponentSpawns.Add(new RangePos(i % Constants.GridMax, i / Constants.GridMax));
        }
        foreach (var opponentBhv in opponentBhvs)
        {            
            int spawnId = Random.Range(0, opponentSpawns.Count);
            opponentBhv.Spawn(opponentSpawns[spawnId].X, opponentSpawns[spawnId].Y);
            opponentSpawns.RemoveAt(spawnId);
        }
    }

    public void SpawnPlayer()
    {
        foreach (var cell in Cells)
        {
            cell.GetComponent<CellBhv>().ShowPlayerSpawn();
        }
    }

    public void OnPlayerSpawnClick(int x, int y)
    {
        _fightSceneBhv.OnPlayerSpawnClick(x, y);
    }

    #endregion

    #region Mouvement

    public void ShowPm(CharacterBhv characterBhv, List<CharacterBhv> opponentBhvs, bool unlimitedPm = false)
    {
        ResetAllCellsVisited();
        ResetAllCellsDisplay();
        var nbPm = unlimitedPm ? Constants.UnlimitedPm : characterBhv.GetComponent<CharacterBhv>().Pm;
        int x = characterBhv.GetComponent<CharacterBhv>().X;
        int y = characterBhv.GetComponent<CharacterBhv>().Y;
        if (nbPm <= 0 || IsAdjacentOpponent(x, y, opponentBhvs))
            return;
        SpreadPmStart(x, y, nbPm, characterBhv, opponentBhvs);
    }

    private void SpreadPmStart(int x, int y, int nbPm, CharacterBhv characterBhv, List<CharacterBhv> opponentBhvs)
    {
        SpreadPm(x, y + 1, nbPm, 1, characterBhv, opponentBhvs);
        SpreadPm(x + 1, y, nbPm, 1, characterBhv, opponentBhvs);
        SpreadPm(x, y - 1, nbPm, 1, characterBhv, opponentBhvs);
        SpreadPm(x - 1, y, nbPm, 1, characterBhv, opponentBhvs);
    }

    private void SpreadPm(int x, int y, int nbPm, int spentPm, CharacterBhv characterBhv, List<CharacterBhv> opponentBhvs)
    {
        if (!Helper.IsPosValid(x, y))
            return;
        var cell = Cells[x, y];
        if (cell == null || (x == characterBhv.X && y == characterBhv.Y))
            return;
        if (cell.GetComponent<CellBhv>().Type == CellType.On &&
            (spentPm < cell.GetComponent<CellBhv>().Visited || cell.GetComponent<CellBhv>().Visited == -1) &&
            !IsOpponentOnCell(x, y))
        {
            //if (characterBhv.IsPlayer)
            cell.GetComponent<CellBhv>().ShowPm();
            cell.GetComponent<CellBhv>().Visited = spentPm;
            //DEBUG//
            //cell.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = cell.GetComponent<CellBhv>().Visited.ToString();
            if (--nbPm > 0 && !IsAdjacentOpponent(x, y, opponentBhvs))
            {
                SpreadPm(x, y + 1, nbPm, spentPm + 1, characterBhv, opponentBhvs);
                SpreadPm(x + 1, y, nbPm, spentPm + 1, characterBhv, opponentBhvs);
                SpreadPm(x, y - 1, nbPm, spentPm + 1, characterBhv, opponentBhvs);
                SpreadPm(x - 1, y, nbPm, spentPm + 1, characterBhv, opponentBhvs);
            }
        }
    }

    public bool IsAdjacentOpponent(int x, int y, List<CharacterBhv> opponentBhvs)
    {
        foreach (var opponentBhv in opponentBhvs)
        {
            if (x == opponentBhv.X && y + 1 == opponentBhv.Y)
                return true;
            else if (x + 1 == opponentBhv.X && y == opponentBhv.Y)
                return true;
            else if (x == opponentBhv.X && y - 1 == opponentBhv.Y)
                return true;
            else if (x - 1 == opponentBhv.X && y == opponentBhv.Y)
                return true;
        }
        return false;
    }

    public CharacterBhv IsOpponentOnCell(int x, int y)
    {
        if (_currentOpponentBhvs == null)
            return null;
        foreach (var opponentBhv in _currentOpponentBhvs)
        {
            if (x == opponentBhv.X && y == opponentBhv.Y)
                return opponentBhv;
        }
        if (!_currentCharacterBhv.IsPlayer)
        {
            foreach (var opponentBhv in _fightSceneBhv.OpponentBhvs)
            {
                if (x == opponentBhv.X && y == opponentBhv.Y)
                    return opponentBhv;
            }
        }
        return null;
    }

    public void OnPlayerMovementClick(int x, int y)
    {
        _fightSceneBhv.OnPlayerMovementClick(x, y);
    }

    #endregion

    #region Weapon

    public bool IsOpponentInWeaponRangeAndZone(CharacterBhv characterBhv, int weaponId, List<CharacterBhv> opponentBhvs)
    {
        _currentCharacterBhv = characterBhv;
        _currentOpponentBhvs = opponentBhvs;
        _currentWeaponId = weaponId;
        var character = characterBhv.Character;
        for (int i = 0; i < character.Weapons[weaponId].RangePositions.Count; i += 2)
        {
            var x = character.Weapons[weaponId].RangePositions[i] + characterBhv.X;
            var y = character.Weapons[weaponId].RangePositions[i + 1] + characterBhv.Y;
            if (!Helper.IsPosValid(x, y))
                continue;
            var cell = Cells[x, y].GetComponent<CellBhv>();
            if (cell.Type == CellType.On)
            {
                if (IsAnythingBetween(characterBhv.X, characterBhv.Y, x, y))
                    continue;
                else if (IsOpponentOnCell(x, y) != null || IsOpponentInZone(x, y) != null)
                    return true;
            }
        }
        return false;
    }

    public void ShowWeaponRange(CharacterBhv characterBhv, int weaponId, List<CharacterBhv> opponentBhvs)
    {
        ResetAllCellsDisplay();
        _currentCharacterBhv = characterBhv;
        _currentOpponentBhvs = opponentBhvs;
        _currentWeaponId = weaponId;
        var character = characterBhv.Character;
        for (int i = 0; i < character.Weapons[weaponId].RangePositions.Count; i += 2)
        {
            var x = character.Weapons[weaponId].RangePositions[i] + characterBhv.X;
            var y = character.Weapons[weaponId].RangePositions[i+1] + characterBhv.Y;
            if (!Helper.IsPosValid(x, y))
                continue;
            var cell = Cells[x, y].GetComponent<CellBhv>();
            if (cell.Type == CellType.On && cell.State == CellState.None)
            {
                if (IsAnythingBetween(characterBhv.X, characterBhv.Y, x, y))
                    cell.ShowWeaponOutOfRange();
                else
                    cell.ShowWeaponRange();
            }
                
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

    private bool IsAnythingBetween(int x1, int y1, int x2, int y2)
    {
        SetOpponentsHitBoxes(true);
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        if (Physics2D.Linecast(Cells[x1, y1].transform.position, Cells[x2, y2].transform.position, contactFilter.NoFilter(), results:hits ) > 0)
        {
            foreach (var hit in hits)
            {
                if ((hit.transform.gameObject.TryGetComponent(out CellBhv cell) && cell.Type == CellType.Off) ||
                    (hit.transform.gameObject.TryGetComponent(out CharacterBhv characterBhv) && characterBhv.IsPlayer == false && !(characterBhv.X == x2 && characterBhv.Y == y2)))
                {
                    SetOpponentsHitBoxes(false);
                    return true;
                }
            }
        }
        SetOpponentsHitBoxes(false);
        return false;
    }

    private void SetOpponentsHitBoxes(bool setting)
    {
        foreach (var opponentBhv in _currentOpponentBhvs)
        {
            opponentBhv.GetComponent<BoxCollider2D>().enabled = setting;
        }
        if (!_currentCharacterBhv.IsPlayer)
        {
            foreach (var opponentBhv in _fightSceneBhv.OpponentBhvs)
            {
                opponentBhv.GetComponent<BoxCollider2D>().enabled = setting;
            }
        }
    }

    public void OnPlayerAttackClick(int x, int y)
    {
        List<CharacterBhv> tmpTouchedOpponents = null;
        CharacterBhv touchedCell = null;
        List<CharacterBhv> touchedZone = null;
        if ((touchedCell = IsOpponentOnCell(x, y)) != null || (touchedZone = IsOpponentInZone(x, y)) != null)
        {
            tmpTouchedOpponents = new List<CharacterBhv>();
            if (touchedCell != null)
                tmpTouchedOpponents.Add(touchedCell);
            if (touchedZone != null)
            {
                foreach (var touched in touchedZone)
                {
                    tmpTouchedOpponents.Add(touched);
                }
            }
            _fightSceneBhv.OnPlayerAttackClick(_currentWeaponId, tmpTouchedOpponents);
            return;
        }
        _fightSceneBhv.OnPlayerAttackClick(_currentWeaponId, null);
    }

    private List<CharacterBhv> IsOpponentInZone(int x, int y)
    {
        var character = _currentCharacterBhv.Character;
        if (character.Weapons[_currentWeaponId].RangeZones == null)
            return null;
        List<CharacterBhv> tmpTouchedOpponents = new List<CharacterBhv>();
        foreach (var tmpDirection in character.Weapons[_currentWeaponId].RangeZones)
        {
            var tmpPos = Helper.DetermineRangePosFromRangeDirection(x - _currentCharacterBhv.X, y - _currentCharacterBhv.Y, tmpDirection);
            var tmpX = tmpPos.X + x;
            var tmpY = tmpPos.Y + y;
            if (!Helper.IsPosValid(tmpX, tmpY))
                continue;
            var cell = Cells[tmpX, tmpY].GetComponent<CellBhv>();
            CharacterBhv tmpTouched = null;
            if (cell.Type == CellType.On && (tmpTouched = IsOpponentOnCell(tmpX, tmpY)) != null)
            {
                tmpTouchedOpponents.Add(tmpTouched);
                continue;
            }
        }
        if (tmpTouchedOpponents.Count != 0)
            return tmpTouchedOpponents;
        return null;
    }

    #endregion

    #region SKill

    public bool IsOpponentInSkillRange(CharacterBhv characterBhv, int skillId, List<CharacterBhv> opponentBhvs)
    {
        _currentCharacterBhv = characterBhv;
        _currentOpponentBhvs = opponentBhvs;
        var character = characterBhv.Character;
        for (int i = 0; i < character.Skills[skillId].RangePositions?.Count; i += 2)
        {
            var x = character.Skills[skillId].RangePositions[i] + characterBhv.X;
            var y = character.Skills[skillId].RangePositions[i + 1] + characterBhv.Y;
            if (!Helper.IsPosValid(x, y))
                continue;
            var cell = Cells[x, y].GetComponent<CellBhv>();
            if (cell.Type == CellType.On)
            {
                if (IsAnythingBetween(characterBhv.X, characterBhv.Y, x, y))
                    continue;
                else if (IsOpponentOnCell(x, y) != null)
                    return true;
            }
        }
        return false;
    }

    public void ShowSkillRange(RangeType rangeType, CharacterBhv characterBhv, int skillId, List<CharacterBhv> opponentBhvs, bool hasToBeEmpty = false)
    {
        ResetAllCellsDisplay();
        ResetAllCellsVisited();
        _currentCharacterBhv = characterBhv;
        _currentOpponentBhvs = opponentBhvs;
        _currentSkillId = skillId;
        var character = characterBhv.Character;
        if (rangeType != RangeType.FullRange)
        {
            if (character.Skills[skillId].RangePositions == null || character.Skills[skillId].RangePositions.Count == 0)
                return;
            for (int i = 0; i < character.Skills[skillId].RangePositions.Count; i += 2)
            {
                var x = character.Skills[skillId].RangePositions[i] + characterBhv.X;
                var y = character.Skills[skillId].RangePositions[i + 1] + characterBhv.Y;
                ShowSkillRangeOnPosition(x, y, hasToBeEmpty, rangeType, characterBhv);
            }
        }
        else
        {
            for (int x = 0; x < Constants.GridMax; ++x)
            {
                for (int y = 0; y < Constants.GridMax; ++y)
                {
                    ShowSkillRangeOnPosition(x, y, hasToBeEmpty, rangeType, characterBhv);
                }
            }
        }
    }

    private void ShowSkillRangeOnPosition(int x, int y, bool hasToBeEmpty, RangeType rangeType, CharacterBhv characterBhv)
    {
        if (!Helper.IsPosValid(x, y))
            return;
        var cell = Cells[x, y].GetComponent<CellBhv>();
        if (cell.Type == CellType.On && cell.State == CellState.None)
        {
            if ((hasToBeEmpty && IsOpponentOnCell(x, y)) ||
                (rangeType == RangeType.Normal && IsAnythingBetween(characterBhv.X, characterBhv.Y, x, y)))
            {
                if (characterBhv.IsPlayer)
                    cell.ShowSkillOutOfRange();
            }
            else
            {
                if (characterBhv.IsPlayer)
                    cell.ShowSkillRange();
                else
                    cell.ShowSkillRangeVisited();
            }
                
        }
    }

    public void OnPlayerSkillClick(int x, int y)
    {
        _fightSceneBhv.OnPlayerSkillClick(_currentSkillId, x, y);
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
