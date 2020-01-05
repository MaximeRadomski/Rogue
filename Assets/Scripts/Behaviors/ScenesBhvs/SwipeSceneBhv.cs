﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeSceneBhv : MonoBehaviour
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
        Application.targetFrameRate = 60;
        SetPrivates();
        SetButtons();
        FirstDisplayJourneyAndCharacterStats();
    }

    private void SetPrivates()
    {
        _journey = PlayerPrefsHelper.GetJourney();
        _playerCharacter = JsonUtility.FromJson<Character>(PlayerPrefs.GetString(Constants.PpPlayer, Constants.PpSerializeDefault));
        _instantiator = GetComponent<Instantiator>();
        _instantiator.SetPrivates();
        _currentBiomeChoice = 0;
        _avoidBhv = GameObject.Find("ButtonAvoid").GetComponent<ButtonBhv>();
        _ventureBhv = GameObject.Find("ButtonVenture").GetComponent<ButtonBhv>();

    }

    private void SetButtons()
    {
        GameObject.Find("ButtonPause").GetComponent<ButtonBhv>().EndActionDelegate = GoToRaceChoiceScene;
        _instantiator.NewRandomCard(1, _journey.Day, _journey.Biome.MapType);
        _instantiator.NewRandomCard(0, _journey.Day, _journey.Biome.MapType);
        _avoidBhv.EndActionDelegate = GameObject.Find("Card1").GetComponent<CardBhv>().Avoid;
        _ventureBhv.EndActionDelegate = GameObject.Find("Card1").GetComponent<CardBhv>().Venture;
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
        _xp.text = _playerCharacter.Experience.ToString() + " ®";
        _gold.text = _playerCharacter.Gold.ToString() + " ©";
        _matchPercentage.text = "100%";
        float englishHour = _journey.Hour > 12 ? _journey.Hour - 12 : _journey.Hour;
        if (_journey.Hour == 24 || _journey.Hour == 12) englishHour = 0;
        float minutes = _journey.Minutes / 60.0f;
        var newRotation = 30.0f * (englishHour + minutes);
        _hoursCircle.GetComponent<HoursCircleBhv>().Rotate(new Vector3(0.0f, 0.0f, newRotation));
        _biomePicture.sprite = Resources.Load<Sprite>("Sprites/" + _journey.Biome.MapType + "/BiomePicture");
        _amPm.text = _journey.Hour > 12 ? "PM" : "AM";
        _day.text = _journey.Day.ToString();
        _dayNight.sprite = _journey.Hour >= 20 || _journey.Hour < 4 ? DayNight[1] : DayNight[0];
        _biomeSteps.text = (_journey.Step <= _journey.Biome.Steps ? _journey.Step : _journey.Biome.Steps) + "-" + _journey.Biome.Steps;
    }

    public void NewBiome(Biome biome)
    {
        _avoidBhv.EnableButton();
        _currentBiomeChoice = 0;
        _journey.Step = 1;
        _journey.Biome = biome;
        _instantiator.NewRandomCard(1, _journey.Day, _journey.Biome.MapType);
        _instantiator.NewRandomCard(0, _journey.Day, _journey.Biome.MapType);
        _avoidBhv.EndActionDelegate = GameObject.Find("Card1").GetComponent<CardBhv>().Avoid;
        _ventureBhv.EndActionDelegate = GameObject.Find("Card1").GetComponent<CardBhv>().Venture;
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

    public void GoToRaceChoiceScene()
    {
        SceneManager.LoadScene(Constants.RaceChoiceScene);
    }

    public void GoToFightScene(List<Character> opponentCharacters)
    {
        for (int i = 0; i < opponentCharacters.Count; ++i)
        {
            PlayerPrefsHelper.SaveCharacter(Constants.PpOpponent + i, opponentCharacters[i]);
        }
        PlayerPrefs.SetInt(Constants.PpNbOpponents, opponentCharacters.Count);
        SceneManager.LoadScene(Constants.FightScene);
    }
}
