using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem
{
    public string Name;
    public InventoryItemType InventoryItemType;
    public int Weight;
    public Rarity Rarity;
    public string Description;
    public string Story;
    public string PositiveAction = Constants.InventoryItemPositiveAction;
    public string NegativeAction = Constants.InventoryItemNegativeAction;
    public string LootHistory;
    public int BasePrice;

    public int _weaponRandomPriceAdd = -1;

    public virtual string GetNameWithColor()
    {
        var nameTag = "<material=\"LongGreyish\">";
        if (Rarity == Rarity.Magical)
            nameTag = "<material=\"LongBlue\">";
        else if (Rarity == Rarity.Rare)
            nameTag = "<material=\"LongYellow\">";
        return nameTag + Name + "</material>";
    }

    public int GetPrice(Character character, bool isBuying, AlignmentMerchant alignment, int merchantDeal)
    {
        int levelAdd = (BasePrice / 3) * (character.Level - 1);
        int rarityAdd = 0;
        if (Rarity == Rarity.Magical)
            rarityAdd = (int)(BasePrice * 2f);
        else if (Rarity == Rarity.Rare)
            rarityAdd = (int)(BasePrice * 3.5f);
        int finalPrice = BasePrice + levelAdd;
        if (InventoryItemType == InventoryItemType.Weapon)
            finalPrice += rarityAdd;
        finalPrice += _weaponRandomPriceAdd == -1 ? (_weaponRandomPriceAdd = Random.Range(1, BasePrice / 5)) : _weaponRandomPriceAdd;
        if (isBuying)
        {
            float alignmentPercent = 1.0f;
            if (alignment == AlignmentMerchant.Fraudulent)
                alignmentPercent = Helper.MultiplierFromPercent(1, BiomesData.MerchentPriceBonusPercent);
            else if (alignment == AlignmentMerchant.OnSale)
                alignmentPercent = Helper.MultiplierFromPercent(1, -BiomesData.MerchentPriceBonusPercent);
            float merchantDealBuyPercent = Helper.MultiplierFromPercent(1, -merchantDeal);
            return (int)(finalPrice * alignmentPercent * merchantDealBuyPercent);
        }
        else
        {
            float multiplier = 1.0f;
            if (alignment == AlignmentMerchant.Fraudulent)
                multiplier = Helper.MultiplierFromPercent(1, -BiomesData.MerchentPriceBonusPercent);
            else if (alignment == AlignmentMerchant.OnSale)
                multiplier = Helper.MultiplierFromPercent(1, BiomesData.MerchentPriceBonusPercent);
            float merchantDealSellPercent = Helper.MultiplierFromPercent(1, merchantDeal);
            return (int)((finalPrice / 5) * multiplier * merchantDealSellPercent);
        }
    }
}
