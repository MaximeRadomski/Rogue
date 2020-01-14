using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeSceneBhv : SceneBhv
{
    public Sprite[] DayNight;

    private Character _playerCharacter;
    private Instantiator _instantiator;
    private Journey _journey;

    private GameObject _characterSkinContainer;
    private TMPro.TextMeshPro _hp;
    private TMPro.TextMeshPro _characterName;
    private TMPro.TextMeshPro _level;
    private TMPro.TextMeshPro _xp;
    private TMPro.TextMeshPro _gold;
    private TMPro.TextMeshPro _matchPercentage;

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
        _journey = PlayerPrefsHelper.GetJourney();
        _playerCharacter = JsonUtility.FromJson<Character>(PlayerPrefs.GetString(Constants.PpPlayer, Constants.PpSerializeDefault));
        _instantiator = GetComponent<Instantiator>();
        _instantiator.SetPrivates();
        _currentBiomeChoice = 0;
        _avoidBhv = GameObject.Find("ButtonAvoid").GetComponent<ButtonBhv>();
        _ventureBhv = GameObject.Find("ButtonVenture").GetComponent<ButtonBhv>();
        PauseMenu = _instantiator.NewPauseMenu();
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonPause").GetComponent<ButtonBhv>().EndActionDelegate = Pause;
        GameObject.Find("CharacterButton").GetComponent<ButtonBhv>().EndActionDelegate = ShowCharacterStats;
        _instantiator.NewRandomCard(1, _journey.Day, _journey.Biome.MapType);
        _instantiator.NewRandomCard(0, _journey.Day, _journey.Biome.MapType);
        _avoidBhv.EndActionDelegate = GameObject.Find("Card1").GetComponent<CardBhv>().Avoid;
        _ventureBhv.EndActionDelegate = GameObject.Find("Card1").GetComponent<CardBhv>().Venture;
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

    public void NewCard()
    {
        var minutesPassed = Random.Range(20, 61);
        _journey.Minutes += minutesPassed;
        if (_journey.Minutes >= 60)
        {
            _journey.Minutes -= 60;
            ++_journey.Hour;
        }
        if (_journey.Hour >= 24)
        {
            _journey.Hour -= 24;
        }
        //Debug.Log("Minutes Passed = " + minutesPassed + "\t|\t\tHours = " + _journey.Hour + "h" + _journey.Minutes);
        ++_journey.Step;
        Destroy(GameObject.Find("Card1"));
        var backCard = GameObject.Find("Card0");
        backCard.GetComponent<CardBhv>().BringToFront();
        _avoidBhv.EndActionDelegate = backCard.GetComponent<CardBhv>().Avoid;
        _ventureBhv.EndActionDelegate = backCard.GetComponent<CardBhv>().Venture;
        if (_journey.Step < _journey.Biome.Steps) //Just '<' because it instantiates one in advance
        {
            _instantiator.NewRandomCard(0, _journey.Day, _journey.Biome.MapType);
        }            
        else if (_currentBiomeChoice < _journey.Biome.Destinations)
        {
            ++_currentBiomeChoice;
            _instantiator.NewBiomeCard(0, _journey.Day, _currentBiomeChoice, _journey.Biome.Destinations);
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
        _hp = GameObject.Find("HpText").GetComponent<TMPro.TextMeshPro>();
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
        _hp.text = _playerCharacter.Hp.ToString();
        _level.text = _playerCharacter.Level.ToString();
        _xp.text = _playerCharacter.Experience.ToString() + "/" + Helper.XpNeedForLevel(_playerCharacter.Level) + " ®";
        _gold.text = _playerCharacter.Gold.ToString() + " ©";
        _matchPercentage.text = "100%";
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

    public void NewBiome(Biome biome)
    {
        var remainingCards = GameObject.FindGameObjectsWithTag(Constants.TagGrabbableCard);
        foreach (var card in remainingCards)
        {
            Destroy(card);
        }
        _avoidBhv.EnableButton();
        _currentBiomeChoice = 0;
        _journey.Step = 1;
        _journey.Biome = biome;
        _instantiator.NewRandomCard(1, _journey.Day, _journey.Biome.MapType);
        var backCard = GameObject.Find("Card1");
        //backCard.GetComponent<CardBhv>().BringToFront();
        _avoidBhv.EndActionDelegate = backCard.GetComponent<CardBhv>().Avoid;
        _ventureBhv.EndActionDelegate = backCard.GetComponent<CardBhv>().Venture;
        _instantiator.NewRandomCard(0, _journey.Day, _journey.Biome.MapType);
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
        _instantiator.NewPopupCharacterStats(_playerCharacter);
    }

    #region PauseMenu

    private void GiveUp()
    {
        NavigationService.LoadPreviousScene();
    }

    private void Settings()
    {

    }
    private void Exit()
    {
        NavigationService.LoadPreviousScene();
    }

    #endregion
}
