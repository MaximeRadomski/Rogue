﻿using UnityEngine;

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
    public int SkillVisited;
    public CellType Type;
    public CellState State;
    public bool ReadyToStart;
    public bool IsOccupied;

    private GridBhv _gridBhv;
    private FightSceneBhv _fightSceneBhv;
    private SoundControlerBhv _soundControler;

    private bool _isStretching;
    private bool _isStarting;
    private Vector3 _resetedScale;
    private Vector3 _pressedScale;

    private SpriteRenderer _onSprite;
    private SpriteRenderer _offSprite;
    private SpriteRenderer _overSprite;
    private Vector3 _endPosition;

    public override void SetPrivates()
    {
        base.SetPrivates();
        _fightSceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>();
        _gridBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GridBhv>();
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _isStretching = false;
        _resetedScale = new Vector3(1.0f, 1.0f, 1.0f);
        _pressedScale = new Vector3(1.1f, 1.1f, 1.0f);
        _onSprite = transform.Find("OnSprite").GetComponent<SpriteRenderer>();
        _offSprite = transform.Find("OffSprite").GetComponent<SpriteRenderer>();
        _overSprite = transform.Find("OverSprite").GetComponent<SpriteRenderer>();
        SetStartVisuals();
    }

    private void SetStartVisuals()
    {
        var mapType = GameObject.Find(Constants.GoSceneBhvName).GetComponent<FightSceneBhv>().Journey.Biome.MapType;
        if (Type != CellType.Impracticable)
        {
            if (Type != CellType.Off)
            {
                _offSprite.gameObject.SetActive(false);
                _onSprite.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Biomes/" + mapType + "/CellsOn_" + Random.Range(1, MapsData.NbOnTemplates));
            }
            else
            {
                _offSprite.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Biomes/" + mapType + "/CellsOff_" + Random.Range(0, MapsData.NbOffTemplates));
                _onSprite.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Biomes/" + mapType + "/CellsOn_1");
                _offSprite.sortingOrder = (Constants.GridMax - Y) * 100;
            }
            _onSprite.sortingOrder = Constants.GridMax - Y;
        }        
        else if (Type == CellType.Impracticable)
        {
            _offSprite.gameObject.SetActive(false);
            _onSprite.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Biomes/" + mapType + "/CellsOn_0");
        }
        _overSprite.sprite = null;
        _overSprite.sortingOrder = _onSprite.sortingOrder + 1;
        SetStartAnimation();
    }

    private void SetStartAnimation()
    {
        _endPosition = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y - Random.Range(1.0f, 3.0f), transform.position.z);
        _onSprite.color = Constants.ColorPlainTransparent;
        if (_offSprite.gameObject.activeSelf)
            _offSprite.color = Constants.ColorPlainTransparent;
        StartCoroutine(Helper.ExecuteAfterDelay(Random.Range(0.0f, 0.5f), () =>
        {
            _isStarting = true;
            return true;
        }));
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
        if (Type == CellType.On || Type == CellType.OpponentSpawn)
        {
            var onCellCharacter = _gridBhv.IsOpponentOnCell(X, Y, true);
            if (onCellCharacter != null)
            {
                _fightSceneBhv.ShowCharacterLifeName(onCellCharacter.Character);
            }
            else
            {
                _fightSceneBhv.HideCharacterLifeName();
            }
        }
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
            _fightSceneBhv.AfterPlayerMovement();
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

    public void ResetSkillVisited()
    {
        SkillVisited = Constants.VisitedPmValue;
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
        if (_fightSceneBhv.State == FightState.PlayerTurn)
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
        _overSprite.color = Constants.ColorPlainQuarterTransparent;
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
        SkillVisited = Constants.VisitedSkillValue;
    }

    public void ShowSkillOutOfRange()
    {
        _overSprite.sprite = OverSprites[_skill];
        _overSprite.color = Constants.ColorPlainQuarterTransparent;
    }

    void Update()
    {
        if (_isStretching)
            StretchOnBegin();
        //DEBUG//
        //transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = Visited.ToString();
        //if (Visited > -1)
        //    _overSprite.sprite = OverSprites[_pm];
        if (_isStarting)
        {
            transform.position = Vector3.Lerp(transform.position, _endPosition, 0.15f);
            _onSprite.color = Color.Lerp(_onSprite.color, Constants.ColorPlain, 0.2f);
            if (_offSprite.gameObject.activeSelf)
                _offSprite.color = Color.Lerp(_offSprite.color, Constants.ColorPlain, 0.2f);
            if (Helper.VectorEqualsPrecision(transform.position, _endPosition, 0.01f))
            {
                transform.position = _endPosition;
                _onSprite.color = Constants.ColorPlain;
                if (_offSprite.gameObject.activeSelf)
                    _offSprite.color = Constants.ColorPlain;
                _isStarting = false;
                ReadyToStart = true;
            }
        }
    }

    private void StretchOnBegin()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _resetedScale, 0.2f);
        if (transform.localScale == _resetedScale)
            _isStretching = false;
    }
}
