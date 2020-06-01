using Assets.Scripts.Models;
using UnityEngine;

public class CardInnBhv : CardBhv
{
    private AlignmentInn _alignmentInn;
    private int _innId;
    private int _innFee;
    private int _hpRecovered;

    public override void SetPrivates(int id, int day, Biome biome, Character character, Instantiator instantiator)
    {
        base.SetPrivates(id, day, biome, character, instantiator);
        _cacheSpriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/SwipeCardCache_" + biome.MapType.GetHashCode());
        _innId = Random.Range(0, BiomesData.InnNames.Length);
        _innFee = Random.Range(5, 20+1) * character.Level;
        _alignmentInn = AlignmentInn.Classic;
        if (Random.Range(0, 100) < biome.MediocreInnPercentage)
            _alignmentInn = AlignmentInn.Mediocre;
        else if (Random.Range(0, 100) < biome.GoodInnPercentage)
            _alignmentInn = AlignmentInn.Good;
        _minutesNeededAvoid = 60;
        _minutesNeededVenturePositive = (character.SleepHoursNeeded * 60) + (int)(character.SleepHoursNeeded * 60 * Helper.MultiplierFromPercent(0.0f, BiomesData.InnSleepBonusPercent) * _alignmentInn.GetHashCode());
        _hpRecovered = (int)(_character.HpMax * Helper.MultiplierFromPercent(0, _character.SleepRestorationPercent));
        DisplayStats();
    }


    public void DisplayStats()
    {
        var innAlignmentHour = _alignmentInn + "\n";
        innAlignmentHour += Helper.TimeFromMinutes(_minutesNeededVenturePositive);
        transform.Find("InnAlignmentHour").GetComponent<TMPro.TextMeshPro>().text = innAlignmentHour;
        transform.Find("InnFee").GetComponent<TMPro.TextMeshPro>().text = _innFee + " " + Constants.UnitGold;
        transform.Find("InnName").GetComponent<TMPro.TextMeshPro>().text = BiomesData.InnNames[_innId];
        transform.Find("InnLogo").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/SwipeInnLogo_" + _innId);
    }

    public override void Venture()
    {
        base.Venture();
        if (Helper.FloatEqualsPrecision(transform.position.x, _likePosition.x, 0.1f))
        {
            _state = CardState.Off;
            if (_character.Gold >= _innFee)
                OnVenturePositive();
            else
                OnVentureNegative();
        }
    }

    private void OnVenturePositive()
    {
        _instantiator.NewPopupYesNo("Inn",
            MakeContent("Sleep Time Needed: ", Helper.TimeFromMinutes(_character.SleepHoursNeeded * 60))
            + MakeContent(_alignmentInn + " Inn: ", "Sleep Time " + (_alignmentInn != AlignmentInn.Good ? "+" : "") + (_alignmentInn.GetHashCode() * BiomesData.InnSleepBonusPercent) + "%")
            + MakeContent("HP Restoration: ", "+" + _character.SleepRestorationPercent + "%")
            + MakeContent("", "You recover <material=\"LongRed\">" + _hpRecovered + " HP</material>."),
            string.Empty, "Zzz", AfterVenturePositive);
    }

    private object AfterVenturePositive(bool result)
    {
        _instantiator.NewOverBlend(OverBlendType.StartLoadingEndAction, "DEEP SLEEP", 1f, AfterSleep);
        return result;
    }

    private object AfterSleep(bool result)
    {
        _character.GainHp(_hpRecovered);
        _character.LooseGold(_innFee);
        _swipeSceneBhv.NewCard(_minutesNeededVenturePositive, false);
        return result;
    }

    private void OnVentureNegative()
    {
        _instantiator.NewPopupYesNo("Inn", "The innkeeper throws you out of his establishment when he realises you do not have enough <material=\"LongGold\">"
            + Constants.UnitGold + "</material> to rent a room.",
            string.Empty, "Damn", AfterVentureNegative);
    }

    private object AfterVentureNegative(bool result)
    {
        _swipeSceneBhv.NewCard(_minutesNeededAvoid);
        return result;
    }

    private string MakeContent(string libelle, string content)
    {
        return "<material=\"LongGreyish\">" + libelle + "</material><material=\"LongWhite\">" + content + "</material>\n";
    }
}
