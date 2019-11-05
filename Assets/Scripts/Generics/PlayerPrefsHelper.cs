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
        PlayerPrefs.SetString(characterName + "Skill1", character.Skills != null && character.Skills.Count >= 1 ? character.Skills[0].Name : null);
        PlayerPrefs.SetString(characterName + "Skill2", character.Skills != null && character.Skills.Count >= 2 ? character.Skills[1].Name : null);
    }

    public static Character GetCharacter(string characterName)
    {
        var character = JsonUtility.FromJson<Character>(PlayerPrefs.GetString(characterName, Constants.PpSerializeDefault));
        character.Weapons = new List<Weapon>();
        character.Weapons.Add(JsonUtility.FromJson<Weapon>(PlayerPrefs.GetString(characterName + "Weapon1", Constants.PpSerializeDefault)));
        character.Weapons.Add(JsonUtility.FromJson<Weapon>(PlayerPrefs.GetString(characterName + "Weapon2", Constants.PpSerializeDefault)));
        character.Skills = new List<Skill>();
        character.Skills.Add(RacesData.SkillsData.GetSkillFromName(PlayerPrefs.GetString(characterName + "Skill1", Constants.PpSerializeDefault)));
        character.Skills.Add(RacesData.SkillsData.GetSkillFromName(PlayerPrefs.GetString(characterName + "Skill2", Constants.PpSerializeDefault)));
        return character;
    }
}
