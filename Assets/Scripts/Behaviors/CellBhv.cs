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

    private GridBhv _gridBhv;
    private CharacterBhv _player;
    private FightSceneBhv _fightSceneBhv;
    private SoundControlerBhv _soundControler;

    private bool _isStretching;
    private Vector3 _resetedScale;
    private Vector3 _pressedScale;

    private void Start()
    {
        SetPrivates();
        SetStartVisuals();
    }

    private void SetPrivates()
    {
        _gridBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GridBhv>();
        _fightSceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>();
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _isStretching = false;
        _resetedScale = new Vector3(1.0f, 1.0f, 1.0f);
        _pressedScale = new Vector3(1.1f, 1.1f, 1.0f);
    }

    private void SetStartVisuals()
    {
        if (Type == CellType.Off)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
        }
        else if (Type == CellType.Impracticable)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.1f);
        }
    }

    public void ShowPlayerSpawn()
    {
        if (Type == CellType.Spawn)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 1.0f, 1.0f);
            State = CellState.Spawn;
        }
        else if (Type == CellType.OpponentSpawn)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.8f, 1.0f, 1.0f);
            State = CellState.Spawn;
        }
    }

    private void GetPlayer()
    {
        _player = GameObject.Find("Player").GetComponent<CharacterBhv>();
    }

    public void BeginAction()
    {
        _isStretching = true;
        transform.localScale = _pressedScale;
        _soundControler.PlaySound(_soundControler.ClickIn);
    }

    public void DoAction()
    {
        if (State == CellState.AttackRange)
        {
            _gridBhv.ShowWeaponZone(X, Y);
        }
    }

    public void EndAction()
    {
        _soundControler.PlaySound(_soundControler.ClickOut);
        if (State == CellState.None)
        {
            _gridBhv.ResetAllCellsZone();
            return;
        }
        else if (State == CellState.Mouvement)
        {
            AskPlayerToMove();
        }
        else if (State == CellState.Spawn && Type == CellType.Spawn)
        {
            _gridBhv.ResetAllCellsSpawn();
            if (_player == null)
                GetPlayer();
            _player.Spawn(X, Y);
        }
        else if (State == CellState.AttackRange || State == CellState.AttackZone)
        {
            _fightSceneBhv.AfterPlayerAttack();
        }
    }

    private void AskPlayerToMove(bool usePm = true)
    {
        if (_player == null)
            GetPlayer();
        if (!_player.IsMoving)
        {
            _player.MoveToPosition(X, Y, usePm);
        }
    }

    public void ResetDisplay()
    {
        if (Type == CellType.Off || Type == CellType.Impracticable)
            return;
        State = CellState.None;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void ResetMovement()
    {
        if (State == CellState.Mouvement)
            State = CellState.None;
    }

    public void ResetZone()
    {
        if (State == CellState.AttackZone)
            ResetDisplay();
    }

    public void ResetVisited()
    {
        Visited = -1;
    }

    public void ResetSpawn()
    {
        if (Type == CellType.Spawn || Type == CellType.OpponentSpawn)
            Type = CellType.On;
    }

    public void ShowPm()
    {
        State = CellState.Mouvement;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 1.0f, 0.8f, 1.0f);
    }

    public void ShowWeaponRange()
    {
        State = CellState.AttackRange;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.8f, 0.8f, 1.0f);
    }

    public void ShowWeaponZone()
    {
        State = CellState.AttackZone;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.5f, 0.5f, 1.0f);
    }

    void Update()
    {
        if (_isStretching)
            StretchOnBegin();
        //DEBUG//
        //transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = Visited.ToString();
    }

    private void StretchOnBegin()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _resetedScale, 0.2f);
        if (transform.localScale == _resetedScale)
            _isStretching = false;
    }
}
