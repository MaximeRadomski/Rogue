using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    void Start()
    {
        SetPrivates();
    }

    public void SetPrivates()
    {
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

        var tmpPoppingTextInstance = Instantiate(tmpPoppingTextObject, position, tmpPoppingTextObject.transform.rotation);
        tmpPoppingTextInstance.GetComponent<PoppingTextBhv>().SetPrivates(text, position + new Vector2(0.0f, -0.2f * nbTextsOnThisPosition), type);
    }

    public void NewCard(int id)
    {
        var tmpCardObject = Resources.Load<GameObject>("Prefabs/Card");
        var tmpCardInstance = Instantiate(tmpCardObject, tmpCardObject.transform.position, tmpCardObject.transform.rotation);
        tmpCardInstance.GetComponent<GrabbableCardBhv>().SetPrivates(id);
    }

    public static GameObject NewCharacterGameObject(string characterName, bool isPlayer = false, string id = "")
    {
        var character = PlayerPrefsHelper.GetCharacter(characterName);
        var characterObject = Resources.Load<GameObject>("Prefabs/" + CharacterRace.Human + "Character");
        var characterInstance = Instantiate(characterObject, new Vector2(-3.0f, -5.0f), characterObject.transform.rotation);
        LoadCharacterSkin(character, characterInstance.transform.Find("SkinContainer").gameObject);
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
        playerBhv.Character = character;
        playerBhv.IsPlayer = isPlayer;
        return characterInstance;
    }

    public static void LoadCharacterSkin(Character character, GameObject skinContainer)
    {
        for (int i = 0; i < character.BodyParts.Count; ++i)
        {
            var tmpBodyPart = skinContainer.transform.Find(RacesData.BodyParts[i]);
            if (tmpBodyPart != null)
            {
                var path = character.BodyParts[i];
                var separatorId = path.IndexOf('_');
                var spriteSheetPath = path.Substring(0, separatorId);
                var spriteSheet = Resources.LoadAll<Sprite>(spriteSheetPath);
                var spriteId = int.Parse(path.Substring(separatorId + 1));
                if (spriteId >= spriteSheet.Length)
                    tmpBodyPart.GetComponent<SpriteRenderer>().sprite = null;
                else
                    tmpBodyPart.GetComponent<SpriteRenderer>().sprite = spriteSheet[spriteId];
            }
                
        }
    }

    public static GameObject NewCell(int x, int y, char c, Grid grid)
    {
        var cellGameObject = Resources.Load<GameObject>("Prefabs/TemplateCell");
        var cellInstance = Instantiate(cellGameObject, cellGameObject.transform.position, cellGameObject.transform.rotation);
        cellInstance.transform.parent = grid.transform;
        cellInstance.transform.position = new Vector3(x * grid.cellSize.x, y * grid.cellSize.y, 0.0f) + grid.transform.position;
        cellInstance.gameObject.name = "Cell" + x + y;
        var cellBhv = cellInstance.GetComponent<CellBhv>();
        cellBhv.X = x;
        cellBhv.Y = y;
        cellBhv.Type = (CellType)int.Parse(c.ToString(), System.Globalization.NumberStyles.Integer);
        if (cellBhv.Type == CellType.Spawn || cellBhv.Type == CellType.OpponentSpawn)
            cellBhv.State = CellState.Spawn;
        else
            cellBhv.State = CellState.None;
        cellInstance.GetComponent<SpriteRenderer>().sortingOrder = Constants.GridMax - y;
        return cellInstance;
    }
}
