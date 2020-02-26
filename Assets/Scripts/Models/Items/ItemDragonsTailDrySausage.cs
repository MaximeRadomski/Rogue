using UnityEngine;

class ItemDragonsTailDrySausage : Item
{
    public ItemDragonsTailDrySausage()
    {
        Name = ItemsData.MagicalItemsNames[0];
        Description = "Boost maximum equipment load by 5 " + Constants.UnitWeight;
        Story = "A dish favoured by Havel the Rock, the famous knight in dragon's scale armor.";
        Weight = 3;
        Rarity = Rarity.Magical;
        IconId = 2;
        MinutesNeeded = 45;
        PositiveAction = "Eat";
        BasePrice = 75;
    }

    public override void OnUse(Character character, int id, System.Func<bool, object> resultAction)
    {
        character.WeightLimit += 5;
        character.Inventory.RemoveAt(id);
        resultAction(true);
    }
}
