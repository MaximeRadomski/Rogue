using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeSceneBhv : SceneBhv
{
    public Sprite[] DayNight;

    private Character _playerCharacter;
    private Journey _journey;

    private GameObject _characterSkinContainer;
    private OrbBhv _orbLife;
    private TMPro.TextMeshPro _characterName;
    private TMPro.TextMeshPro _level;
    private TMPro.TextMeshPro _xp;
    private TMPro.TextMeshPro _gold;
    private TMPro.TextMeshPro _matchPercentage;

    private GameObject _currentCard;
    private GameObject _hoursCircle;
    private SpriteRenderer _biomePicture;
    private SpriteRenderer _dayNight;
    private TMPro.TextMeshPro _amPm;
    private TMPro.TextMeshPro _day;
    private TMPro.TextMeshPro _biomeSteps;

    private int _currentBiomeChoice;

    private ButtonBhv _avoidBhv;
    private ButtonBhv _ventureBhv;

    void Start()
    {
        SetPrivates();
        SetButtons();
        FirstDisplayJourneyAndCharacterStats();
    }

    protected override void SetPrivates()
    {
        base.SetPrivates();
        OnRootPreviousScene = Constants.RaceChoiceScene;
        _journey = PlayerPrefsHelper.GetJourney();
        _playerCharacter = PlayerPrefsHelper.GetCharacter(Constants.PpPlayer);
        Instantiator = GetComponent<Instantiator>();
        Instantiator.SetPrivates();
        _currentBiomeChoice = 0;
        _avoidBhv = GameObject.Find("ButtonAvoid").GetComponent<ButtonBhv>();
        _ventureBhv = GameObject.Find("ButtonVenture").GetComponent<ButtonBhv>();
        PauseMenu = Instantiator.NewPauseMenu();
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonPause").GetComponent<ButtonBhv>().EndActionDelegate = Pause;
        GameObject.Find("CharacterButton").GetComponent<ButtonBhv>().EndActionDelegate = ShowCharacterStats;
        GameObject.Find("ButtonInventory").GetComponent<ButtonBhv>().EndActionDelegate = ShowInventory;
        Instantiator.NewRandomCard(1, _journey.Day, _journey.Biome.MapType, _playerCharacter);
        Instantiator.NewRandomCard(0, _journey.Day, _journey.Biome.MapType, _playerCharacter);
        _currentCard = GameObject.Find("Card1");
        _avoidBhv.EndActionDelegate = _currentCard.GetComponent<CardBhv>().Avoid;
        _ventureBhv.EndActionDelegate = _currentCard.GetComponent<CardBhv>().Venture;
        PauseMenu.Buttons[0].EndActionDelegate = Resume;
        PauseMenu.TextMeshes[0].text = "Resume";
        PauseMenu.Buttons[1].EndActionDelegate = GiveUp;
        PauseMenu.TextMeshes[1].text = "Give Up";
        PauseMenu.Buttons[2].EndActionDelegate = Settings;
        PauseMenu.TextMeshes[2].text = "Settings";
        PauseMenu.Buttons[3].EndActionDelegate = Exit;
        PauseMenu.TextMeshes[3].text = "Exit";
        PauseMenu.Buttons[4].gameObject.SetActive(false);
    }

    public void NewCard(int minutesPassed, bool regenerate = true)
    {
        if (_playerCharacter.GetTotalWeight() > _playerCharacter.WeightLimit)
            minutesPassed *= 2;
        _journey.UpdateTime(minutesPassed);
        if (regenerate)
            _playerCharacter.RegenerationFromMinutes(minutesPassed);
        //Debug.Log("Minutes Passed = " + minutesPassed + "\t|\t\tHours = " + _journey.Hour + "h" + _journey.Minutes);
        ++_journey.Step;
        Destroy(GameObject.Find("Card1"));
        _currentCard = GameObject.Find("Card0");
        _currentCard.GetComponent<CardBhv>().BringToFront();
        Instantiator.PopText(Helper.TimeFromMinutes(minutesPassed), (Vector2)_currentCard.transform.position + new Vector2(0.0f, 1f), TextType.Normal);
        _avoidBhv.EndActionDelegate = _currentCard.GetComponent<CardBhv>().Avoid;
        _ventureBhv.EndActionDelegate = _currentCard.GetComponent<CardBhv>().Venture;
        if (_journey.Step < _journey.Biome.Steps) //Just '<' because it instantiates one in advance
        {
            Instantiator.NewRandomCard(0, _journey.Day, _journey.Biome.MapType, _playerCharacter);
        }            
        else if (_currentBiomeChoice < _journey.Biome.Destinations)
        {
            ++_currentBiomeChoice;
            Instantiator.NewCardBiome(0, _journey.Day, _journey.Biome.MapType, _currentBiomeChoice, _journey.Biome.Destinations, _playerCharacter);
        }
        else
        {
            _avoidBhv.DisableButton();
        }
        UpdateDisplayJourneyAndCharacterStats();
    }

    private void FirstDisplayJourneyAndCharacterStats()
    {
        _characterSkinContainer = GameObject.Find("CharacterSkinContainer");
        _characterName = GameObject.Find("CharacterName").GetComponent<TMPro.TextMeshPro>();
        _orbLife = GameObject.Find("Hp").GetComponent<OrbBhv>();
        _level = GameObject.Find("LevelText").GetComponent<TMPro.TextMeshPro>();
        _xp = GameObject.Find("Xp").GetComponent<TMPro.TextMeshPro>();
        _gold = GameObject.Find("Gold").GetComponent<TMPro.TextMeshPro>();
        _matchPercentage = GameObject.Find("MatchPercentage").GetComponent<TMPro.TextMeshPro>();

        _hoursCircle = GameObject.Find("HoursCircle");
        _biomePicture = GameObject.Find("BiomePicture").GetComponent<SpriteRenderer>();
        _amPm = GameObject.Find("AmPm").GetComponent<TMPro.TextMeshPro>();
        _day = GameObject.Find("Day").GetComponent<TMPro.TextMeshPro>();
        _dayNight = GameObject.Find("DayNight").GetComponent<SpriteRenderer>();
        _biomeSteps = GameObject.Find("BiomeSteps").GetComponent<TMPro.TextMeshPro>();

        Instantiator.LoadCharacterSkin(_playerCharacter, _characterSkinContainer);
        _characterName.text = _playerCharacter.Name;
        UpdateDisplayJourneyAndCharacterStats();
    }

    private void UpdateDisplayJourneyAndCharacterStats()
    {
        _orbLife.UpdateContent(_playerCharacter.Hp, _playerCharacter.HpMax);
        _level.text = _playerCharacter.Level.ToString();
        _xp.text = _playerCharacter.Experience.ToString() + "/" + Helper.XpNeedForLevel(_playerCharacter.Level) + " ®";
        _gold.text = _playerCharacter.Gold.ToString() + " ©";
        _matchPercentage.text = _currentCard.GetComponent<CardBhv>().PositiveOutcomePercent + "%";
        float englishHour = _journey.Hour > 12 ? _journey.Hour - 12 : _journey.Hour;
        if (_journey.Hour == 24 || _journey.Hour == 12) englishHour = 0;
        float minutes = _journey.Minutes / 60.0f;
        var newRotation = 30.0f * (englishHour + minutes);
        _hoursCircle.GetComponent<HoursCircleBhv>().Rotate(new Vector3(0.0f, 0.0f, newRotation));
        _biomePicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/BiomePicture_" + _journey.Biome.MapType.GetHashCode());
        _amPm.text = _journey.Hour > 12 ? "PM" : "AM";
        _day.text = _journey.Day.ToString();
        _dayNight.sprite = _journey.Hour >= 20 || _journey.Hour < 4 ? DayNight[1] : DayNight[0];
        _biomeSteps.text = (_journey.Step <= _journey.Biome.Steps ? _journey.Step : _journey.Biome.Steps) + "-" + _journey.Biome.Steps;
    }

    public void NewBiome(Biome biome, int minutesPassed)
    {
        if (_playerCharacter.GetTotalWeight() > _playerCharacter.WeightLimit)
            minutesPassed *= 2;
        _journey.UpdateTime(minutesPassed);
        _playerCharacter.RegenerationFromMinutes(minutesPassed);
        var remainingCards = GameObject.FindGameObjectsWithTag(Constants.TagGrabbableCard);
        foreach (var card in remainingCards)
        {
            Destroy(card);
        }
        _avoidBhv.EnableButton();
        _currentBiomeChoice = 0;
        _journey.Step = 1;
        _journey.Biome = biome;
        Instantiator.NewRandomCard(1, _journey.Day, _journey.Biome.MapType, _playerCharacter);
        _currentCard = GameObject.Find("Card1");
        Instantiator.PopText(Helper.TimeFromMinutes(minutesPassed), (Vector2)_currentCard.transform.position + new Vector2(0.0f, 1f), TextType.Normal);
        //backCard.GetComponent<CardBhv>().BringToFront();
        _avoidBhv.EndActionDelegate = _currentCard.GetComponent<CardBhv>().Avoid;
        _ventureBhv.EndActionDelegate = _currentCard.GetComponent<CardBhv>().Venture;
        Instantiator.NewRandomCard(0, _journey.Day, _journey.Biome.MapType, _playerCharacter);
        UpdateDisplayJourneyAndCharacterStats();
    }

    public bool CanAvoid()
    {
        return !_avoidBhv.Disabled;
    }

    public bool CanVenture()
    {
        return !_ventureBhv.Disabled;
    }

    public void GoToFightScene(List<Character> opponentCharacters)
    {
        for (int i = 0; i < opponentCharacters.Count; ++i)
        {
            PlayerPrefsHelper.SaveCharacter(Constants.PpOpponent + i, opponentCharacters[i]);
        }
        PlayerPrefs.SetInt(Constants.PpNbOpponents, opponentCharacters.Count);
        NavigationService.LoadNextScene(Constants.FightScene);
    }

    private void ShowCharacterStats()
    {
        Instantiator.NewPopupCharacterStats(_playerCharacter, OnCallUpdate, isInventoryAvailable:true);
    }

    private void ShowInventory()
    {
        Instantiator.NewPopupInventory(_playerCharacter, OnCallUpdate);
    }

    private bool OnCallUpdate()
    {
        UpdateDisplayJourneyAndCharacterStats();
        return true;
    }

    #region PauseMenu

    private void GiveUp()
    {
        Instantiator.NewPopupYesNo(Constants.YesNoTitle,
            "You wont be able to recover your progress if you give up now!"
            , Constants.Cancel, Constants.Proceed, OnGiveUp);
    }

    private object OnGiveUp(bool result)
    {
        if (result)
        {
            NavigationService.LoadPreviousScene(OnRootPreviousScene);
        }
        return result;
    }

    private void Settings()
    {

    }
    private void Exit()
    {
        NavigationService.LoadPreviousScene(OnRootPreviousScene);
    }

    #endregion
}
