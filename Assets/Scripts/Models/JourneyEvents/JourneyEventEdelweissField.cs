public class JourneyEventEdelweissField : JourneyEvent
{
    public JourneyEventEdelweissField()
    {
        Name = JourneyEventsData.MountainsEvent[1];
        PositiveOutcomePercent = Helper.RandomIntMultipleOf(30, 60, 5);
        MinutesNeededAvoid = 10;
        MinutesNeededVenturePositive = 120;
        MinutesNeededVentureNegative = 120;
        Content = "An Edelweiss field lies before you. This flower is rare, but you can make a potion out of it. It might juste take some time...";
    }

    public override void PositiveOutcome()
    {
        _instantiator.NewPopupYesNo("Succes!",
            "It took you a long amount of time, but you gathered enough flowers to craft a potion out of it. <material=\"LongYellow\">+1 Edelweiss Potion</material>",
            string.Empty, "Ok", OnPositiveOutcome);
    }

    private object OnPositiveOutcome(bool result)
    {
        _character.AddToInventory(new System.Collections.Generic.List<InventoryItem>
        {
            new ConsumableEdelweissPotion()
        }, OnInventoryWork);
        return result;
    }

    private object OnInventoryWork(bool result)
    {
        _swipeSceneBhv.NewCard(MinutesNeededVenturePositive);
        return result;
    }

    public override void NegativeOutcome()
    {
        _instantiator.NewPopupYesNo("Fail...",
            "You waisted your time looking for some Edelweiss, but couldn't find enough to make a potion out of it..",
            string.Empty, "Damn", OnNegativeOutcome);
    }

    private object OnNegativeOutcome(bool result)
    {
        _swipeSceneBhv.NewCard(MinutesNeededVentureNegative, false);
        return result;
    }
}
