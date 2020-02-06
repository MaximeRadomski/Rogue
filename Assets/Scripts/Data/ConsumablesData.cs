using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConsumablesData 
{
    public static int RareConsumableAppearancePercent = 10;
    public static int MagicalConsumableAppearancePercent = 30;

    public static string[] NormalConsumablesNames = { "Health Potion" };
    public static string[] MagicalConsumablesNames = { "Health Potion" }; //TODO
    public static string[] RareConsumablesNames = { "Edelweiss Potion" };

    public static Consumable GetRandomConsumable(bool isPlayer = false)
    {
        if (isPlayer)
            return GetRandomConsumableFromRarity(Rarity.Normal);
        int rarityPercent = UnityEngine.Random.Range(0, 100);
        if (rarityPercent < RareConsumableAppearancePercent)
            return GetRandomConsumableFromRarity(Rarity.Rare);
        else if (rarityPercent < MagicalConsumableAppearancePercent)
            return GetRandomConsumableFromRarity(Rarity.Magical);
        return GetRandomConsumableFromRarity(Rarity.Normal);
    }

    public static Consumable GetRandomConsumableFromRarity(Rarity rarity)
    {
        if (rarity == Rarity.Rare)
            return GetConsumableFromName(RareConsumablesNames[UnityEngine.Random.Range(0, RareConsumablesNames.Length)]);
        else if (rarity == Rarity.Magical)
            return GetConsumableFromName(MagicalConsumablesNames[UnityEngine.Random.Range(0, MagicalConsumablesNames.Length)]);
        return GetConsumableFromName(NormalConsumablesNames[UnityEngine.Random.Range(0, NormalConsumablesNames.Length)]);
    }

    public static Consumable GetConsumableFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        var instance = Activator.CreateInstance(Type.GetType("Consumable" + name.Replace(" ", "")));
        return (Consumable)instance;
    }
}
