using UnityEngine;

class ItemWhetstone : Item
{
    public ItemWhetstone()
    {
        Name = ItemsData.NormalItemsNames[1];
        Description = "Increases a weapon damages by +10.";
        Story = "Sharpens your blade. Doesn't work with bows. A weapon cannot be sharpened more than "+ WeaponsData.MaxSharpenedAmount + " times.";
        Weight = 4;
        Rarity = Rarity.Normal;
        IconId = 4;
        MinutesNeeded = 60;
        PositiveAction = "Use";
        BasePrice = 40;
    }

    public override void OnUse(Character character, int id, System.Func<bool, object> resultAction)
    {
        var instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
        instantiator.NewPopupApplyItem(this, id, character, InventoryItemType.Weapon, AfterUse);        

        object AfterUse(InventoryItem item)
        {
            if (item == null)
                return false;
            var tmpWeapon = (Weapon)item;
            if (tmpWeapon.AmountSharpened >= WeaponsData.MaxSharpenedAmount)
            {
                instantiator.NewSnack("This Weapon cannot be more sharpened than it already is");
                return false;
            }
            tmpWeapon.AmountSharpened++;
            tmpWeapon.BaseDamage += 10;            
            character.Inventory.RemoveAt(id);
            resultAction(true);
            return true;
        }

        
    }
}
