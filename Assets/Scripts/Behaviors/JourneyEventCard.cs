using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class JourneyEventCardBhv : CardBhv
{
    private JourneyEvent _journeyEvent;

    public override void SetPrivates()
    {
        base.SetPrivates();
        _journeyEvent = JourneyEventsData.GetRandomBiome();
    }
}
