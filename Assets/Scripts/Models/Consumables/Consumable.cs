using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : InventoryItem
{
    public int IconId;
    public int MinutesNeeded;

    public Consumable()
    {
        InventoryItemType = InventoryItemType.Consumable;
    }

    public virtual void OnUse(Character character, int id, System.Func<bool, object> resultAction)
    {

    }
}
