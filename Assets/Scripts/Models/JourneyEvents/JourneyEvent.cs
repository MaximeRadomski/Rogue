using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class JourneyEvent
{
    public string Name;
    public string Content;
    public int PositiveOutcomePercent;

    protected virtual void NegativeOutcome()
    {

    }

    protected virtual void PositiveOutcome()
    {

    }
}
