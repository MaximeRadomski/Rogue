using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightSceneBhv : SceneBhv
{
    public FightState State;
    public GameObject OrderIndicator;

    private Map _map;
    private GridBhv _gridBhv;
    private GameObject _player;
    private CharacterBhv _playerBhv;
    private List<GameObject> _opponents;
    public List<CharacterBhv> OpponentBhvs;

    private int _currentOrderId;
    private List<CharOrder> _orderList;
    private CharacterBhv _currentPlayingCharacterBhv;

    private bool IsWaitingStart;
    private ResourceBarBhv _healthBar;
    private TMPro.TextMeshPro _fightTitle;
    private OrbBhv _orbHp;
    private OrbBhv _orbPa;
    private OrbBhv _orbPm;
    private ButtonBhv _buttonPass;
    private ButtonBhv _buttonRunAway;
    private ButtonBhv _buttonPm;
    private ButtonBhv _buttonWeapon1, _buttonWeapon2;
    private ButtonBhv _buttonSkill1, _buttonSkill2;
    private Vector3 _hidePosition;

    void Start()
    {
        State = FightState.Spawn;
        SetPrivates();
        InitGrid();
        InitCharacters();
        InitCharactersOrder();
        SetButtons();        
        UpdateResources();
        IsWaitingStart = true;
    }

    private void Update()
    {
        if (IsWaitingStart)
        {
            if (_gridBhv.CanStart())
                GameStart();
        }
        if (State == FightState.PlayerTurn)
        {
            if (!_buttonWeapon1.Disabled && _playerBhv.Pa < _playerBhv.Character.Weapons[0].PaNeeded)
                _buttonWeapon1.DisableButton();
            if (!_buttonWeapon2.Disabled && _playerBhv.Pa < _playerBhv.Character.Weapons[1].PaNeeded)
                _buttonWeapon2.DisableButton();
            if (!_buttonSkill1.Disabled && _playerBhv.Pa < _playerBhv.Character.Skills[0].PaNeeded)
                _buttonSkill1.DisableButton();
            if (!_buttonSkill2.Disabled && _playerBhv.Pa < _playerBhv.Character.Skills[1].PaNeeded)
                _buttonSkill2.DisableButton();
        }
    }

    #region Init

    protected override void SetPrivates()
    {
        base.SetPrivates();
        PauseMenu = Instantiator.NewPauseMenu();
        OnRootPreviousScene = Constants.SwipeScene;
        _gridBhv = GetComponent<GridBhv>();
        _map = MapsData.EasyMaps[Random.Range(0, MapsData.EasyMaps.Count)];
        _healthBar = GameObject.Find("HealthBar")?.GetComponent<ResourceBarBhv>();
        _fightTitle = GameObject.Find("FightTitle")?.GetComponent<TMPro.TextMeshPro>();
        _orbHp = GameObject.Find("Hp")?.GetComponent<OrbBhv>();
        _orbPa = GameObject.Find("Pa")?.GetComponent<OrbBhv>();
        _orbPm = GameObject.Find("Pm")?.GetComponent<OrbBhv>();
        _hidePosition = new Vector3(-10.0f, 10.0f, 0.0f);
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonReload").GetComponent<ButtonBhv>().EndActionDelegate = Helper.ReloadScene;
        GameObject.Find("ButtonPause").GetComponent<ButtonBhv>().EndActionDelegate = Pause;
        (_buttonPass = GameObject.Find("ButtonPassTurn").GetComponent<ButtonBhv>()).EndActionDelegate = PassTurn;
        (_buttonRunAway = GameObject.Find("ButtonRunAway").GetComponent<ButtonBhv>()).EndActionDelegate = RunAway;
        (_buttonPm = GameObject.Find("Pm").GetComponent<ButtonBhv>()).EndActionDelegate = OnPlayerPmClick;
        _buttonWeapon1 = GameObject.Find("PlayerWeapon1").GetComponent<ButtonBhv>();
        _buttonWeapon1.EndActionDelegate = ShowWeaponOneRange;
        _buttonWeapon1.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsWeapon_" + _playerBhv.Character.Weapons[0].Type.GetHashCode());
        _buttonWeapon2 = GameObject.Find("PlayerWeapon2").GetComponent<ButtonBhv>();
        _buttonWeapon2.EndActionDelegate = ShowWeaponTwoRange;
        _buttonWeapon2.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsWeapon_" + _playerBhv.Character.Weapons[1].Type.GetHashCode());
        _buttonSkill1 = GameObject.Find("PlayerSkill1").GetComponent<ButtonBhv>();
        _buttonSkill1.EndActionDelegate = ClickSkill1;
        _buttonSkill1.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsSkill_" + _playerBhv.Character.Skills[0].IconId);
        _buttonSkill2 = GameObject.Find("PlayerSkill2").GetComponent<ButtonBhv>();
        _buttonSkill2.EndActionDelegate = ClickSkill2;
        _buttonSkill2.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsSkill_" + _playerBhv.Character.Skills[1].IconId);
        PauseMenu.Buttons[0].EndActionDelegate = Resume;
        PauseMenu.TextMeshes[0].text = "Resume";
        PauseMenu.Buttons[1].EndActionDelegate = GiveUp;
        PauseMenu.TextMeshes[1].text = "Give Up";
        PauseMenu.Buttons[2].EndActionDelegate = Settings;
        PauseMenu.TextMeshes[2].text = "Settings";
        PauseMenu.Buttons[3].EndActionDelegate = Exit;
        PauseMenu.TextMeshes[3].text = "Exit";
        PauseMenu.Buttons[4].gameObject.SetActive(false);

        bool isFramePlayerSet = false;
        //bool isFirstOpponentSet = false;
        for (int i = 0; i < _orderList.Count; ++i)
        {
            var tmpCharacterBhv = GetCharacterBhvFromOrderId(_orderList[i].Id);
            var tmpFrameX = new Vector3(0.944f * (isFramePlayerSet ? i - 1 : i), 0.0f);//Space between frames
            var isPlayer = false;
            if (tmpCharacterBhv.Character.IsPlayer)
            {
                tmpFrameX = new Vector3(_buttonPm.transform.position.x + 0.64f, _buttonPm.transform.position.y);//Space next to Pm
                isFramePlayerSet = true;
                isPlayer = true;
            }
            //else if (!isFirstOpponentSet)
            //{
            //    ShowCharacterLifeName(tmpCharacterBhv.Character);
            //    isFirstOpponentSet = true;
            //}
            var tmpFrameInstance = Instantiator.NewCharacterFrame(tmpCharacterBhv.Character.Race, tmpFrameX, tmpCharacterBhv.OrderId, isPlayer);
            tmpCharacterBhv.Character.Frame = tmpFrameInstance;
            tmpFrameInstance.GetComponent<ButtonBhv>().EndActionDelegate = OnPlayerCharacterClick;
            Instantiator.LoadCharacterSkin(tmpCharacterBhv.Character, tmpFrameInstance.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject);
            tmpFrameInstance.transform.Find("Order").GetComponent<TMPro.TextMeshPro>().text = (i + 1).ToString();
        }
        var firstPlayingFrame = GameObject.Find("FrameCharacter" + _orderList[0].Id);
        OrderIndicator.transform.parent = firstPlayingFrame.transform;
        OrderIndicator.transform.position = firstPlayingFrame.transform.position;
        _fightTitle.text = "Spawning Phase";
        HideCharacterLifeName();
        ManagePlayerButtons();
    }

    private void InitGrid()
    {
        _gridBhv.SetPrivates();
        _gridBhv.InitGrid(_map);
    }

    private void InitCharacters()
    {
        InitOpponents();
        InitPlayer();
        _playerBhv.SetPrivates();
        foreach (var opponentBhv in OpponentBhvs)
            opponentBhv.SetPrivates();
    }

    private void InitOpponents()
    {
        int nbOpponents = PlayerPrefs.GetInt(Constants.PpNbOpponents);
        _opponents = new List<GameObject>();
        OpponentBhvs = new List<CharacterBhv>();
        for (int i = 0; i < nbOpponents; ++i)
        {
            _opponents.Add(Instantiator.NewCharacterGameObject(Constants.PpOpponent + i, false, i.ToString()));
            OpponentBhvs.Add(_opponents[i].GetComponent<CharacterBhv>());
        }
        //DisplayCharacterStats(_opponent.name, _opponentBhv.Character);
    }

    private void InitPlayer()
    {
        _player = Instantiator.NewCharacterGameObject(Constants.PpPlayer, true);
        _playerBhv = _player.GetComponent<CharacterBhv>();
        //DisplayCharacterStats(_player.name, _playerBhv.Character);
    }

    private void InitCharactersOrder()
    {
        int orderId = 0;
        _orderList = new List<CharOrder>();
        CalculateInitiative(_playerBhv, orderId++);
        foreach (var opponentBhv in OpponentBhvs)
            CalculateInitiative(opponentBhv, orderId++);
        int i = 0;
        while (i < _orderList.Count)
        {
            if (i + 1 < _orderList.Count &&
                _orderList[i].Initiative < _orderList[i + 1].Initiative)
            {
                var tmpCharOrder = new CharOrder(0,0);
                tmpCharOrder.Id = _orderList[i].Id;
                tmpCharOrder.Initiative = _orderList[i].Initiative;
                _orderList[i].Id = _orderList[i + 1].Id;
                _orderList[i].Initiative = _orderList[i + 1].Initiative;
                _orderList[i + 1].Id = tmpCharOrder.Id;
                _orderList[i + 1].Initiative = tmpCharOrder.Initiative;
                i = 0;
            }
            else
                ++i;
        }
        _currentOrderId = -1; //Because Spawn
    }

    private CharacterBhv GetCharacterBhvFromOrderId(int id)
    {
        if (_playerBhv.OrderId == id)
            return _playerBhv;
        foreach (var opponentBhv in OpponentBhvs)
        {
            if (opponentBhv.OrderId == id)
                return opponentBhv;
        }
        return OpponentBhvs[0];
    }

    private int CalculateInitiative(CharacterBhv characterBhv, int orderId = -1)
    {
        int initiative = 0;
        initiative += (characterBhv.Character.Level - 1) * RacesData.InitiativeLevel;
        if (characterBhv.Character.Weapons != null)
        {
            if (characterBhv.Character.Weapons.Count > 0)
                initiative += characterBhv.Character.Weapons[0].Rarity.GetHashCode() * RacesData.InitiativeWeapon;
            if (characterBhv.Character.Weapons.Count > 1)
                initiative += characterBhv.Character.Weapons[1].Rarity.GetHashCode() * RacesData.InitiativeWeapon;
        }
        if (characterBhv.Character.Skills != null)
        {
            if (characterBhv.Character.Skills.Count > 0)
                initiative += characterBhv.Character.Skills[0].Rarity.GetHashCode() * RacesData.InitiativeSkill;
            if (characterBhv.Character.Skills.Count > 1)
                initiative += characterBhv.Character.Skills[1].Rarity.GetHashCode() * RacesData.InitiativeSkill;
        }
        initiative += characterBhv.Character.Hp / characterBhv.Character.HpMax * 100;
        initiative += _map.Type == characterBhv.Character.StrongIn ? RacesData.InitiativeStrongIn : 0;
        if (orderId != -1)
        {
            characterBhv.Initiative = initiative;
            characterBhv.OrderId = orderId;
            _orderList.Add(new CharOrder(orderId, initiative));
        }
        return initiative;
    }

    #endregion

    #region GameLife

    private void GameStart()
    {
        _gridBhv.ResetAllCellsDisplay();
        _gridBhv.ResetAllCellsVisited();
        _gridBhv.SpawnOpponent(OpponentBhvs);
        _gridBhv.SpawnPlayer();
        IsWaitingStart = false;
        Instantiator.NewOverTitleMap(_map, null, Direction.Left);
    }

    private void NextTurn()
    {
        bool? lastCharacterIsPlayer = _currentPlayingCharacterBhv?.Character.IsPlayer ?? null;
        if (++_currentOrderId >= _orderList.Count)
            _currentOrderId = 0;
        _currentPlayingCharacterBhv = GetCharacterBhvFromOrderId(_orderList[_currentOrderId].Id);
        var currentPlayingFrame = GameObject.Find("FrameCharacter" + _currentPlayingCharacterBhv.OrderId);
        OrderIndicator.transform.parent = currentPlayingFrame.transform;
        OrderIndicator.transform.position = currentPlayingFrame.transform.position;
        if (!_currentPlayingCharacterBhv.Character.IsPlayer)
            ShowCharacterLifeName(_currentPlayingCharacterBhv.Character);
        else
            HideCharacterLifeName();
        //DEBUG
        //_currentPlayingCharacterBhv = _playerBhv;
        foreach (var skill in _currentPlayingCharacterBhv.Character.Skills)
        {
            if (skill != null)
                skill.OnStartTurn();
        }
        _currentPlayingCharacterBhv.Turn++;
        if (_currentOrderId == 0)
            _fightTitle.text = _map.Name + ":  <material=\"LongWhite\">" + Helper.GetOrdinal(GetCharacterBhvFromOrderId(_orderList[0].Id).Turn) + " Turn</material>";
        UpdateResources();

        if (_currentPlayingCharacterBhv.Character.IsPlayer)
        {
            State = FightState.PlayerTurn;
            ManagePlayerButtons();
            _gridBhv.ShowPm(_currentPlayingCharacterBhv, _currentPlayingCharacterBhv.OpponentBhvs);
            Instantiator.NewOverTitleFight("Player Turn", null, Direction.Left);
        }            
        else
        {
            State = FightState.OpponentTurn;
            ManagePlayerButtons();
            if (lastCharacterIsPlayer == true || lastCharacterIsPlayer == null)
                Instantiator.NewOverTitleFight("Opponent" + (_orderList.Count > 2 ? "s" : "") + " Turn", AfterTitleFight, Direction.Left);
            else
                AfterTitleFight(true);
            
            object AfterTitleFight(bool result)
            {
                _currentPlayingCharacterBhv.Ai.StartThinking();
                return result;
            }
        }
        
    }

    public void ManagePlayerButtons()
    {
        if (State == FightState.PlayerTurn)
        {
            _buttonPass.EnableButton();
            _buttonRunAway.EnableButton();
            _buttonWeapon1.EnableButton();
            _buttonWeapon2.EnableButton();
            var skill = _playerBhv.Character.Skills[0];
            if (!skill.IsUnderCooldown())
            {
                _buttonSkill1.EnableButton();
                _buttonSkill1.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = string.Empty;
            }
            else
            {
                _buttonSkill1.DisableButton();
                _buttonSkill1.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = skill.Cooldown <= skill.CooldownMax && skill.Cooldown > 0 ? skill.Cooldown.ToString() : string.Empty;
            }
            skill = _playerBhv.Character.Skills[1];
            if (!skill.IsUnderCooldown())
            {
                _buttonSkill2.EnableButton();
                _buttonSkill2.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = string.Empty;
            }
            else
            {
                _buttonSkill2.DisableButton();
                _buttonSkill2.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = skill.Cooldown <= skill.CooldownMax && skill.Cooldown > 0 ? skill.Cooldown.ToString() : string.Empty;
            }
        }
        else
        {
            _buttonPass.DisableButton();
            _buttonRunAway.DisableButton();
            _buttonWeapon1.DisableButton();
            _buttonWeapon2.DisableButton();
            _buttonSkill1.DisableButton();
            _buttonSkill2.DisableButton();
        }
    }

    public void UpdateResources()
    {
        _orbHp.UpdateContent(_playerBhv.Character.Hp, _playerBhv.Character.HpMax, Instantiator, TextType.Hp);
        _orbPa.UpdateContent(_playerBhv.Pa, _playerBhv.Character.PaMax, Instantiator, TextType.Pa);
        _orbPm.UpdateContent(_playerBhv.Pm, _playerBhv.Character.PmMax, Instantiator, TextType.Pm);
    }

    public void PassTurn()
    {
        if (State == FightState.Spawn)
        {
            _gridBhv.ResetAllCellsSpawn();
            NextTurn();
        }
        else
        {
            if (_currentPlayingCharacterBhv.Character.IsPlayer && State != FightState.PlayerTurn)
                return;
            foreach (var skill in _currentPlayingCharacterBhv.Character.Skills)
            {
                if (skill != null)
                    skill.OnEndTurn();
            }
            _currentPlayingCharacterBhv.ResetPa();
            _currentPlayingCharacterBhv.ResetPm();
            NextTurn();
        }
    }

    public void OnPlayerMovementClick(int x, int y)
    {
        if (State != FightState.PlayerTurn)
            return;
        _gridBhv.ResetAllCellsDisplay();
        _playerBhv.MoveToPosition(x, y, true);
    }

    public void AfterPlayerMovement()
    {
        if (State != FightState.PlayerTurn)
            return;
        _gridBhv.ShowPm(_playerBhv, OpponentBhvs);
    }

    public void OnPlayerSpawnClick(int x, int y)
    {
        _playerBhv.Spawn(x, y);
        _buttonPass.EnableButton();
        MakeOpponentsLookPlayer();
        MakePlayerLookOpponents();
        //NextTurn();
    }

    private void MakeOpponentsLookPlayer()
    {
        foreach (var opponent in OpponentBhvs)
        {
            opponent.SkinContainer.OrientToTarget(opponent.X - _playerBhv.X);
        }
    }

    private void MakePlayerLookOpponents()
    {
        if (OpponentBhvs.Count == 1)
        {
            _playerBhv.SkinContainer.OrientToTarget(_playerBhv.X - OpponentBhvs[0].X);
        }
        else
        {
            _playerBhv.SkinContainer.OrientToTarget(_playerBhv.X - 3); //Because [3,3] is the middle of the map
        }
    }

    public void OnPlayerAttackClick(int weaponId, List<CharacterBhv> touchedOpponents, Vector3 touchedPosition = default(Vector3))
    {
        if (touchedOpponents != null)
        {
            foreach (var opponentBhv in touchedOpponents)
            {
                opponentBhv.TakeDamages(_playerBhv.AttackWithWeapon(weaponId, opponentBhv, _map, touchedPosition: touchedPosition));
            }
        }
        else
        {
            _playerBhv.AttackWithWeapon(weaponId, null, _map, touchedPosition: touchedPosition);
        }
        _gridBhv.ShowPm(_playerBhv, OpponentBhvs);
    }

    public void OnPlayerPmClick()
    {
        if (State != FightState.PlayerTurn)
            return;
        _gridBhv.ShowPm(_playerBhv, OpponentBhvs);
    }

    public void OnPlayerSkillClick(int skillId, int x, int y)
    {
        _playerBhv.Character.Skills[skillId].Activate(x, y);
        _gridBhv.ShowPm(_playerBhv, OpponentBhvs);
    }

    public void OnPlayerCharacterClick()
    {
        int orderId = int.Parse(Constants.LastEndActionClickedName.Substring(Helper.CharacterAfterString(Constants.LastEndActionClickedName, "FrameCharacter")));
        var character = GetCharacterBhvFromOrderId(orderId).Character;
        //ShowCharacterLifeName(character);
        Instantiator.NewPopupCharacterStats(character, null);
    }

    #endregion

    #region Display

    public void DisplayCharacterStats(string name, Character character)
    {
        GameObject.Find(name + "Name").GetComponent<UnityEngine.UI.Text>().text = "Name:" + character.Name;
        GameObject.Find(name + "Gender").GetComponent<UnityEngine.UI.Text>().text = "Gender:" + character.Gender;
        GameObject.Find(name + "Race").GetComponent<UnityEngine.UI.Text>().text = "Race:" + character.Race;
        GameObject.Find(name + "Level").GetComponent<UnityEngine.UI.Text>().text = "Level:" + character.Level;
        GameObject.Find(name + "Gender").GetComponent<UnityEngine.UI.Text>().text = "Gender:" + character.Gender;
        GameObject.Find(name + "HpMax").GetComponent<UnityEngine.UI.Text>().text = "HP Max:" + character.HpMax;
        GameObject.Find(name + "Pa").GetComponent<UnityEngine.UI.Text>().text = "Pa:" + character.PaMax;
        GameObject.Find(name + "Pm").GetComponent<UnityEngine.UI.Text>().text = "Pm:" + character.PmMax;

        GameObject.Find(name + "Weapon1Name").GetComponent<UnityEngine.UI.Text>().text = character.Weapons[0].Name;
        GameObject.Find(name + "Weapon1Type").GetComponent<UnityEngine.UI.Text>().text = "Type:" + character.Weapons[0].Type;
        GameObject.Find(name + "Weapon1Rarity").GetComponent<UnityEngine.UI.Text>().text = "Rarity:" + character.Weapons[0].Rarity;
        GameObject.Find(name + "Weapon1Damage").GetComponent<UnityEngine.UI.Text>().text = "Damage:" + character.Weapons[0].BaseDamage;
        GameObject.Find(name + "Weapon1Crit").GetComponent<UnityEngine.UI.Text>().text = "Crit:" + character.Weapons[0].CritChancePercent;
        GameObject.Find(name + "Weapon1CritMult").GetComponent<UnityEngine.UI.Text>().text = "Mult:" + character.Weapons[0].CritMultiplierPercent;
        GameObject.Find(name + "Weapon1PA").GetComponent<UnityEngine.UI.Text>().text = "PA:" + character.Weapons[0].PaNeeded;

        GameObject.Find(name + "Weapon2Name").GetComponent<UnityEngine.UI.Text>().text = character.Weapons[1].Name;
        GameObject.Find(name + "Weapon2Type").GetComponent<UnityEngine.UI.Text>().text = "Type:" + character.Weapons[1].Type;
        GameObject.Find(name + "Weapon2Rarity").GetComponent<UnityEngine.UI.Text>().text = "Rarity:" + character.Weapons[1].Rarity;
        GameObject.Find(name + "Weapon2Damage").GetComponent<UnityEngine.UI.Text>().text = "Damage:" + character.Weapons[1].BaseDamage;
        GameObject.Find(name + "Weapon2Crit").GetComponent<UnityEngine.UI.Text>().text = "Crit:" + character.Weapons[1].CritChancePercent;
        GameObject.Find(name + "Weapon2CritMult").GetComponent<UnityEngine.UI.Text>().text = "Mult:" + character.Weapons[1].CritMultiplierPercent;
        GameObject.Find(name + "Weapon2PA").GetComponent<UnityEngine.UI.Text>().text = "PA:" + character.Weapons[1].PaNeeded;
    }

    private void ShowWeaponOneRange()
    {
        if (State == FightState.PlayerTurn && _playerBhv.Pa >= _playerBhv.Character.Weapons[0].PaNeeded && !_playerBhv.IsMoving)
            _gridBhv.ShowWeaponRange(_playerBhv, 0, OpponentBhvs);
        if (Constants.DoubleClick)
            Instantiator.NewPopupCharacterStats(_playerBhv.Character, null, false, 1);
    }

    private void ShowWeaponTwoRange()
    {
        if (State == FightState.PlayerTurn && _playerBhv.Pa >= _playerBhv.Character.Weapons[1].PaNeeded && !_playerBhv.IsMoving)
            _gridBhv.ShowWeaponRange(_playerBhv, 1, OpponentBhvs);
        if (Constants.DoubleClick)
            Instantiator.NewPopupCharacterStats(_playerBhv.Character, null, false, 2);
    }

    private void ClickSkill1()
    {
        if (State == FightState.PlayerTurn && _playerBhv.Character.Skills != null && _playerBhv.Character.Skills.Count >= 1
            && _playerBhv.Pa >= _playerBhv.Character.Skills[0].PaNeeded && !_playerBhv.IsMoving)
            _playerBhv.Character.Skills[0].OnClick();
        if (Constants.DoubleClick)
            Instantiator.NewPopupCharacterStats(_playerBhv.Character, null, false, 3);
    }

    private void ClickSkill2()
    {
        if (State == FightState.PlayerTurn && _playerBhv.Character.Skills != null && _playerBhv.Character.Skills.Count >= 2
            && _playerBhv.Pa >= _playerBhv.Character.Skills[1].PaNeeded && !_playerBhv.IsMoving)
            _playerBhv.Character.Skills[1].OnClick();
        if (Constants.DoubleClick)
            Instantiator.NewPopupCharacterStats(_playerBhv.Character, null, false, 4);
    }

    #endregion

    public void ShowCharacterLifeName(Character character)
    {
        _fightTitle.transform.position = _hidePosition;
        _healthBar.GetComponent<PositionBhv>().UpdatePositions();
        _healthBar.UpdateContent(character.Hp, character.HpMax, character.Name, character.Frame);
    }

    public void HideCharacterLifeName()
    {
        _healthBar.transform.position = _hidePosition;
        _fightTitle.GetComponent<PositionBhv>().UpdatePositions();
    }

    public virtual void RunAway()
    {
        Instantiator.NewPopupYesNo(Constants.YesNoTitle,
            "You have " + (Soul.RunAwayPercent + Journey.RunAwayPercent) + "% chance of running away.\nDo you risk it?"
            , Constants.Cancel, "Risk it!", OnRiskRunningAway);

        object OnRiskRunningAway(bool result)
        {
            if (result)
            {
                var rand = Random.Range(0, 100);
                if (rand < Soul.RunAwayPercent + Journey.RunAwayPercent)
                {
                    Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "RUNNING AWAY", 2.0f, TransitionRunAway, reverse: true);
                    object TransitionRunAway(bool transResult)
                    {
                        NavigationService.LoadPreviousScene(OnRootPreviousScene);
                        return transResult;
                    }
                }
                else
                {
                    Instantiator.NewSnack("Your attempt to flee fails");
                    PassTurn();
                }
            }
            return result;
        }
    }
}
