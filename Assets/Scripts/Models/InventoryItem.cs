using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem
{
    public string Name;
    public InventoryItemType InventoryItemType;
    public int Weight;
    public Rarity Rarity;
    public string Description;

    public virtual string GetNameWithColor()
    {
        var weaponTag = "<material=\"LongWhite\">";
        if (Rarity == Rarity.Magical)
            weaponTag = "<material=\"LongBlue\">";
        else if (Rarity == Rarity.Rare)
            weaponTag = "<material=\"LongYellow\">";
        return weaponTag + Name + "</material>";
    }
}
