using UnityEngine;

public class CardJourneyEventBhv : CardBhv
{
    public JourneyEvent JourneyEvent;

    public override void SetPrivates(int id, int day, MapType mapType, Character character, Instantiator instantiator)
    {
        base.SetPrivates(id, day, mapType, character, instantiator);
        _cacheSpriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/SwipeCardCache_" + mapType.GetHashCode());
        JourneyEvent = JourneyEventsData.GetRandomJourneyEventFromBiome(mapType);
        _minutesNeededAvoid = JourneyEvent.MinutesNeededAvoid;
        _minutesNeededVenturePositive = JourneyEvent.MinutesNeededVenturePositive;
        _minutesNeededVentureNegative = JourneyEvent.MinutesNeededVentureNegative;
        PositiveOutcomePercent = JourneyEvent.PositiveOutcomePercent;
        JourneyEvent.SetPrivates(instantiator, character, _swipeSceneBhv);
        DisplayInfo();
    }    

    private void DisplayInfo()
    {
        transform.Find("Name").GetComponent<TMPro.TextMeshPro>().text = JourneyEvent.Name;
        transform.Find("Content").GetComponent<TMPro.TextMeshPro>().text = JourneyEvent.Content;
    }

    public override void Venture()
    {
        base.Venture();
        if (Helper.FloatEqualsPrecision(transform.position.x, _likePosition.x, 1.0f))
            OnVenture();
        
    }

    private void OnVenture()
    {
        var percent = Random.Range(0, 100);
        if (percent < JourneyEvent.PositiveOutcomePercent)
            JourneyEvent.PositiveOutcome();
        else
            JourneyEvent.NegativeOutcome();
        _state = CardState.Off;
    }
}
