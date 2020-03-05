using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeSceneBhv : SceneBhv
{
    public Sprite[] DayNight;

    private Character _playerCharacter;

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
        PauseMenu = Instantiator.NewPauseMenu();
        OnRootPreviousScene = Constants.RaceChoiceScene;
        _playerCharacter = PlayerPrefsHelper.GetCharacter(Constants.PpPlayer);
        Instantiator.SetPrivates();
        _currentBiomeChoice = 0;
        _avoidBhv = GameObject.Find("ButtonAvoid").GetComponent<ButtonBhv>();
        _ventureBhv = GameObject.Find("ButtonVenture").GetComponent<ButtonBhv>();        
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonPause").GetComponent<ButtonBhv>().EndActionDelegate = Pause;
        GameObject.Find("CharacterButton").GetComponent<ButtonBhv>().EndActionDelegate = ShowCharacterStats;
        GameObject.Find("ButtonInventory").GetComponent<ButtonBhv>().EndActionDelegate = ShowInventory;
        if (Journey.Step >= Journey.Biome.Steps) //Just '<' because it instantiates one in advance
        {
            ++_currentBiomeChoice;
            Instantiator.NewCardBiome(1, Journey.Day, Journey.Biome, _currentBiomeChoice, Journey.Biome.Destinations, _playerCharacter);
            if (Journey.Biome.Destinations > 1)
            {
                ++_currentBiomeChoice;
                Instantiator.NewCardBiome(0, Journey.Day, Journey.Biome, _currentBiomeChoice, Journey.Biome.Destinations, _playerCharacter);
            }
            else
            {
                _avoidBhv.DisableButton();
            }
        }
        else
        {
            Instantiator.NewRandomCard(1, Journey.Day, Journey.Biome, _playerCharacter);
            Instantiator.NewRandomCard(0, Journey.Day, Journey.Biome, _playerCharacter);
        }
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
        Journey.UpdateTime(minutesPassed);
        if (regenerate)
            _playerCharacter.RegenerationFromMinutes(minutesPassed);
        //Debug.Log("Minutes Passed = " + minutesPassed + "\t|\t\tHours = " + _journey.Hour + "h" + _journey.Minutes);
        ++Journey.Step;
        Destroy(GameObject.Find("Card1"));
        _currentCard = GameObject.Find("Card0");
        _currentCard.GetComponent<CardBhv>().BringToFront();
        Instantiator.PopText(Helper.TimeFromMinutes(minutesPassed), (Vector2)_currentCard.transform.position + new Vector2(0.0f, 1.6f), TextType.Normal);
        _avoidBhv.EndActionDelegate = _currentCard.GetComponent<CardBhv>().Avoid;
        _ventureBhv.EndActionDelegate = _currentCard.GetComponent<CardBhv>().Venture;
        if (Journey.Step < Journey.Biome.Steps) //Just '<' because it instantiates one in advance
        {
            Instantiator.NewRandomCard(0, Journey.Day, Journey.Biome, _playerCharacter);
        }            
        else if (_currentBiomeChoice < Journey.Biome.Destinations)
        {
            ++_currentBiomeChoice;
            Instantiator.NewCardBiome(0, Journey.Day, Journey.Biome, _currentBiomeChoice, Journey.Biome.Destinations, _playerCharacter);
        }
        else
        {
            _avoidBhv.DisableButton();
        }
        UpdateDisplayJourneyAndCharacterStats();
        PlayerPrefsHelper.SaveJourney(Journey);
        PlayerPrefsHelper.SaveCharacter(Constants.PpPlayer, _playerCharacter);
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
        _orbLife.UpdateContent(_playerCharacter.Hp, _playerCharacter.HpMax, Instantiator, TextType.Hp);
        _level.text = _playerCharacter.Level.ToString();
        _xp.text = _playerCharacter.Experience.ToString() + "/" + Helper.XpNeedForLevel(_playerCharacter.Level) + " " + Constants.UnitXp;
        _gold.text = _playerCharacter.Gold.ToString() + " " + Constants.UnitGold;
        _matchPercentage.text = _currentCard.GetComponent<CardBhv>().PositiveOutcomePercent + "%";
        float englishHour = Journey.Hour > 12 ? Journey.Hour - 12 : Journey.Hour;
        if (Journey.Hour == 24 || Journey.Hour == 12) englishHour = 0;
        float minutes = Journey.Minutes / 60.0f;
        var newRotation = 30.0f * (englishHour + minutes);
        _hoursCircle.GetComponent<HoursCircleBhv>().Rotate(new Vector3(0.0f, 0.0f, newRotation));
        _biomePicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/BiomePicture_" + Journey.Biome.MapType.GetHashCode());
        _amPm.text = Journey.Hour > 12 ? "PM" : "AM";
        _day.text = Journey.Day.ToString();
        _dayNight.sprite = Journey.Hour >= 20 || Journey.Hour < 4 ? DayNight[1] : DayNight[0];
        _biomeSteps.text = (Journey.Step <= Journey.Biome.Steps ? Journey.Step : Journey.Biome.Steps) + "-" + Journey.Biome.Steps;
    }

    public void NewBiome(Biome biome, int minutesPassed)
    {
        if (_playerCharacter.GetTotalWeight() > _playerCharacter.WeightLimit)
            minutesPassed *= 2;
        Journey.UpdateTime(minutesPassed);
        _playerCharacter.RegenerationFromMinutes(minutesPassed);
        var remainingCards = GameObject.FindGameObjectsWithTag(Constants.TagGrabbableCard);
        foreach (var card in remainingCards)
        {
            Destroy(card);
        }
        _avoidBhv.EnableButton();
        _currentBiomeChoice = 0;
        Journey.Step = 1;
        Journey.Biome = biome;
        Instantiator.NewRandomCard(1, Journey.Day, Journey.Biome, _playerCharacter);
        _currentCard = GameObject.Find("Card1");
        Instantiator.PopText(Helper.TimeFromMinutes(minutesPassed), (Vector2)_currentCard.transform.position + new Vector2(0.0f, 1.6f), TextType.Normal);
        //backCard.GetComponent<CardBhv>().BringToFront();
        _avoidBhv.EndActionDelegate = _currentCard.GetComponent<CardBhv>().Avoid;
        _ventureBhv.EndActionDelegate = _currentCard.GetComponent<CardBhv>().Venture;
        Instantiator.NewRandomCard(0, Journey.Day, Journey.Biome, _playerCharacter);
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
        Journey.Step++;
        PlayerPrefsHelper.SaveJourney(Journey);
        PlayerPrefsHelper.SaveCharacter(Constants.PpPlayer, _playerCharacter);
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "ENTERING COMBAT", 2.0f, TransitionFight);
        object TransitionFight(bool transResult)
        {
            NavigationService.LoadNextScene(Constants.FightScene);
            return transResult;
        }
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
}
