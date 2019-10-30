using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridSceneBhv : MonoBehaviour
{
    public GameObject[,] Cells;

    private Map _map;
    private Grid _grid;
    private GameObject _player;
    private GameObject _opponent;

    void Start()
    {
        Application.targetFrameRate = 60;
        SetPrivates();
        SetButtons();
        InitGrid();
        InitOpponent();
        InitPlayer();
        GameLife();
        Helpers.XpNeedForLevel(1);
        Helpers.XpNeedForLevel(2);
        Helpers.XpNeedForLevel(3);
        Helpers.XpNeedForLevel(4);
        Helpers.XpNeedForLevel(5);
        Helpers.XpNeedForLevel(6);
        Helpers.XpNeedForLevel(7);
        Helpers.XpNeedForLevel(8);
        Helpers.XpNeedForLevel(9);
        Helpers.XpNeedForLevel(10);
        Helpers.XpNeedForLevel(11);
    }

    private void SetPrivates()
    {
        _grid = GameObject.Find("Grid").GetComponent<Grid>();
        Cells = new GameObject[Constants.GridMax, Constants.GridMax];
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonReload").GetComponent<ButtonBhv>().EndActionDelegate = ReloadScene;
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToSwipeScene;
        GameObject.Find("ButtonPassTurn").GetComponent<ButtonBhv>().EndActionDelegate = PassTurn;
    }

    public void GoToSwipeScene()
    {
        SceneManager.LoadScene(Constants.SwipeScene);
    }

    private void InitGrid()
    {
        _map = MapsData.EasyMaps[Random.Range(0, MapsData.EasyMaps.Count)];
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
        cellBhv.Type = (CellBhv.CellType)int.Parse(c.ToString(),System.Globalization.NumberStyles.Integer);
        cellBhv.State = CellBhv.CellState.None;
        Cells[x, y] = cellInstance;
    }

    private void InitOpponent()
    {
        var characterObject = Resources.Load<GameObject>("Prefabs/TemplateCharacter");
        _opponent = Instantiate(characterObject, GameObject.Find("Cell24").transform.position, characterObject.transform.rotation);
        _opponent.name = "Opponent";
        var characterBhv = _opponent.GetComponent<CharacterBhv>();
        characterBhv.X = 2;
        characterBhv.Y = 4;
        characterBhv.Character = RacesData.GetBaseHuman();
        characterBhv.Character.Name = "TemplateOpponent";
        characterBhv.Character.Race = CharacterRace.Elf;
        characterBhv.Character.Level = 1;
        characterBhv.Character.Gold = 0;
        characterBhv.Character.HpMax = 1000;
        characterBhv.Character.PmMax = 4;
    }

    private void InitPlayer()
    {
        var characterObject = Resources.Load<GameObject>("Prefabs/TemplateCharacter");
        _player = Instantiate(characterObject, GameObject.Find("Cell31").transform.position, characterObject.transform.rotation);
        _player.name = "Player";
        var characterBhv = _player.GetComponent<CharacterBhv>();
        characterBhv.X = 3;
        characterBhv.Y = 1;
        characterBhv.Character = RacesData.GetBaseHuman();
        characterBhv.Character.Name = "TemplateCharacter";
        characterBhv.Character.Race = CharacterRace.Human;
        characterBhv.Character.Level = 1;
        characterBhv.Character.Gold = 0;
        characterBhv.Character.HpMax = 1000;
        characterBhv.Character.PmMax = 4;
    }

    public void ResetAllCellsDisplay()
    {
        for (int y = 0; y < Constants.GridMax; ++y)
        {
            for (int x = 0; x < Constants.GridMax; ++x)
            {
                Cells[x, y].GetComponent<CellBhv>().ResetDisplay();
            }
        }
    }

    public void ResetAllCellsVisited()
    {
        for (int y = 0; y < Constants.GridMax; ++y)
        {
            for (int x = 0; x < Constants.GridMax; ++x)
            {
                Cells[x, y].GetComponent<CellBhv>().ResetVisited();
            }
        }
    }

    private void GameLife()
    {
        ResetAllCellsDisplay();
        ResetAllCellsVisited();
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        _player.GetComponent<CharacterBhv>().Pm = _player.GetComponent<CharacterBhv>().Character.PmMax;
        ShowPm();
    }

    private void PassTurn()
    {
        GameObject.Find("TurnCount").GetComponent<UnityEngine.UI.Text>().text = "Turn Count: " + (++_player.GetComponent<CharacterBhv>().Turn).ToString();
        PlayerTurn();
    }

    public void AfterPlayerAction()
    {
        ResetAllCellsVisited();
        ShowPm();
    }

    private void ShowPm()
    {
        var nbPm = _player.GetComponent<CharacterBhv>().Pm;
        int x = _player.GetComponent<CharacterBhv>().X;
        int y = _player.GetComponent<CharacterBhv>().Y;
        if (nbPm <= 0 || IsAdjacentOpponent(x, y))
            return;
        SpreadPmStart(x, y, nbPm);
    }

    private void SpreadPmStart(int x, int y, int nbPm)
    {
        SpreadPm(x, y + 1, nbPm, 1);
        SpreadPm(x + 1, y, nbPm, 1);
        SpreadPm(x, y - 1, nbPm, 1);
        SpreadPm(x - 1, y, nbPm, 1);
    }

    private void SpreadPm(int x, int y, int nbPm, int spentPm)
    {
        if (x >= Constants.GridMax || y >= Constants.GridMax || x < 0 || y < 0)
            return;
        var cell = Cells[x, y];
        if (cell == null || (x == _player.GetComponent<CharacterBhv>().X && y == _player.GetComponent<CharacterBhv>().Y))
            return;
        if (cell.GetComponent<CellBhv>().Type == CellBhv.CellType.On
            && (spentPm < cell.GetComponent<CellBhv>().Visited || cell.GetComponent<CellBhv>().Visited == -1)
            && !(x == _opponent.GetComponent<CharacterBhv>().X && y == _opponent.GetComponent<CharacterBhv>().Y))
        {
            cell.GetComponent<CellBhv>().ShowPm();
            cell.GetComponent<CellBhv>().Visited = spentPm;
            //DEBUG//
            //cell.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = cell.GetComponent<CellBhv>().Visited.ToString();
        }
        if (cell.GetComponent<CellBhv>().Type == CellBhv.CellType.On && --nbPm > 0 && !IsAdjacentOpponent(x, y))
        {
            SpreadPm(x, y + 1, nbPm, spentPm + 1);
            SpreadPm(x + 1, y, nbPm, spentPm + 1);
            SpreadPm(x, y - 1, nbPm, spentPm + 1);
            SpreadPm(x - 1, y, nbPm, spentPm + 1);
        }
    }

    public bool IsAdjacentOpponent(int x, int y)
    {
        int xOpponent = _opponent.GetComponent<CharacterBhv>().X;
        int yOpponent = _opponent.GetComponent<CharacterBhv>().Y;
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

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
