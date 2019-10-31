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
    private GameObject _opponent;

    void Start()
    {
        Application.targetFrameRate = 60;
        State = FightState.Spawn;
        SetPrivates();
        SetButtons();
        InitGrid();
        InitOpponent();
        InitPlayer();
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
        var characterBhv = _opponent.GetComponent<CharacterBhv>();
        characterBhv.X = 0;
        characterBhv.Y = 0;
        characterBhv.Character = RacesData.GetCharacterFromRaceAndLevel((CharacterRace)Random.Range(0, Helpers.EnumCount<CharacterRace>()), 1);
        characterBhv.SetPrivates();
        DisplayCharacterStats(_opponent.name, characterBhv.Character);
    }

    private void InitPlayer()
    {
        var characterObject = Resources.Load<GameObject>("Prefabs/TemplateCharacter");
        _player = Instantiate(characterObject, new Vector2(3.0f, -5.0f), characterObject.transform.rotation);
        _player.name = Constants.GoPlayerName;
        var characterBhv = _player.GetComponent<CharacterBhv>();
        characterBhv.X = 0;
        characterBhv.Y = 0;
        characterBhv.Character = RacesData.GetCharacterFromRaceAndLevel((CharacterRace)Random.Range(0, Helpers.EnumCount<CharacterRace>()), 1, true);
        characterBhv.IsPlayer = true;
        characterBhv.SetPrivates();
        DisplayCharacterStats(_player.name, characterBhv.Character);
    }

    #endregion

    #region GameLife

    private void GameStart()
    {
        _gridBhv.ResetAllCellsDisplay();
        _gridBhv.ResetAllCellsVisited();
        _gridBhv.SpawnOpponent(_opponent);
        _gridBhv.SpawnPlayer();
    }

    private void PlayerTurn()
    {
        State = FightState.PlayerTurn;
        _player.GetComponent<CharacterBhv>().Pm = _player.GetComponent<CharacterBhv>().Character.PmMax;
        _player.GetComponent<CharacterBhv>().Turn++;
        //DEBUG//
        GameObject.Find("TurnCountPlayer").GetComponent<UnityEngine.UI.Text>().text = "Turn Count: " + (_player.GetComponent<CharacterBhv>().Turn).ToString();
        _gridBhv.ShowPm(_player, _opponent);
    }

    private void PassTurn()
    {
        if (_player.GetComponent<CharacterBhv>().Turn == 0)
            return;
        PlayerTurn();
    }

    public void AfterPlayerMovement()
    {
        _gridBhv.ShowPm(_player, _opponent);
    }

    public void AfterPlayerSpawn()
    {
        PlayerTurn();
    }

    public void AfterPlayerAttack()
    {
        _gridBhv.ShowPm(_player, _opponent);
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
        GameObject.Find(name + "Weapon1PA").GetComponent<UnityEngine.UI.Text>().text = "PA:" + character.Weapons[0].CritChancePercent;

        GameObject.Find(name + "Weapon2Name").GetComponent<UnityEngine.UI.Text>().text = character.Weapons[1].Name;
        GameObject.Find(name + "Weapon2Type").GetComponent<UnityEngine.UI.Text>().text = "Type:" + character.Weapons[1].Type;
        GameObject.Find(name + "Weapon2Rarity").GetComponent<UnityEngine.UI.Text>().text = "Rarity:" + character.Weapons[1].Rarity;
        GameObject.Find(name + "Weapon2Damage").GetComponent<UnityEngine.UI.Text>().text = "Damage:" + character.Weapons[1].BaseDamage;
        GameObject.Find(name + "Weapon2Crit").GetComponent<UnityEngine.UI.Text>().text = "Crit:" + character.Weapons[1].CritChancePercent;
        GameObject.Find(name + "Weapon2CritMult").GetComponent<UnityEngine.UI.Text>().text = "Mult:" + character.Weapons[1].CritMultiplierPercent;
        GameObject.Find(name + "Weapon2PA").GetComponent<UnityEngine.UI.Text>().text = "PA:" + character.Weapons[1].CritChancePercent;
    }

    private void ShowWeaponOneRange()
    {
        if (State == FightState.PlayerTurn)
            _gridBhv.ShowWeaponRange(_player.GetComponent<CharacterBhv>(), 0);
    }

    private void ShowWeaponTwoRange()
    {
        if (State == FightState.PlayerTurn)
            _gridBhv.ShowWeaponRange(_player.GetComponent<CharacterBhv>(), 1);
    }

    #endregion

    #region exit

    public static void GoToSwipe()
    {
        SceneManager.LoadScene(Constants.SwipeScene);
    }

    #endregion
}
