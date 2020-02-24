using UnityEngine;

public class ItemEdelweissPotion : Item
{
    public ItemEdelweissPotion()
    {
        Name = ItemsData.RareItemsNames[0];
        Description = "Recover <material=\"LongRed\">+15%</material> of your max <material=\"LongRed\">HP</material>";
        Story = "This potion is made from rare Edelweiss. It is said your experience alter its effects a lot.";
        Weight = 1;
        Rarity = Rarity.Rare;
        IconId = 1;
        MinutesNeeded = 10;
        PositiveAction = "Drink";
    }

    public override void OnUse(Character character, int id, System.Func<bool, object> resultAction)
    {
        if (character.Hp < character.HpMax)
        {
            character.GainHp((int)(character.HpMax * 0.15f));
            character.Inventory.RemoveAt(id);
            resultAction(true);
        }
        else
        {
            var instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
            instantiator.NewPopupYesNo("Nope", "Your life is already maxed out!", string.Empty, "Indeed", AfterWarning);
        }

        object AfterWarning(bool result)
        {
            resultAction(false);
            return result;
        }
    }
}
