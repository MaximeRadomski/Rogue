﻿using Assets.Scripts.Models;
using UnityEngine;

class CardMerchantBhv : CardBhv
{
    private AlignmentMerchant _alignmentMerchant;
    private InventoryItemType _inventoryItemType;
    private int _merchantId;

    public override void SetPrivates(int id, int day, Biome biome, Character character, Instantiator instantiator)
    {
        base.SetPrivates(id, day, biome, character, instantiator);
        _cacheSpriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/SwipeCardCache_" + biome.MapType.GetHashCode());
        _merchantId = Random.Range(0, BiomesData.MerchantNames.Length);
        _inventoryItemType = (InventoryItemType)Random.Range(0, Helper.EnumCount<InventoryItemType>());
        _alignmentMerchant = AlignmentMerchant.Honest;
        if (Random.Range(0, 100) < biome.FraudulentMerchantPercentage)
            _alignmentMerchant = AlignmentMerchant.Fraudulent;
        else if (Random.Range(0, 100) < biome.HonestMerchantPercentage)
            _alignmentMerchant = AlignmentMerchant.Honest;
        _minutesNeededAvoid = 20;
        _minutesNeededAvoid = 20;
        DisplayStats();
    }

    public void DisplayStats()
    {
        transform.Find("MerchantAlignment").GetComponent<TMPro.TextMeshPro>().text = _alignmentMerchant.GetDescription();
        transform.Find("MerchantName").GetComponent<TMPro.TextMeshPro>().text = _inventoryItemType + "s " + BiomesData.MerchantNames[_merchantId];
        transform.Find("MerchantPicture").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/SwipeCardMerchant_" + _inventoryItemType.GetHashCode());
    }

    public override void Venture()
    {
        base.Venture();
        if (Helper.FloatEqualsPrecision(transform.position.x, _likePosition.x, 0.1f))
        {
            _state = CardState.Off;
            _instantiator.NewPopupMerchant(_character, _alignmentMerchant, _inventoryItemType, isBuying: true, AfterVenture, null);
        }
    }

    private object AfterVenture(bool result)
    {
        _swipeSceneBhv.NewCard(_minutesNeededVenturePositive);
        return result;
    }
}
