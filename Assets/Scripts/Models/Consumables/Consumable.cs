using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : InventoryItem
{
    public int IconId;
    public int MinutesNeeded;

    public Consumable()
    {
        InventoryItemType = InventoryItemType.Consumable;
    }

    public virtual void OnUse(object target)
    {
        
    }
}
