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
        var tmpPoppingTextInstance = Instantiate(tmpPoppingTextObject, position, tmpPoppingTextObject.transform.rotation, _canvas.transform);
        tmpPoppingTextInstance.GetComponent<PoppingTextBhv>().SetPrivates(text, position, type);
    }

    public void NewCard(int id)
    {
        var tmpCardObject = Resources.Load<GameObject>("Prefabs/Card");
        var tmpCardInstance = Instantiate(tmpCardObject, tmpCardObject.transform.position, tmpCardObject.transform.rotation, _canvas.transform);
        tmpCardInstance.GetComponent<GrabbableCardBhv>().SetPrivates(id);
    }

    public static GameObject NewCharacterGameObject(string characterName, bool isPlayer = false)
    {
        var characterObject = Resources.Load<GameObject>("Prefabs/TemplateCharacter");
        var characterInstance = Instantiate(characterObject, new Vector2(-3.0f, -5.0f), characterObject.transform.rotation);
        if (isPlayer)
            characterInstance.name = Constants.GoPlayerName;
        else
            characterInstance.name = Constants.GoOpponentName;
        var playerBhv = characterInstance.GetComponent<CharacterBhv>();
        playerBhv.X = 0;
        playerBhv.Y = 0;
        playerBhv.Character = PlayerPrefsHelper.GetCharacter(characterName);
        playerBhv.IsPlayer = isPlayer;
        return characterInstance;
    }
}
