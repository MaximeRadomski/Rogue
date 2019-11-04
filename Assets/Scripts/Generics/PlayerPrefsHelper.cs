using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsHelper : MonoBehaviour
{
    public static void SaveCharacter(string characterName, Character character)
    {
        PlayerPrefs.SetString(characterName, JsonUtility.ToJson(character));
        PlayerPrefs.SetString(characterName + "Weapon1", JsonUtility.ToJson(character.Weapons[0]));
        PlayerPrefs.SetString(characterName + "Weapon2", JsonUtility.ToJson(character.Weapons[1]));
    }

    public static Character GetCharacter(string characterName)
    {
        var character = JsonUtility.FromJson<Character>(PlayerPrefs.GetString(characterName, Constants.PpSerializeDefault));
        character.Weapons = new List<Weapon>();
        character.Weapons.Add(JsonUtility.FromJson<Weapon>(PlayerPrefs.GetString(Constants.PpPlayerWeapon1, Constants.PpSerializeDefault)));
        character.Weapons[0].RangePositions = WeaponsData.GetWeaponRangeFromType(character.Weapons[0].Type);
        character.Weapons.Add(JsonUtility.FromJson<Weapon>(PlayerPrefs.GetString(Constants.PpPlayerWeapon2, Constants.PpSerializeDefault)));
        character.Weapons[1].RangePositions = WeaponsData.GetWeaponRangeFromType(character.Weapons[1].Type);
        return character;
    }
}
