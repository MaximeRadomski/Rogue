using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleGridSceneBhv : MonoBehaviour
{
    public GameObject[,] Cells;

    private Maps.Map _map;
    private Grid _grid;
    private GameObject _player;

    void Start()
    {
        SetPrivates();
        SetButtons();
        InitGrid();
        InitPlayer();
        GameLife();
    }

    private void SetPrivates()
    {
        _grid = GameObject.Find("Grid").GetComponent<Grid>();
        Cells = new GameObject[Constants.GridMax, Constants.GridMax];
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonReload").GetComponent<ButtonBhv>().EndActionDelegate = ReloadScene;
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToSampleScene;
    }

    public void GoToSampleScene()
    {
        SceneManager.LoadScene(Constants.SampleScene);
    }

    private void InitGrid()
    {
        _map = Maps.EasyMaps[Random.Range(0, Maps.EasyMaps.Count)];
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
        cellInstance.GetComponent<CellBhv>().X = x;
        cellInstance.GetComponent<CellBhv>().Y = y;
        cellInstance.GetComponent<CellBhv>().Type = (CellBhv.CellType)int.Parse(c.ToString(),System.Globalization.NumberStyles.Integer);
        cellInstance.GetComponent<CellBhv>().State = CellBhv.CellState.None;
        Cells[x, y] = cellInstance;
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

    private void InitPlayer()
    {
        var characterObject = Resources.Load<GameObject>("Prefabs/TemplateCharacter");
        _player = Instantiate(characterObject, GameObject.Find("Cell31").transform.position, characterObject.transform.rotation);
        _player.name = "Player";
        _player.GetComponent<CharacterBhv>().X = 3;
        _player.GetComponent<CharacterBhv>().Y = 1;
        _player.GetComponent<CharacterBhv>().Name = "TemplateCharacter";
        _player.GetComponent<CharacterBhv>().Race = CharacterBhv.CharacterRace.Human;
        _player.GetComponent<CharacterBhv>().Level = 1;
        _player.GetComponent<CharacterBhv>().Gold = 0;
        _player.GetComponent<CharacterBhv>().Hp = 1000;
        _player.GetComponent<CharacterBhv>().Pm = 4;
    }

    private void GameLife()
    {
        ResetAllCellsDisplay();
        ResetAllCellsVisited();
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        ShowPm();
    }

    public void AfterPlayerMoved()
    {
        ResetAllCellsVisited();
        ShowPm();
    }

    private void ShowPm()
    {
        var nbPm = _player.GetComponent<CharacterBhv>().Pm;
        if (nbPm <= 0)
            return;
        SpreadPmStart(_player.GetComponent<CharacterBhv>().X, _player.GetComponent<CharacterBhv>().Y, nbPm);
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
        if (cell.GetComponent<CellBhv>().Type == CellBhv.CellType.On && (spentPm < cell.GetComponent<CellBhv>().Visited || cell.GetComponent<CellBhv>().Visited == -1))
        {
            cell.GetComponent<CellBhv>().ShowPm();
            cell.GetComponent<CellBhv>().Visited = spentPm;
            cell.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = cell.GetComponent<CellBhv>().Visited.ToString();
        }
        if (cell.GetComponent<CellBhv>().Type == CellBhv.CellType.On && --nbPm > 0)
        {
            SpreadPm(x, y + 1, nbPm, spentPm + 1);
            SpreadPm(x + 1, y, nbPm, spentPm + 1);
            SpreadPm(x, y - 1, nbPm, spentPm + 1);
            SpreadPm(x - 1, y, nbPm, spentPm + 1);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
