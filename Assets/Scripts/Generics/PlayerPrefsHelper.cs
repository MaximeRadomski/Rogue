﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerPrefsHelper : MonoBehaviour
{
    public static float GetSpeed()
    {
        return PlayerPrefs.GetFloat(Constants.PpCombatSpeed, Constants.PpCombatSpeedDefault);
    }

    public static void SaveJourney(Journey journey)
    {
        PlayerPrefs.SetString(Constants.PpJourney, JsonUtility.ToJson(journey));
        PlayerPrefs.SetString(Constants.PpCurrentBiome, JsonUtility.ToJson(journey.Biome));
        //Debug.Log(JsonUtility.ToJson(journey));
    }

    public static Journey GetJourney()
    {
        var journey = JsonUtility.FromJson<Journey>(PlayerPrefs.GetString(Constants.PpJourney, Constants.PpSerializeDefault));
        if (journey == null)
            return null;
        journey.Biome = JsonUtility.FromJson<Biome>(PlayerPrefs.GetString(Constants.PpCurrentBiome, Constants.PpSerializeDefault));
        return journey;
    }

    public static void SaveCharacter(string characterName, Character character)
    {
        PlayerPrefs.SetString(characterName, JsonUtility.ToJson(character));
        PlayerPrefs.SetString(characterName + Constants.PpWeapon1, JsonUtility.ToJson(character.Weapons[0]));
        PlayerPrefs.SetString(characterName + Constants.PpWeapon2, JsonUtility.ToJson(character.Weapons[1]));
        PlayerPrefs.SetString(characterName + Constants.PpSkill1, character.Skills != null && character.Skills.Count >= 1 ? character.Skills[0].Name : null);
        PlayerPrefs.SetString(characterName + Constants.PpSkill2, character.Skills != null && character.Skills.Count >= 2 ? character.Skills[1].Name : null);
        // Inventory
        for (int i = 0; i < character.InventoryPlace; ++i)
        {
            if (character.Inventory == null || i >= character.Inventory.Count)
            {
                PlayerPrefs.SetString(characterName + Constants.PpInventoryItem + i, null);
                continue;
            }
            if (character.Inventory[i].InventoryItemType == InventoryItemType.Skill
                || character.Inventory[i].InventoryItemType == InventoryItemType.Item)
                PlayerPrefs.SetString(characterName + Constants.PpInventoryItem + i, character.Inventory[i].Name);
            else
                PlayerPrefs.SetString(characterName + Constants.PpInventoryItem + i, JsonUtility.ToJson(character.Inventory[i]));
            PlayerPrefs.SetInt(characterName + Constants.PpInventoryItem + i + "Type", character.Inventory[i].InventoryItemType.GetHashCode());
        }
        //Debug.Log("Pp" + characterName + " :\n" + PlayerPrefs.GetString(characterName) + "\n\n" +
        //    "Pp" + characterName + Constants.PpWeapon1 + " :\n" + PlayerPrefs.GetString(characterName + Constants.PpWeapon1) + "\n\n" +
        //    "Pp" + characterName + Constants.PpWeapon2 + " :\n" + PlayerPrefs.GetString(characterName + Constants.PpWeapon2) + "\n\n" +
        //    "Pp" + characterName + Constants.PpSkill1 + " :\n" + PlayerPrefs.GetString(characterName + Constants.PpSkill1) + "\n\n" +
        //    "Pp" + characterName + Constants.PpSkill2 + " :\n" + PlayerPrefs.GetString(characterName + Constants.PpSkill2) + "\n\n");
    }

    public static Character GetCharacter(string characterName)
    {
        var character = JsonUtility.FromJson<Character>(PlayerPrefs.GetString(characterName, Constants.PpSerializeDefault));
        character.Weapons = new List<Weapon>();
        character.Weapons.Add(JsonUtility.FromJson<Weapon>(PlayerPrefs.GetString(characterName + Constants.PpWeapon1, Constants.PpSerializeDefault)));
        character.Weapons.Add(JsonUtility.FromJson<Weapon>(PlayerPrefs.GetString(characterName + Constants.PpWeapon2, Constants.PpSerializeDefault)));
        character.Skills = new List<Skill>();
        character.Skills.Add(SkillsData.GetSkillFromName(PlayerPrefs.GetString(characterName + Constants.PpSkill1, Constants.PpSerializeDefault)));
        character.Skills.Add(SkillsData.GetSkillFromName(PlayerPrefs.GetString(characterName + Constants.PpSkill2, Constants.PpSerializeDefault)));
        character.Inventory = new List<InventoryItem>();
        for (int i = 0; i < character.InventoryPlace; ++i)
        {
            var serialized = PlayerPrefs.GetString(characterName + Constants.PpInventoryItem + i, Constants.PpSerializeDefault);
            if (string.IsNullOrEmpty(serialized))
                continue;
            var typeId = PlayerPrefs.GetInt(characterName + Constants.PpInventoryItem + i + "Type");
            if (typeId == InventoryItemType.Weapon.GetHashCode())
                character.Inventory.Add(JsonUtility.FromJson<Weapon>(serialized));
            else if (typeId == InventoryItemType.Skill.GetHashCode())
                character.Inventory.Add(SkillsData.GetSkillFromName(serialized));
            else
                character.Inventory.Add(ItemsData.GetItemFromName(serialized));
        }
        return character;
    }

    public static void SaveSoul(Soul soul)
    {
        PlayerPrefs.SetString(Constants.PpSoul, JsonUtility.ToJson(soul));
        Debug.Log(JsonUtility.ToJson(soul));
    }

    public static Soul GetSoul()
    {
        var soul = JsonUtility.FromJson<Soul>(PlayerPrefs.GetString(Constants.PpSoul, Constants.PpSerializeDefault));
        if (soul == null)
            soul = new Soul();
        return soul;
    }

    public static void ResetSoul()
    {
        PlayerPrefs.SetString(Constants.PpSoul, Constants.PpSerializeDefault);
        Debug.Log("\t[DEBUG]\tSoul Reseted");
    }

    public static void SaveToXML<T>(object tmpObject, string name)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        FileStream stream = new FileStream(Application.dataPath + "/XML/" + name + ".xml", FileMode.Create);
        serializer.Serialize(stream, tmpObject);
        stream.Close();
    }

    public static T GetFromXML<T>(string name)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        FileStream stream = new FileStream(Application.dataPath + "/XML/" + name + ".xml", FileMode.Open);
        var tmpObject = (T)serializer.Deserialize(stream);
        stream.Close();
        return tmpObject;
    }
}
