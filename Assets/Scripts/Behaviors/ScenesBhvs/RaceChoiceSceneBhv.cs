using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceChoiceSceneBhv : SceneBhv
{
    private TMPro.TextMeshPro _characterName;
    private Character _playerCharacter;
    private GameObject _skinContainer;
    private Instantiator _instantiator;
    private TMPro.TextMeshPro _raceTextMesh;
    private int _skinColor;
    private CharacterGender _gender;
    private CharacterRace _race;
    private List<ButtonBhv> _rightButtons;
    private List<ButtonBhv> _leftButtons;

    void Start()
    {
        SetPrivates();
        SetButtons();
    }

    protected override void SetPrivates()
    {
        base.SetPrivates();
        _characterName = GameObject.Find("CharacterName").GetComponent<TMPro.TextMeshPro>();
        _skinContainer = GameObject.Find("SkinContainer");
        _instantiator = GetComponent<Instantiator>();
        _raceTextMesh = GameObject.Find("RaceText").GetComponent<TMPro.TextMeshPro>();
        _gender = (CharacterGender)Random.Range(0, 1);
        _race = CharacterRace.Human;
        _rightButtons = new List<ButtonBhv>();
        _leftButtons = new List<ButtonBhv>();
        var allButtons = GameObject.FindGameObjectsWithTag(Constants.TagButton);
        foreach (var button in allButtons)
        {
            if (button.name.Contains("Right"))
                _rightButtons.Add(button.GetComponent<ButtonBhv>());
            if (button.name.Contains("Left"))
                _leftButtons.Add(button.GetComponent<ButtonBhv>());
        }
    }

    private void SetButtons()
    {
        _characterName.gameObject.GetComponent<ButtonBhv>().EndActionDelegate = _instantiator.EditViaKeyboard;
        GameObject.Find("ButtonStart").GetComponent<ButtonBhv>().EndActionDelegate = GoToSwipeScene;
        GameObject.Find("ButtonRandomAll").GetComponent<ButtonBhv>().EndActionDelegate = RandomizeAll;
        GameObject.Find("ButtonRandomSkin").GetComponent<ButtonBhv>().EndActionDelegate = RandomizeSkin;
        foreach (var button in _rightButtons)
            button.EndActionDelegate = LeftRight;
        foreach (var button in _leftButtons)
            button.EndActionDelegate = LeftRight;
        UpdateButtons();
    }

    private void RandomizeAll()
    {
        foreach (var button in _leftButtons)
        {
            if (!IsTopButtons(button.name))
                continue;
            int maxRand = RacesData.NbSkinTemplates;
            if (button.gameObject.name.Contains("Gender"))
                maxRand = 2;
            else if (button.gameObject.name.Contains("Race"))
                maxRand = Helper.EnumCount<CharacterRace>();

            var max = Random.Range(0, maxRand);
            for (int i = 0; i < max; ++i)
            {
                Constants.LastEndActionClickedName = button.name;
                button.EndActionDelegate.Invoke();
            }
        }
        _characterName.text = RacesData.GetRandomNameFromRaceAndGender(_race, _gender);
        RandomizeSkin();
    }

    private void RandomizeSkin()
    {
        foreach (var button in _leftButtons)
        {
            if (IsTopButtons(button.name))
                continue;
            int maxRand = RacesData.NbBodyTemplates;
            if (button.gameObject.name.Contains("Hair"))
            {
                maxRand = RacesData.NbHairTemplates;
            }

            var max = Random.Range(0, maxRand);
            for (int i = 0; i < max; ++i)
            {
                Constants.LastEndActionClickedName = button.name;
                button.EndActionDelegate.Invoke();
            }
        }
    }

    private void UpdateButtons()
    {
        foreach (var button in _rightButtons)
        {
            Constants.LastEndActionClickedName = button.transform.parent.name;
            button.EndActionDelegate.Invoke();
        }
    }

    private bool IsTopButtons(string name)
    {
        return name.Contains("SkinColor")
            || name.Contains("Gender")
            || name.Contains("Race");
    }

    private void LeftRight()
    {
        GameObject parent;
        if (Constants.LastEndActionClickedName.Contains("Selector"))
            parent = GameObject.Find(Constants.LastEndActionClickedName);
        else
            parent = GameObject.Find(Constants.LastEndActionClickedName).transform.parent.gameObject;
        var bodyPart = parent.name.Substring(Helper.CharacterAfterString(parent.name, "Selector"));
        var spriteRenderer = parent.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        // GENDER //
        if (bodyPart.Contains("Gender"))
        {
            if (!Constants.LastEndActionClickedName.Contains("Selector"))
                _gender = _gender == CharacterGender.Male ? CharacterGender.Female : CharacterGender.Male;
            spriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/IconsGender_" + _gender.GetHashCode());
            if (!Constants.LastEndActionClickedName.Contains("Selector"))
                UpdateButtons();
            return;
        }
        // RACE //
        if (bodyPart.Contains("Race"))
        {
            var raceId = _race.GetHashCode();
            if (Constants.LastEndActionClickedName.Contains("Right"))
                ++raceId;
            else if (Constants.LastEndActionClickedName.Contains("Left"))
                --raceId;
            if (raceId < 0) raceId = Helper.EnumCount<CharacterRace>() - 1;
            else if (raceId >= Helper.EnumCount<CharacterRace>()) raceId = 0;
            _race = (CharacterRace)raceId;
            _raceTextMesh.text = _race.ToString();
            if (!Constants.LastEndActionClickedName.Contains("Selector"))
                UpdateButtons();
            return;
        }
        int id = 0;
        if (spriteRenderer.sprite != null)
            id = int.Parse(spriteRenderer.sprite.name.Substring(Helper.CharacterAfterString(spriteRenderer.sprite.name, "_")));
        if (Constants.LastEndActionClickedName.Contains("Right"))
            ++id;
        else if (Constants.LastEndActionClickedName.Contains("Left"))
            --id;
        var customGender = "";
        if (bodyPart.Contains("Skin"))
        {
            if (id < 0) id = RacesData.NbSkinTemplates - 1;
            else if (id >= RacesData.NbSkinTemplates) id = 0;
        }
        else if (bodyPart.Contains("Hair"))
        {
            if (id < 0) id = RacesData.NbHairTemplates - 1;
            else if (id >= RacesData.NbHairTemplates) id = 0;
            customGender += _gender;
        }
        else
        {
            if (id < 0) id = RacesData.NbBodyTemplates - 1;
            else if (id >= RacesData.NbBodyTemplates) id = 0;
        }
        var idMesh = GameObject.Find("Id" + bodyPart);
        if (idMesh != null)
            idMesh.GetComponent<TMPro.TextMeshPro>().text = (id + 1).ToString();
        var customPart = "";
        if (bodyPart.Contains("Arm") || bodyPart.Contains("Hand"))
            customPart = "Front";
        var tmpRace = "Human";
        var path = "Sprites/" + tmpRace + "/" + tmpRace + customGender + customPart + bodyPart + "_" + id;
        spriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet(path);
        // SKIN COLOR //
        if (bodyPart.Contains("Skin"))
        {
            for (int i = 0; i < _skinContainer.transform.childCount; ++i)
            {
                var child = _skinContainer.transform.GetChild(i);
                if (child.name.Contains("Naked"))
                {
                    var childSpriteRenderer = child.GetComponent<SpriteRenderer>();
                    var skinPath = "Sprites/" + tmpRace + "/" + childSpriteRenderer.sprite.name.Substring(0, childSpriteRenderer.sprite.name.Length - 1) + id;
                    childSpriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet(skinPath);
                }
            }
            return;
        }
        // BODY PART //
        if (customPart == "")
            _skinContainer.transform.Find(bodyPart).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet(path);
        else
        {
            _skinContainer.transform.Find(customPart + bodyPart).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet(path);
            customPart = "Back";
            _skinContainer.transform.Find(customPart + bodyPart).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/" + tmpRace + "/" + tmpRace + customGender + customPart + bodyPart + "_" + id);
        }
    }

    public void GoToSwipeScene()
    {
        _instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "YOUR JOURNEY BEGINS", 2.0f, OnToSwipeScene);
    }

    public object OnToSwipeScene(bool result)
    {
        var tmpRace = "Human";
        _playerCharacter = RacesData.GetCharacterFromRaceAndLevel(_race, 1, true);
        if (_playerCharacter.Gender != CharacterGender.Transgender)
            _playerCharacter.Gender = _gender;
        _playerCharacter.Name = _characterName.text;
        for (int i = 0; i < _skinContainer.transform.childCount; ++i)
        {
            var child = _skinContainer.transform.GetChild(i);
            var childSpriteRenderer = child.GetComponent<SpriteRenderer>();
            for (int y = 0; y < _playerCharacter.BodyParts.Count; ++y)
            {
                if (RacesData.BodyParts[y].Contains(child.name))
                {
                    var skinPath = "Sprites/" + tmpRace + "/" + childSpriteRenderer.sprite.name;
                    _playerCharacter.BodyParts[y] = skinPath;
                }
            }
        }
        var journey = new Journey(_playerCharacter);
        PlayerPrefsHelper.SaveJourney(journey);
        PlayerPrefsHelper.SaveCharacter(Constants.PpPlayer, _playerCharacter);
        NavigationService.LoadNextScene(Constants.SwipeScene);
        return result;
    }
}
