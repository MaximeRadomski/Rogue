using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBhv : MonoBehaviour
{
    public int X;
    public int Y;
    public int Visited;
    public CellType Type;
    public CellState State;

    private CharacterBhv _player;
    private SampleGridSceneBhv _sampleGridSceneBhv;

    private void Start()
    {
        SetPrivates();
        SetVisuals();
    }

    private void SetPrivates()
    {
        Visited = -1;
        _sampleGridSceneBhv = GameObject.Find("Canvas").GetComponent<SampleGridSceneBhv>();
}

    private void SetVisuals()
    {
        if (Type == CellType.Off)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
        else if (Type == CellType.Destructible)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        }
    }

    private void GetPlayer()
    {
        _player = GameObject.Find("Player").GetComponent<CharacterBhv>();
    }

    public void BeginAction()
    {
        
    }

    public void DoAction()
    {

    }

    public void EndAction()
    {
        if (State == CellState.None)
            return;
        else if (State == CellState.Mouvement)
        {
            AskPlayerToMove();
            _sampleGridSceneBhv.ResetAllCellsDisplay();
        }
    }

    private void AskPlayerToMove()
    {
        if (_player == null)
            GetPlayer();
        if (!_player.IsMoving)
        {
            Visited = 0;
            _player.MoveToPosition(transform.position, X, Y);
        }
    }

    public void ResetDisplay()
    {
        if (Type == CellType.Off || Type == CellType.Destructible)
            return;
        State = CellState.None;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void ResetVisited()
    {
        Visited = -1;
    }

    public void ShowPm()
    {
        State = CellState.Mouvement;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 1.0f, 0.8f, 1.0f);
    }

    public enum CellType
    {
        Off = 0,
        On = 1,
        Destructible = 2
    }

    public enum CellState
    {
        None = 0,
        Mouvement = 1,
        AttackRange = 2,
        AttackZone = 3
    }
}
