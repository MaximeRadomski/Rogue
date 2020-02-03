using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyEvent
{
    protected string _name;
    protected string _content;
    protected int _positiveOutcomePercent;

    protected virtual void NegativeOutcome()
    {

    }

    protected virtual void PositiveOutcome()
    {

    }
}
