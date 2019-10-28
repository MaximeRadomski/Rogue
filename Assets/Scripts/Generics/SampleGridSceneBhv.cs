using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleGridSceneBhv : MonoBehaviour
{
    private UnityEngine.UI.Text _sampleText;
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
        _sampleText = GameObject.Find("SampleText").GetComponent<UnityEngine.UI.Text>();
        _grid = GameObject.Find("Grid").GetComponent<Grid>();
    }

    private void SetButtons()
    {
        SetSampleButton("ButtonBotMid");
        SetSampleButton("ButtonBotLeft");
        SetSampleButton("ButtonBotRight");
        GameObject.Find("ButtonTopMid").GetComponent<ButtonBhv>().EndActionDelegate = ReloadScene;
        GameObject.Find("ButtonTopLeft").GetComponent<ButtonBhv>().EndActionDelegate = GoToSampleScene;
    }

    private void SetSampleButton(string name)
    {
        var tmpButtonBhv = GameObject.Find(name).GetComponent<ButtonBhv>();
        tmpButtonBhv.BeginActionDelegate = BeginAction;
        tmpButtonBhv.DoActionDelegate = DoAction;
        tmpButtonBhv.EndActionDelegate = EndAction;
    }

    public void BeginAction()
    {
        _sampleText.text = "Start\n";
    }

    public void DoAction()
    {
        _sampleText.text += "|";
    }

    public void EndAction()
    {
        _sampleText.text += "\nEnd";
    }

    public void GoToSampleScene()
    {
        SceneManager.LoadScene(Constants.SampleScene);
    }

    private void InitGrid()
    {
        _map = Maps.EasyMaps[Random.Range(0, Maps.EasyMaps.Count)];
        GameObject.Find("MapName").GetComponent<UnityEngine.UI.Text>().text = _map.Name;
        for (int y = 0; y < 6; ++y)
        {
            for (int x = 0; x < 6; ++x)
            {
                InitCell(x, y, _map.Cells[6 * y + x]);
            }
        }
    }

    private void InitCell(int x, int y, char c)
    {
        var cellGameObject = Resources.Load<GameObject>("Prefabs/TemplateCell");
        var cellInstance = Instantiate(cellGameObject, cellGameObject.transform);
        cellInstance.transform.parent = _grid.transform;
        cellInstance.transform.position = new Vector3(x * _grid.cellSize.x, y * _grid.cellSize.y, 0.0f) + _grid.transform.position;
        cellInstance.gameObject.name = "Cell" + x + y;
        cellInstance.GetComponent<CellBhv>().X = x;
        cellInstance.GetComponent<CellBhv>().Y = y;
        cellInstance.GetComponent<CellBhv>().Type = (CellBhv.CellType)int.Parse(c.ToString(),System.Globalization.NumberStyles.Integer);
        cellInstance.GetComponent<CellBhv>().State = CellBhv.CellState.None;
    }

    private void InitPlayer()
    {
        var characterObject = Resources.Load<GameObject>("Prefabs/TemplateCharacter");
        _player = Instantiate(characterObject, GameObject.Find("Cell31").transform);
        _player.GetComponent<Character>().X = 3;
        _player.GetComponent<Character>().Y = 1;
        _player.GetComponent<Character>().Name = "TemplateCharacter";
        _player.GetComponent<Character>().Race = Character.CharacterRace.Human;
        _player.GetComponent<Character>().Level = 1;
        _player.GetComponent<Character>().Gold = 0;
        _player.GetComponent<Character>().Hp = 1000;
        _player.GetComponent<Character>().Pm = 2;
    }

    private void GameLife()
    {
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        ShowPm();
    }

    private void ShowPm()
    {
        var nbPm = _player.GetComponent<Character>().Pm;
        if (nbPm <= 0)
            return;
        SpreadPm(_player.GetComponent<Character>().X, _player.GetComponent<Character>().Y + 1, nbPm, 1, 1);
        SpreadPm(_player.GetComponent<Character>().X + 1, _player.GetComponent<Character>().Y, nbPm, 1, -1);
        SpreadPm(_player.GetComponent<Character>().X, _player.GetComponent<Character>().Y - 1, nbPm, -1, -1);
        SpreadPm(_player.GetComponent<Character>().X - 1, _player.GetComponent<Character>().Y, nbPm, -1, 1);
    }

    private void SpreadPm(int x, int y, int nbPm, int xMult, int yMult)
    {
        var cell = GameObject.Find("Cell" + x + y);
        if (cell == null)
            return;
        if (cell.GetComponent<CellBhv>().Type == CellBhv.CellType.On && cell.GetComponent<CellBhv>().State != CellBhv.CellState.Mouvement)
            cell.GetComponent<CellBhv>().ShowPm();
        if (--nbPm > 0)
        {
            SpreadPm(x, y + yMult, nbPm, xMult, yMult);
            SpreadPm(x + xMult, y, nbPm, xMult, yMult);
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
