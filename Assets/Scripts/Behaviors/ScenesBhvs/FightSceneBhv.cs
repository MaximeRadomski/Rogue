using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightSceneBhv : SceneBhv
{
    public FightState State;
    public Journey Journey;

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

    private OrbBhv _orbHp;
    private OrbBhv _orbPa;
    private OrbBhv _orbPm;

    void Start()
    {
        State = FightState.Spawn;
        SetPrivates();
        InitGrid();
        InitCharacters();
        SetButtons();
        InitCharactersOrder();
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
    }

    #region Init

    protected override void SetPrivates()
    {
        base.SetPrivates();
        Journey = PlayerPrefsHelper.GetJourney();
        OnRootPreviousScene = Constants.SwipeScene;
        _gridBhv = GetComponent<GridBhv>();
        _map = MapsData.EasyMaps[Random.Range(0, MapsData.EasyMaps.Count)];
        _orbHp = GameObject.Find("Hp")?.GetComponent<OrbBhv>();
        _orbPa = GameObject.Find("Pa")?.GetComponent<OrbBhv>();
        _orbPm = GameObject.Find("Pm")?.GetComponent<OrbBhv>();
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonReload").GetComponent<ButtonBhv>().EndActionDelegate = Helper.ReloadScene;
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToSwipe;
        GameObject.Find("ButtonPassTurn").GetComponent<ButtonBhv>().EndActionDelegate = PassTurn;
        GameObject.Find("Pm").GetComponent<ButtonBhv>().EndActionDelegate = OnPlayerPmClick;
        GameObject.Find("PlayerCharacter").GetComponent<ButtonBhv>().EndActionDelegate = OnPlayerCharacterClick;
        var tmpButton = GameObject.Find("PlayerWeapon1");
        tmpButton.GetComponent<ButtonBhv>().EndActionDelegate = ShowWeaponOneRange;
        tmpButton.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsWeapon_" + _playerBhv.Character.Weapons[0].Type.GetHashCode());
        tmpButton = GameObject.Find("PlayerWeapon2");
        tmpButton.GetComponent<ButtonBhv>().EndActionDelegate = ShowWeaponTwoRange;
        tmpButton.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsWeapon_" + _playerBhv.Character.Weapons[1].Type.GetHashCode());
        tmpButton = GameObject.Find("PlayerSkill1");
        tmpButton.GetComponent<ButtonBhv>().EndActionDelegate = ClickSkill1;
        tmpButton.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsSkill_" + _playerBhv.Character.Skills[0].IconId);
        tmpButton = GameObject.Find("PlayerSkill2");
        tmpButton.GetComponent<ButtonBhv>().EndActionDelegate = ClickSkill2;
        tmpButton.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/ButtonsSkill_" + _playerBhv.Character.Skills[1].IconId);
        Instantiator.LoadCharacterSkin(_playerBhv.Character, GameObject.Find("CharacterSkinContainer"));
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
    }

    private void NextTurn()
    {
        if (++_currentOrderId >= _orderList.Count)
            _currentOrderId = 0;
        _currentPlayingCharacterBhv = GetCharacterBhvFromOrderId(_orderList[_currentOrderId].Id);
        //DEBUG
        //_currentPlayingCharacterBhv = _playerBhv;
        foreach (var skill in _currentPlayingCharacterBhv.Character.Skills)
        {
            if (skill != null)
                skill.OnStartTurn();
        }
        _currentPlayingCharacterBhv.ResetPa();
        _currentPlayingCharacterBhv.ResetPm();
        _currentPlayingCharacterBhv.Turn++;
        UpdateResources();

        if (_currentPlayingCharacterBhv.Character.IsPlayer)
        {
            State = FightState.PlayerTurn;
            _gridBhv.ShowPm(_currentPlayingCharacterBhv, _currentPlayingCharacterBhv.OpponentBhvs);
        }            
        else
        {
            State = FightState.OpponentTurn;
            _currentPlayingCharacterBhv.Ai.StartThinking();
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
            foreach (var skill in _currentPlayingCharacterBhv.Character.Skills)
            {
                if (skill != null)
                    skill.OnEndTurn();
            }
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
        //NextTurn();
    }

    public void OnPlayerAttackClick(int weaponId, List<CharacterBhv> touchedOpponents)
    {
        if (touchedOpponents != null)
        {
            foreach (var opponentBhv in touchedOpponents)
            {
                opponentBhv.TakeDamages(_playerBhv.AttackWithWeapon(weaponId, opponentBhv, _map));
            }
        }
        else
        {
            _playerBhv.AttackWithWeapon(weaponId, null, _map);
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
    }

    public void OnPlayerCharacterClick()
    {
        Instantiator.NewPopupCharacterStats(_playerBhv.Character, null);
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
    }

    private void ShowWeaponTwoRange()
    {
        if (State == FightState.PlayerTurn && _playerBhv.Pa >= _playerBhv.Character.Weapons[1].PaNeeded && !_playerBhv.IsMoving)
            _gridBhv.ShowWeaponRange(_playerBhv, 1, OpponentBhvs);
    }

    private void ClickSkill1()
    {
        if (State == FightState.PlayerTurn && _playerBhv.Character.Skills != null && _playerBhv.Character.Skills.Count >= 1
            && _playerBhv.Pa >= _playerBhv.Character.Skills[0].PaNeeded && !_playerBhv.IsMoving)
            _playerBhv.Character.Skills[0].OnClick();
    }

    private void ClickSkill2()
    {
        if (State == FightState.PlayerTurn && _playerBhv.Character.Skills != null && _playerBhv.Character.Skills.Count >= 2
            && _playerBhv.Pa >= _playerBhv.Character.Skills[1].PaNeeded && !_playerBhv.IsMoving)
            _playerBhv.Character.Skills[1].OnClick();
    }

    #endregion

    public void ShowCharacterStats(Character character)
    {
        Instantiator.NewPopupCharacterStats(character, null);
    }

    #region exit

    public void GoToSwipe()
    {
        NavigationService.LoadPreviousScene(OnRootPreviousScene);
    }

    #endregion
}
