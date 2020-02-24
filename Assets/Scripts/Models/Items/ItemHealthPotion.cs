using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealthPotion : Item
{
    public ItemHealthPotion()
    {
        Name = ItemsData.NormalItemsNames[0];
        Description = "Heal <material=\"LongRed\">" + _hp + " HP</material>";
        Story = "Made with plants, river water and bat blood, this potion taste is dreadful. And the purpose of the plants is only aromatic...";
        Weight = 2;
        Rarity = Rarity.Normal;
        IconId = 0;
        MinutesNeeded = 15;
        PositiveAction = "Drink";
    }

    private int _hp = 75;

    public override void OnUse(Character character, int id, System.Func<bool, object> resultAction)
    {
        if (character.Hp < character.HpMax)
        {
            character.GainHp(_hp);
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
