using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : InventoryItem
{
    public int IconId;
    public int MinutesNeeded;

    public Item()
    {
        InventoryItemType = InventoryItemType.Item;
    }

    public virtual void OnUse(Character character, int id, System.Func<bool, object> resultAction)
    {

    }
}
