using System.Collections;
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
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonPause").GetComponent<ButtonBhv>().EndActionDelegate = GoToRaceChoiceScene;
        _instantiator.NewCard(1);
        _instantiator.NewCard(0);
        GameObject.Find("ButtonAvoid").GetComponent<ButtonBhv>().EndActionDelegate = GameObject.Find("Card1").GetComponent<GrabbableCardBhv>().Avoid;
        GameObject.Find("ButtonVenture").GetComponent<ButtonBhv>().EndActionDelegate = GameObject.Find("Card1").GetComponent<GrabbableCardBhv>().Venture;
    }

    public void NewCard()
    {
        _journey.Minutes += 20;
        if (_journey.Minutes >= 60)
        {
            _journey.Minutes -= 60;
            ++_journey.Hour;
        }
        if (_journey.Hour >= 24)
        {
            _journey.Hour -= 24;
        }
        ++_journey.Step;
        Destroy(GameObject.Find("Card1"));
        var backCard = GameObject.Find("Card0");
        backCard.GetComponent<GrabbableCardBhv>().BringToFront();
        GameObject.Find("ButtonAvoid").GetComponent<ButtonBhv>().EndActionDelegate = backCard.GetComponent<GrabbableCardBhv>().Avoid;
        GameObject.Find("ButtonVenture").GetComponent<ButtonBhv>().EndActionDelegate = backCard.GetComponent<GrabbableCardBhv>().Venture;
        _instantiator.NewCard(0);
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
        _xp.text = _playerCharacter.Experience.ToString();
        _gold.text = _playerCharacter.Gold.ToString();
        _matchPercentage.text = "100%";
        float englishHour = _journey.Hour > 12 ? _journey.Hour - 2 : _journey.Hour;
        float minutes = _journey.Minutes / 60.0f;
        var newRotation = 30.0f * (englishHour + minutes);
        _hoursCircle.transform.eulerAngles = new Vector3(0.0f, 0.0f, newRotation);
        for (int i = 0; i < _hoursCircle.transform.childCount; ++i)
        {
            var child = _hoursCircle.transform.GetChild(i);
            if (child == null)
                continue;
            if (child.transform.position.y > _hoursCircle.transform.position.y + 0.2f)
            {
                child.gameObject.SetActive(true);
                var tmpColor = child.GetComponent<TMPro.TextMeshPro>().color;
                var transparency = 1.0f;
                if (int.Parse(child.GetComponent<TMPro.TextMeshPro>().text) < englishHour)
                    transparency = 0.5f;
                else if (int.Parse(child.GetComponent<TMPro.TextMeshPro>().text) > englishHour)
                    transparency = 0.8f;
                tmpColor = new Color(tmpColor.r, tmpColor.g, tmpColor.b, transparency);
            }
            else
                child.gameObject.SetActive(false);
        }
        _biomePicture.sprite = Resources.Load<Sprite>("Sprites/" + _journey.Biome + "/BiomePicture");
        _amPm.text = _journey.Hour > 12 ? "PM" : "AM";
        _day.text = _journey.Day.ToString();
        _dayNight.sprite = _journey.Hour >= 8 && _journey.Hour < 4 ? DayNight[1] : DayNight[0];
        _biomeSteps.text = _journey.Step + "-" + _journey.MaxStep;
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
