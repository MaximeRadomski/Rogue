using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceChoiceSceneBhv : MonoBehaviour
{
    private Character _playerCharacter;

    void Start()
    {
        Application.targetFrameRate = 60;
        SetPrivates();
        SetButtons();
    }

    private void SetPrivates()
    {
        
    }

    private void SetButtons()
    {
        GameObject.Find("Human").GetComponent<ButtonBhv>().EndActionDelegate = SelectHuman;
        GameObject.Find("Gobelin").GetComponent<ButtonBhv>().EndActionDelegate = SelectGobelin;
        GameObject.Find("Elf").GetComponent<ButtonBhv>().EndActionDelegate = SelectElf;
        GameObject.Find("Dwarf").GetComponent<ButtonBhv>().EndActionDelegate = SelectDwarf;
        GameObject.Find("Orc").GetComponent<ButtonBhv>().EndActionDelegate = SelectOrc;
        GameObject.Find("Go").GetComponent<ButtonBhv>().EndActionDelegate = GoToSwipeScene;
    }

    private void SelectHuman()
    {
        _playerCharacter = RacesData.GetCharacterFromRaceAndLevel(CharacterRace.Human, 1, true);
        DisplayStats();
    }

    private void SelectGobelin()
    {
        _playerCharacter = RacesData.GetCharacterFromRaceAndLevel(CharacterRace.Gobelin, 1, true);
        DisplayStats();
    }

    private void SelectElf()
    {
        _playerCharacter = RacesData.GetCharacterFromRaceAndLevel(CharacterRace.Elf, 1, true);
        DisplayStats();
    }

    private void SelectDwarf()
    {
        _playerCharacter = RacesData.GetCharacterFromRaceAndLevel(CharacterRace.Dwarf, 1, true);
        DisplayStats();
    }

    private void SelectOrc()
    {
        _playerCharacter = RacesData.GetCharacterFromRaceAndLevel(CharacterRace.Orc, 1, true);
        DisplayStats();
    }

    private void DisplayStats()
    {
        GameObject.Find("RaceName").GetComponent<UnityEngine.UI.Text>().text = _playerCharacter.Race.ToString();
        GameObject.Find("Weapons").GetComponent<UnityEngine.UI.Text>().text = _playerCharacter.Weapons[0].Type.ToString() + " + " +
                                                                              _playerCharacter.Weapons[1].Type.ToString();
        GameObject.Find("Hp").GetComponent<UnityEngine.UI.Text>().text = "Health: " + _playerCharacter.HpMax;
        GameObject.Find("Pa").GetComponent<UnityEngine.UI.Text>().text = "Pa: " + _playerCharacter.PaMax;
        GameObject.Find("Pm").GetComponent<UnityEngine.UI.Text>().text = "Pm: " + _playerCharacter.PmMax;
        GameObject.Find("RacialSkill").GetComponent<UnityEngine.UI.Text>().text = "SKill: " + _playerCharacter.Skills != null
            && _playerCharacter.Skills.Count >= 1 ? _playerCharacter.Skills[0].Name : "None";
        GameObject.Find("Skill").GetComponent<UnityEngine.UI.Text>().text = "SKill: " + _playerCharacter.Skills != null
            && _playerCharacter.Skills.Count >= 2 ? _playerCharacter.Skills[1].Name : "None";
    }

    public void GoToSwipeScene()
    {
        if (_playerCharacter == null)
            return;
        var journey = new Journey(_playerCharacter);
        PlayerPrefsHelper.SaveJourney(journey);
        PlayerPrefsHelper.SaveCharacter(Constants.PpPlayer, _playerCharacter);
        SceneManager.LoadScene(Constants.SwipeScene);
    }
}
