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
        var nameTag = "<material=\"LongWhite\">";
        if (Rarity == Rarity.Magical)
            nameTag = "<material=\"LongBlue\">";
        else if (Rarity == Rarity.Rare)
            nameTag = "<material=\"LongYellow\">";
        return nameTag + Name + "</material>";
    }
}
