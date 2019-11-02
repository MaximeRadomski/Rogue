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
    private GameObject _opponent;
    private CharacterBhv _opponentBhv;

    void Start()
    {
        Application.targetFrameRate = 60;
        State = FightState.Spawn;
        SetPrivates();
        SetButtons();
        InitGrid();
        InitOpponent();
        InitPlayer();
        _playerBhv.SetPrivates();
        _opponentBhv.SetPrivates();
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
        GameObject.Find("ButtonReload").GetComponent<ButtonBhv>().EndActionDelegate = Helpers.ReloadScene;
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToSwipe;
        GameObject.Find("ButtonPassTurn").GetComponent<ButtonBhv>().EndActionDelegate = PassTurn;
        GameObject.Find("ButtonWeapon1").GetComponent<ButtonBhv>().EndActionDelegate = ShowWeaponOneRange;
        GameObject.Find("ButtonWeapon2").GetComponent<ButtonBhv>().EndActionDelegate = ShowWeaponTwoRange;
        GameObject.Find("ButtonPlayer").GetComponent<ButtonBhv>().EndActionDelegate = AfterPlayerMovement;
    }

    private void InitGrid()
    {
        _gridBhv.SetPrivates();
        _gridBhv.InitGrid();
    }

    private void InitOpponent()
    {
        var characterObject = Resources.Load<GameObject>("Prefabs/TemplateCharacter");
        _opponent = Instantiate(characterObject, new Vector2(-3.0f, 5.0f), characterObject.transform.rotation);
        _opponent.name = Constants.GoOpponentName;
        _opponentBhv = _opponent.GetComponent<CharacterBhv>();
        _opponentBhv.X = 0;
        _opponentBhv.Y = 0;
        _opponentBhv.Character = RacesData.GetCharacterFromRaceAndLevel((CharacterRace)Random.Range(0, Helpers.EnumCount<CharacterRace>()), 1);
        DisplayCharacterStats(_opponent.name, _opponentBhv.Character);
    }

    private void InitPlayer()
    {
        var characterObject = Resources.Load<GameObject>("Prefabs/TemplateCharacter");
        _player = Instantiate(characterObject, new Vector2(3.0f, -5.0f), characterObject.transform.rotation);
        _player.name = Constants.GoPlayerName;
        _playerBhv = _player.GetComponent<CharacterBhv>();
        _playerBhv.X = 0;
        _playerBhv.Y = 0;
        _playerBhv.Character = RacesData.GetCharacterFromRaceAndLevel((CharacterRace)Random.Range(0, Helpers.EnumCount<CharacterRace>()), 1, true);
        _playerBhv.IsPlayer = true;
        DisplayCharacterStats(_player.name, _playerBhv.Character);
    }

    #endregion

    #region GameLife

    private void GameStart()
    {
        _gridBhv.ResetAllCellsDisplay();
        _gridBhv.ResetAllCellsVisited();
        _gridBhv.SpawnOpponent(_opponentBhv);
        _gridBhv.SpawnPlayer();
    }

    private void PlayerTurn()
    {
        State = FightState.PlayerTurn;
        _playerBhv.Pa = _playerBhv.Character.PaMax;
        _playerBhv.Pm = _playerBhv.Character.PmMax;
        _playerBhv.Turn++;
        //DEBUG//
        GameObject.Find("TurnCountPlayer").GetComponent<UnityEngine.UI.Text>().text = "Turn Count: " + (_playerBhv.Turn).ToString();
        _gridBhv.ShowPm(_playerBhv, _opponentBhv);
    }

    private void PassTurn()
    {
        if (State == FightState.PlayerTurn)
            PlayerTurn();
    }

    public void AfterPlayerMovement()
    {
        if (State == FightState.PlayerTurn)
            _gridBhv.ShowPm(_playerBhv, _opponentBhv);
    }

    public void AfterPlayerSpawn()
    {
        PlayerTurn();
    }

    public void AfterPlayerAttack(int weaponId, bool hasTouchedOpponent)
    {
        if (hasTouchedOpponent)
        {
            _opponentBhv.TakeDamages(_playerBhv.AttackWithWeapon(weaponId, _opponentBhv, _map));
        }
        else
        {
            _playerBhv.AttackWithWeapon(weaponId, _opponentBhv, _map);
        }
        _gridBhv.ShowPm(_playerBhv, _opponentBhv);
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
            _gridBhv.ShowWeaponRange(_playerBhv, 0, _opponentBhv);
    }

    private void ShowWeaponTwoRange()
    {
        if (State == FightState.PlayerTurn && _playerBhv.Pa >= _playerBhv.Character.Weapons[1].PaNeeded && !_playerBhv.IsMoving)
            _gridBhv.ShowWeaponRange(_playerBhv, 1, _opponentBhv);
    }

    #endregion

    #region exit

    public static void GoToSwipe()
    {
        SceneManager.LoadScene(Constants.SwipeScene);
    }

    #endregion
}
