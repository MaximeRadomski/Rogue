using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    private GameObject _canvas;

    void Start()
    {
        SetPrivates();
    }

    public void SetPrivates()
    {
        _canvas = GameObject.Find("Canvas");
    }

    public void PopText(string text, Vector2 position, TextType type)
    {
        var tmpPoppingTextObject = Resources.Load<GameObject>("Prefabs/PoppingText");

        var tmpTexts = GameObject.FindGameObjectsWithTag(Constants.TagPoppingText);
        var nbTextsOnThisPosition = 0;
        foreach (var tmpText in tmpTexts)
        {
            if (Helper.FloatEqualsPrecision(tmpText.transform.position.x, position.x, 0.1f) &&
                Helper.FloatEqualsPrecision(tmpText.transform.position.y, position.y, 1f))
                ++nbTextsOnThisPosition;
        }

        var tmpPoppingTextInstance = Instantiate(tmpPoppingTextObject, position, tmpPoppingTextObject.transform.rotation, _canvas.transform);
        tmpPoppingTextInstance.GetComponent<PoppingTextBhv>().SetPrivates(text, position + new Vector2(0.0f, -0.2f * nbTextsOnThisPosition), type);
    }

    public void NewCard(int id)
    {
        var tmpCardObject = Resources.Load<GameObject>("Prefabs/Card");
        var tmpCardInstance = Instantiate(tmpCardObject, tmpCardObject.transform.position, tmpCardObject.transform.rotation, _canvas.transform);
        tmpCardInstance.GetComponent<GrabbableCardBhv>().SetPrivates(id);
    }

    public static GameObject NewCharacterGameObject(string characterName, bool isPlayer = false, string id = "")
    {
        var tmpCharacter = PlayerPrefsHelper.GetCharacter(characterName);
        var characterObject = Resources.Load<GameObject>("Prefabs/" + CharacterRace.Human + "Character");
        var characterInstance = Instantiate(characterObject, new Vector2(-3.0f, -5.0f), characterObject.transform.rotation);
        for (int i = 0; i < tmpCharacter.BodyParts.Count; ++i)
        {
            var tmpBodyPart = characterInstance.transform.Find(RacesData.BodyParts[i]);
            if (tmpBodyPart != null)
                tmpBodyPart.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tmpCharacter.BodyParts[i]);
        }
        if (isPlayer)
            characterInstance.name = Constants.GoPlayerName;
        else
        {
            characterInstance.name = Constants.GoOpponentName + id;
            characterInstance.AddComponent<AiBhv>();
        }
        var playerBhv = characterInstance.GetComponent<CharacterBhv>();
        playerBhv.X = 0;
        playerBhv.Y = 0;
        playerBhv.Character = tmpCharacter;
        playerBhv.IsPlayer = isPlayer;
        return characterInstance;
    }
}
