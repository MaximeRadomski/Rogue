using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyEvent
{
    public string Name;
    public string Content;
    public int PositiveOutcomePercent;
    public int MinutesNeededAvoid;
    public int MinutesNeededVenturePositive;
    public int MinutesNeededVentureNegative;

    protected Instantiator _instantiator;
    protected Character _character;
    protected SwipeSceneBhv _swipeSceneBhv;

    public virtual void SetPrivates(Instantiator instantiator, Character character, SwipeSceneBhv swipeSceneBhv)
    {
        _instantiator = instantiator;
        _character = character;
        _swipeSceneBhv = swipeSceneBhv;
    }

    public virtual void NegativeOutcome()
    {

    }

    public virtual void PositiveOutcome()
    {

    }
}
