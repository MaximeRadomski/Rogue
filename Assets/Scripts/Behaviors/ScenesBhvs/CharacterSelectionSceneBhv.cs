using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionSceneBhv : SceneBhv
{
    private int _nbCharChoice;
    private List<Character> _choices;
    private Character _playerChoice;
    private SkinContainerBhv _skinContainerBhv;
    private GameObject _choiceSelector;
    private GameObject _characterFrame;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        SetPrivates();
        SetButtons();
    }

    protected override void SetPrivates()
    {
        base.SetPrivates();
        CanGoPreviousScene = false;
        Soul = PlayerPrefsHelper.GetSoul();
        _nbCharChoice = Soul.GetStatCurrentValue(Soul.SoulStats[Soul.NbCharChoice_Id]);
        _choices = new List<Character>();
        var maxStartingLevel = Soul.GetStatCurrentValue(Soul.SoulStats[Soul.StartingLevel_Id]);
        var minStartingLevel = maxStartingLevel - 2 > 1 ? maxStartingLevel - 2 : 1;
        for (int i = 0; i < _nbCharChoice; ++i)
        {
            var tmpChoice = RacesData.GetCharacterFromRaceAndLevel((CharacterRace)Random.Range(0, Helper.EnumCount<CharacterRace>()),
                                                                Random.Range(minStartingLevel, maxStartingLevel + 1), isPlayer: true);
            tmpChoice.RunAwayPercent += Soul.GetStatCurrentValue(Soul.SoulStats[Soul.RunAwayPercent_Id]);
            tmpChoice.LootPercent += Soul.GetStatCurrentValue(Soul.SoulStats[Soul.LootPercent_Id]);
            tmpChoice.CritChancePercent += Soul.GetStatCurrentValue(Soul.SoulStats[Soul.CritChance_Id]);
            tmpChoice.InventoryPlace += Soul.GetStatCurrentValue(Soul.SoulStats[Soul.InvPlace_Id]);
            tmpChoice.InventoryPlace = tmpChoice.InventoryPlace > 6 ? 6 : tmpChoice.InventoryPlace;
            tmpChoice.WeightLimit += Soul.GetStatCurrentValue(Soul.SoulStats[Soul.InvWeight_Id]);
            tmpChoice.Gold += Soul.GetStatCurrentValue(Soul.SoulStats[Soul.Gold_Id]);
            tmpChoice.HpMax += Soul.GetStatCurrentValue(Soul.SoulStats[Soul.Health_Id]);
            tmpChoice.Hp = Helper.RandomIntMultipleOf(tmpChoice.HpMax / 2, tmpChoice.HpMax, 10);
            tmpChoice.PmMax += Soul.GetStatCurrentValue(Soul.SoulStats[Soul.Pm_Id]);
            tmpChoice.PaMax += Soul.GetStatCurrentValue(Soul.SoulStats[Soul.Pa_Id]);
            _choices.Add(tmpChoice);
        }
        Soul.XpKept = (int)(Soul.Xp * Helper.MultiplierFromPercent(0, Soul.GetStatCurrentValue(Soul.SoulStats[Soul.XpKeptPercent_Id])));
        Soul.Xp = 0;
        _skinContainerBhv = GameObject.Find("SkinContainer").GetComponent<SkinContainerBhv>();
        _choiceSelector = GameObject.Find("ChoiceSelector");
        _characterFrame = GameObject.Find("CharacterFrame");
        _playerChoice = _choices[0];
    }

    private void SetButtons()
    {
        for (int i = 0; i < Soul.NbCharChoice_Max + Soul.NbCharChoice; ++i)
        {
            var currentChoice = GameObject.Find("Choice" + (i + 1));
            if (i < _nbCharChoice)
            {
                Instantiator.LoadCharacterSkin(_choices[i], currentChoice);
                currentChoice.GetComponent<ButtonBhv>().EndActionDelegate = ChangeChoice;
            }
            else
            {
                currentChoice.SetActive(false);
            }
        }
        DisplayCharacterStats();
        GameObject.Find("ButtonStart").GetComponent<ButtonBhv>().EndActionDelegate = GoToSwipeScene;
        UpdateButtons();
    }

    private void UpdateButtons()
    {

    }

    private void DisplayCharacterStats()
    {
        Instantiator.LoadCharacterSkin(_playerChoice, _skinContainerBhv.gameObject);
        _characterFrame.transform.Find("Level").GetComponent<TMPro.TextMeshPro>().text = _playerChoice.Level.ToString();
        _characterFrame.transform.Find("Name").GetComponent<TMPro.TextMeshPro>().text = _playerChoice.Name;
        _characterFrame.transform.Find("Race").GetComponent<TMPro.TextMeshPro>().text = _playerChoice.Race.ToString();
        _characterFrame.transform.Find("Gender").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsGender_" + _playerChoice.Gender.GetHashCode());
        _characterFrame.transform.Find("Hp").GetComponent<TMPro.TextMeshPro>().text = _playerChoice.Hp.ToString();
        _characterFrame.transform.Find("HpMax").GetComponent<TMPro.TextMeshPro>().text = _playerChoice.HpMax.ToString();
        _characterFrame.transform.Find("Pa").GetComponent<TMPro.TextMeshPro>().text = _playerChoice.PaMax.ToString();
        _characterFrame.transform.Find("Pm").GetComponent<TMPro.TextMeshPro>().text = _playerChoice.PmMax.ToString();
    }

    private void ChangeChoice()
    {
        var id = int.Parse(Constants.LastEndActionClickedName[Helper.CharacterAfterString(Constants.LastEndActionClickedName, "Choice")].ToString());
        var clickedChoice = GameObject.Find(Constants.LastEndActionClickedName);
        _playerChoice = _choices[id - 1];
        DisplayCharacterStats();
        _choiceSelector.transform.position = clickedChoice.transform.position + new Vector3(0.0f, 0.3f, 0.0f);
    }

    private void GoToSwipeScene()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "YOUR JOURNEY BEGINS", 2.0f, OnToSwipeScene);
    }

    public object OnToSwipeScene(bool result)
    {
        Journey = new Journey(_playerChoice);
        PlayerPrefsHelper.SaveJourney(Journey);
        PlayerPrefsHelper.SaveCharacter(Constants.PpPlayer, _playerChoice);
        NavigationService.LoadNextScene(Constants.SwipeScene);
        return result;
    }
}
