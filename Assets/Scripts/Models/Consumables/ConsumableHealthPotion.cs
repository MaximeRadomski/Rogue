using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableHealthPotion : Consumable
{
    public ConsumableHealthPotion()
    {
        Name = ConsumablesData.NormalConsumablesNames[0];
        Description = "Heal <material=\"LongRed\">" + _hp + " HP</material>";
        Story = "Made with plants, river water and bat blood, this potion taste is dreadful. And the purpose of the plants is only aromatic...";
        Weight = 2;
        Rarity = Rarity.Normal;
        IconId = 0;
        MinutesNeeded = 15;
        PositiveAction = "Drink";
    }

    private int _hp = 75;

    public override void OnUse(object target)
    {
        var character = (Character)target;
        character.GainHp(_hp);
        base.OnUse(target);
    }
}
