using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInnBhv : CardBhv
{
    private AlignmentInn _alignmentInn;
    private int _innId;
    private int _hpRecovered;

    public override void SetPrivates(int id, int day, Biome biome, Character character, Instantiator instantiator)
    {
        base.SetPrivates(id, day, biome, character, instantiator);
        _cacheSpriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/SwipeCardCache_" + biome.MapType.GetHashCode());
        _innId = Random.Range(0, BiomesData.InnNames.Length);
        _alignmentInn = AlignmentInn.Classic;
        if (Random.Range(0, 100) < biome.MediocreInnPercentage)
            _alignmentInn = AlignmentInn.Mediocre;
        else if (Random.Range(0, 100) < biome.GoodInnPercentage)
            _alignmentInn = AlignmentInn.Good;
        _minutesNeededAvoid = 60;
        _minutesNeededVenturePositive = (character.SleepHoursNeeded * 60) + (character.SleepHoursNeeded * BiomesData.InnSleepBonusPercent * _alignmentInn.GetHashCode());
        _hpRecovered = (int)(_character.Hp * Helper.MultiplierFromPercent(1, _character.SleepRestorationPercent));
        DisplayStats();
    }


    public void DisplayStats()
    {
        var innAlignmentHour = _alignmentInn + "\n";
        innAlignmentHour += Helper.TimeFromMinutes(_minutesNeededVenturePositive);
        transform.Find("InnAlignmentHour").GetComponent<TMPro.TextMeshPro>().text = innAlignmentHour;
        transform.Find("InnName").GetComponent<TMPro.TextMeshPro>().text = BiomesData.InnNames[_innId];
        transform.Find("InnLogo").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/SwipeInnLogo_" + _innId);
    }

    public override void Venture()
    {
        base.Venture();
        if (Helper.FloatEqualsPrecision(transform.position.x, _likePosition.x, 0.1f))
            OnVenture();
    }

    private void OnVenture()
    {
        _state = CardState.Off;
        _instantiator.NewPopupYesNo("Inn",
            MakeContent(_alignmentInn + " Inn: ", "Sleep Time " + (_alignmentInn != AlignmentInn.Mediocre ? "+" : "") + (_alignmentInn.GetHashCode() * BiomesData.InnSleepBonusPercent) + "%")
            + MakeContent("Sleep Time: ", Helper.TimeFromMinutes(_character.SleepHoursNeeded * 60))
            + MakeContent("Race HP Restoration: ", "+" + _character.SleepRestorationPercent + "%")
            + MakeContent("", "You recover <material=\"LongRed\">" + _hpRecovered + " HP</material>."),
            string.Empty, "Zzz", AfterSleep);
    }

    private object AfterSleep(bool result)
    {
        _character.GainHp(_hpRecovered);
        _swipeSceneBhv.NewCard(_minutesNeededVenturePositive);
        return result;
    }

    private string MakeContent(string libelle, string content)
    {
        return "<material=\"LongGreyish\">" + libelle + "</material><material=\"LongWhite\">" + content + "</material>\n";
    }
}
