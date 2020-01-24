using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : InventoryItem
{
    public Consumable()
    {
        Name = "Basic Name";
        InventoryItemType = InventoryItemType.Consumable;
    }
}
