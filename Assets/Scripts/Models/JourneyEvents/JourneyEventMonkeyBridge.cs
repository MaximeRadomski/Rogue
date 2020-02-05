using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyEventMonkeyBridge : JourneyEvent
{
    private int _xpGained;
    private int _hpLost;

    public JourneyEventMonkeyBridge()
    {
        Name = JourneyEventsData.MountainsEvent[0];
        PositiveOutcomePercent = Helper.RandomIntMultipleOf(50, 95, 5);
        MinutesNeededAvoid = 90;
        MinutesNeededVenturePositive = 5;
        MinutesNeededVentureNegative = 15;
        Content = "You find a shortcut in a canyon! But it is a monkey bridge. The ropes seem old enough to alter your heights appreciation...";
    }

    public override void SetPrivates(Instantiator instantiator, Character character, SwipeSceneBhv swipeSceneBhv)
    {
        base.SetPrivates(instantiator, character, swipeSceneBhv);
        _xpGained = (int)(Helper.XpNeedForLevel(_character.Level) * 0.2f);
        _hpLost = (int)(_character.HpMax * 0.15f);
    }

    public override void PositiveOutcome()
    {
        _instantiator.NewPopupYesNo("Succes!",
            "You made it to the other side of the bridge. You gain confidence, and some experience!\nYou gain <material=\"LongOrange\">" + _xpGained + " ®</material>",
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
