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
            //"You made it to the other side of the bridge. You gain confidence, and some experience!\nYou gain <material=\"LongOrange\">" + _xpGained + " ®</material>",
            "You gather enough flowers to craft a potion.\n+1 Edelweiss Potion",
            string.Empty, "Ok", OnPositiveOutcome);
    }

    private object OnPositiveOutcome(bool result)
    {
        _character.GainXp(_xpGained);
        _swipeSceneBhv.NewCard(MinutesNeededVenturePositive);
        return result;
    }

    public override void NegativeOutcome()
    {
        _instantiator.NewPopupYesNo("Fail...",
            "The bridge breaks! You however manage to climb up by hanging on to it's remaining half. But you lose <material=\"LongRed\">" + _hpLost + " HP</material> in the process...",
            string.Empty, "Damn", OnNegativeOutcome);
    }

    private object OnNegativeOutcome(bool result)
    {
        _character.TakeDamages(_hpLost);
        _swipeSceneBhv.NewCard(MinutesNeededVentureNegative, false);
        return result;
    }
}
