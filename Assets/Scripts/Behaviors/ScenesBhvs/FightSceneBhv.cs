using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightSceneBhv : MonoBehaviour
{
    public FightState State;

    private Map _map;
    private GridBhv _gridBhv;
    private GameObject _player;
    private CharacterBhv _playerBhv;
    private List<GameObject> _opponents;
    private List<CharacterBhv> _opponentBhvs;

    void Start()
    {
        Application.targetFrameRate = 60;
        State = FightState.Spawn;
        SetPrivates();
        SetButtons();
        InitGrid();
        InitCharacters();
        GameStart();
    }

    #region Init

    private void SetPrivates()
    {
        _gridBhv = GetComponent<GridBhv>();
        _map = MapsData.EasyMaps[Random.Range(0, MapsData.EasyMaps.Count)];
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonReload").GetComponent<ButtonBhv>().EndActionDelegate = Helper.ReloadScene;
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToSwipe;
        GameObject.Find("ButtonPassTurn").GetComponent<ButtonBhv>().EndActionDelegate = PassTurn;
        GameObject.Find("PlayerCharacter").GetComponent<ButtonBhv>().EndActionDelegate = OnPlayerCharacterClick;
        GameObject.Find("PlayerWeapon1").GetComponent<ButtonBhv>().EndActionDelegate = ShowWeaponOneRange;
        GameObject.Find("PlayerWeapon2").GetComponent<ButtonBhv>().EndActionDelegate = ShowWeaponTwoRange;
        GameObject.Find("PlayerSkill").GetComponent<ButtonBhv>().EndActionDelegate = ClickSkill1;

    }

    private void InitGrid()
    {
        _gridBhv.SetPrivates();
        _gridBhv.InitGrid();
    }

    private void InitCharacters()
    {
        InitOpponents();
        InitPlayer();
        _playerBhv.SetPrivates();
        foreach (var opponentBhv in _opponentBhvs)
            opponentBhv.SetPrivates();
    }

    private void InitOpponents()
    {
        int nbOpponents = PlayerPrefs.GetInt(Constants.PpNbOpponents);
        _opponents = new List<GameObject>();
        _opponentBhvs = new List<CharacterBhv>();
        for (int i = 0; i < nbOpponents; ++i)
        {
            _opponents.Add(Instantiator.NewCharacterGameObject(Constants.PpOpponent + i, false, i.ToString()));
            _opponentBhvs.Add(_opponents[i].GetComponent<CharacterBhv>());
        }
        //DisplayCharacterStats(_opponent.name, _opponentBhv.Character);
    }

    private void InitPlayer()
    {
        _player = Instantiator.NewCharacterGameObject(Constants.PpPlayer, true);
        _playerBhv = _player.GetComponent<CharacterBhv>();
        //DisplayCharacterStats(_player.name, _playerBhv.Character);
    }

    #endregion

    #region GameLife

    private void GameStart()
    {
        _gridBhv.ResetAllCellsDisplay();
        _gridBhv.ResetAllCellsVisited();
        _gridBhv.SpawnOpponent(_opponentBhvs);
        _gridBhv.SpawnPlayer();
    }

    private void PlayerTurn()
    {
        foreach (var skill in _playerBhv.Character.Skills)
        {
            if (skill != null)
                skill.OnStartTurn();
        }
        State = FightState.PlayerTurn;
        _playerBhv.Pa = _playerBhv.Character.PaMax;
        _playerBhv.Pm = _playerBhv.Character.PmMax;
        _playerBhv.Turn++;
        _gridBhv.ShowPm(_playerBhv, _opponentBhvs);
    }

    private void PassTurn()
    {
        if (State == FightState.PlayerTurn)
        {
            foreach (var skill in _playerBhv.Character.Skills)
            {
                if (skill != null)
                    skill.OnEndTurn();
            }
            PlayerTurn();
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
        _gridBhv.ShowPm(_playerBhv, _opponentBhvs);
    }

    public void OnPlayerSpawnClick(int x, int y)
    {
        _playerBhv.Spawn(x, y);
        PlayerTurn();
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
        _gridBhv.ShowPm(_playerBhv, _opponentBhvs);
    }

    public void OnPlayerCharacterClick()
    {
        _gridBhv.ShowPm(_playerBhv, _opponentBhvs);
    }

    public void OnPlayerSkillClick(int skillId, int x, int y)
    {
        _playerBhv.Character.Skills[skillId].Activate(x, y);
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
            _gridBhv.ShowWeaponRange(_playerBhv, 0, _opponentBhvs);
    }

    private void ShowWeaponTwoRange()
    {
        if (State == FightState.PlayerTurn && _playerBhv.Pa >= _playerBhv.Character.Weapons[1].PaNeeded && !_playerBhv.IsMoving)
            _gridBhv.ShowWeaponRange(_playerBhv, 1, _opponentBhvs);
    }

    private void ClickSkill1()
    {
        if (State == FightState.PlayerTurn && _playerBhv.Character.Skills != null && _playerBhv.Character.Skills.Count >= 1
            && _playerBhv.Pa >= _playerBhv.Character.Skills[0].PaNeeded && !_playerBhv.IsMoving)
            _playerBhv.Character.Skills[0].OnClick();
    }

    #endregion

    #region exit

    public static void GoToSwipe()
    {
        SceneManager.LoadScene(Constants.SwipeScene);
    }

    #endregion
}
