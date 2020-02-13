using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBiomeBhv : CardBhv
{
    private Biome _biome;
    private int _choice;
    private int _maxChoice;

    public override void SetPrivates(int id, int day, Biome biome, Character character, Instantiator instantiator)
    {
        base.SetPrivates(id, day, biome, character, instantiator);
        _minutesNeededAvoid = 60;
        _minutesNeededVenturePositive = 60;
        _biome = BiomesData.GetRandomBiome();
    }

    public void SetChoice(int choice, int maxChoice)
    {
        _choice = choice;
        _maxChoice = maxChoice;
        DisplayStats();
    }


    public void DisplayStats()
    {
        transform.Find("BiomeName").GetComponent<TMPro.TextMeshPro>().text = _biome.Name;
        transform.Find("BiomePicture").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/BiomePicture_" + _biome.MapType.GetHashCode());
        transform.Find("Choice").GetComponent<TMPro.TextMeshPro>().text = "Choice:\n" + _choice + "/" + _maxChoice;
        transform.Find("Steps").GetComponent<TMPro.TextMeshPro>().text = "Steps:\n" + _biome.Steps;
        transform.Find("Inn").GetComponent<TMPro.TextMeshPro>().text = "Inn: " + _biome.InnPercent + "%";
        transform.Find("Merchant").GetComponent<TMPro.TextMeshPro>().text = "Merchant: " + _biome.MerchantPercent + "%";
        transform.Find("Destinations").GetComponent<TMPro.TextMeshPro>().text = "Destinations: " + _biome.Destinations;
    }

    public override void Venture()
    {
        base.Venture();
        if (Helper.FloatEqualsPrecision(transform.position.x, _likePosition.x, 0.1f))
        {
            gameObject.name = "CardOld";
            _swipeSceneBhv.NewBiome(_biome, _minutesNeededVenturePositive);
        }            
    }
}
