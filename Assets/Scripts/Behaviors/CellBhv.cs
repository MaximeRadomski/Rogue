using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBhv : InputBhv
{
    public Sprite[] OverSprites;
    private int _pm = 0;
    private int _weapon = 1;
    private int _skill = 2;
    private int _zone = 3;
    private int _spawn = 4;
    private int _enemySpawn = 5;

    public int X;
    public int Y;
    public int Visited;
    public CellType Type;
    public CellState State;
    public bool ReadyToStart;
    public bool IsOccupied;

    private GridBhv _gridBhv;
    private SoundControlerBhv _soundControler;

    private bool _isStretching;
    private Vector3 _resetedScale;
    private Vector3 _pressedScale;

    private SpriteRenderer _onSprite;
    private SpriteRenderer _offSprite;
    private SpriteRenderer _overSprite;

    private void Start()
    {
        SetPrivates();
        SetStartVisuals();
    }

    public override void SetPrivates()
    {
        base.SetPrivates();
        _gridBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GridBhv>();
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _isStretching = false;
        _resetedScale = new Vector3(1.0f, 1.0f, 1.0f);
        _pressedScale = new Vector3(1.1f, 1.1f, 1.0f);
        _onSprite = transform.Find("OnSprite").GetComponent<SpriteRenderer>();
        _offSprite = transform.Find("OffSprite").GetComponent<SpriteRenderer>();
        _overSprite = transform.Find("OverSprite").GetComponent<SpriteRenderer>();
        ReadyToStart = true;
    }

    private void SetStartVisuals()
    {
        var mapType = GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>().Journey.Biome.MapType;
        if (Type != CellType.Impracticable)
        {
            if (Type != CellType.Off)
            {
                _offSprite.gameObject.SetActive(false);
                _onSprite.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Biomes/" + mapType + "/CellsOn_" + Random.Range(0, MapsData.NbOnTemplates));
            }
            else
            {
                _offSprite.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Biomes/" + mapType + "/CellsOff_" + Random.Range(0, MapsData.NbOffTemplates));
                _onSprite.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Biomes/" + mapType + "/CellsOn_0");
                _offSprite.sortingOrder = Constants.GridMax - Y;
            }
            _onSprite.sortingOrder = Constants.GridMax - Y;
        }        
        else if (Type == CellType.Impracticable)
        {
            _offSprite.gameObject.SetActive(false);
            _onSprite.gameObject.SetActive(false);
        }
        _overSprite.sprite = null;
        _overSprite.sortingOrder = _onSprite.sortingOrder + 1;
    }

    public void ShowPlayerSpawn()
    {
        if (Type == CellType.Spawn)
        {
            _overSprite.sprite = OverSprites[_spawn];
            State = CellState.Spawn;
        }
        else if (Type == CellType.OpponentSpawn)
        {
            _overSprite.sprite = OverSprites[_enemySpawn];
            State = CellState.Spawn;
        }
    }

    public override void BeginAction(Vector2 initialTouchPosition)
    {
        _isStretching = true;
        transform.localScale = _pressedScale;
        _soundControler.PlaySound(_soundControler.ClickIn);
    }

    public override void DoAction(Vector2 touchPosition)
    {
        if (State == CellState.AttackRange)
        {
            _gridBhv.ResetAllCellsZone();
            _gridBhv.ShowWeaponZone(X, Y);
        }
    }

    public override void EndAction(Vector2 lastTouchPosition)
    {
        _soundControler.PlaySound(_soundControler.ClickOut);
        if (State == CellState.None || State == CellState.AttackZone)
        {
            _gridBhv.ResetAllCellsZone();
            return;
        }
        else if (State == CellState.Mouvement)
        {
            _gridBhv.OnPlayerMovementClick(X, Y);
        }
        else if (State == CellState.Spawn && Type == CellType.Spawn)
        {
            _gridBhv.OnPlayerSpawnClick(X, Y);
        }
        else if (State == CellState.AttackRange)
        {
            _gridBhv.OnPlayerAttackClick(X, Y);
        }
        else if (State == CellState.SkillRange)
        {
            _gridBhv.OnPlayerSkillClick(X, Y);
        }
    }

    public override void CancelAction()
    {
        _isStretching = true;
    }

    public void ResetDisplay()
    {
        if (Type == CellType.Off || Type == CellType.Impracticable)
            return;
        State = CellState.None;
        _overSprite.sprite = null;
        _overSprite.color = Constants.ColorPlain;
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
        Visited = Constants.VisitedPmValue;
    }

    public void ResetSpawn()
    {
        if (Type == CellType.Spawn || Type == CellType.OpponentSpawn)
        {
            Type = CellType.On;
            ResetDisplay();
        }
    }

    public void ShowPm()
    {
        State = CellState.Mouvement;
        _overSprite.sprite = OverSprites[_pm];
    }

    public void ShowWeaponRange()
    {
        State = CellState.AttackRange;
        _overSprite.sprite = OverSprites[_weapon];
    }

    public void ShowWeaponOutOfRange()
    {
        _overSprite.sprite = OverSprites[_weapon];
        _overSprite.color = Constants.ColorPlainSemiTransparent;
    }

    public void ShowWeaponZone()
    {
        State = CellState.AttackZone;
        _overSprite.sprite = OverSprites[_zone];
    }

    public void ShowSkillRange()
    {
        State = CellState.SkillRange;
        _overSprite.sprite = OverSprites[_skill];
    }

    public void ShowSkillRangeVisited()
    {
        State = CellState.SkillRange;
        Visited = Constants.VisitedSkillValue;
    }

    public void ShowSkillOutOfRange()
    {
        _overSprite.sprite = OverSprites[_skill];
        _overSprite.color = Constants.ColorPlainSemiTransparent;
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
