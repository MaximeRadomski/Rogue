using UnityEngine;

class ItemSummonedBroccoli : Item
{
    public ItemSummonedBroccoli()
    {
        Name = ItemsData.RareItemsNames[1];
        Description = "Reduces your needed sleep time by one hour.";
        Story = "A fail attempt to invoke a powerful daemon. Occultists can't however tell why it always results in a broccoli.";
        Weight = 1;
        Rarity = Rarity.Rare;
        IconId = 3;
        MinutesNeeded = 10;
        PositiveAction = "Eat";
        BasePrice = 150;
    }

    public override void OnUse(Character character, int id, System.Func<bool, object> resultAction)
    {
        if (character.Diet == Diet.Carnivorous)
        {
            var instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
            instantiator.NewPopupYesNo("Wrong Diet", "You are carnivorous...\nYou won't eat it.", string.Empty, "Hmm...", AfterDiet);
            object AfterDiet(bool result)
            {
                resultAction(false);
                return result;
            }
            return;
        }
        else if (character.SleepHoursNeeded <= 1)
        {
            var instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
            instantiator.NewPopupYesNo("Sleep Limit", "You cannot sleep less than " + character.SleepHoursNeeded + " hour.", string.Empty, "Oh ok", AfterSleepLimit);
            object AfterSleepLimit(bool result)
            {
                resultAction(false);
                return result;
            }
            return;
        }
        else
        {
            character.SleepHoursNeeded -= 1;
            character.Inventory.RemoveAt(id);
            resultAction(true);
        }
    }
}
