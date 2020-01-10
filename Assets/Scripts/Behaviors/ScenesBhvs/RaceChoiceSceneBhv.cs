using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceChoiceSceneBhv : SceneBhv
{
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

    internal override void SetPrivates()
    {
        base.SetPrivates();
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
        GameObject.Find("CharacterName").GetComponent<ButtonBhv>().EndActionDelegate = _instantiator.EditViaKeyboard;
        GameObject.Find("ButtonStart").GetComponent<ButtonBhv>().EndActionDelegate = GoToSwipeScene;
        foreach (var button in _rightButtons)
            button.EndActionDelegate = LeftRight;
        foreach (var button in _leftButtons)
            button.EndActionDelegate = LeftRight;
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        foreach (var button in _rightButtons)
        {
            Constants.LastEndActionClickedName = button.transform.parent.name;
            button.EndActionDelegate.Invoke();
        }
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
            id = int.Parse(spriteRenderer.sprite.name[spriteRenderer.sprite.name.Length - 1].ToString());
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
        _playerCharacter = RacesData.GetCharacterFromRaceAndLevel(CharacterRace.Human, 1, true);
        if (_playerCharacter == null)
            return;
        var journey = new Journey(_playerCharacter);
        PlayerPrefsHelper.SaveJourney(journey);
        PlayerPrefsHelper.SaveCharacter(Constants.PpPlayer, _playerCharacter);
        SceneManager.LoadScene(Constants.SwipeScene);
    }
}
