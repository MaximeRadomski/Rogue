using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomesData
{
    public static string[] CityNames = { "Nooy Orc", "Small", "Town", "What" };
    public static string[] SewersNames = { "Obscure", "Well Designed", "Clean", "Dirty" };
    public static string[] ForestNames = { "Tall Trees", "Elves", "Dark", "Mystic" };
    public static string[] MinesNames = { "More Iah", "Drunguldur", "Slippy", "Mines" };
    public static string[] MountainsNames = { "Cloudy", "Unscorched", "Not That Tall", "Misty" };
    public static string[] PlainsNames = { "Empty", "Not So Empty", "Grassy", "Rock" };
    public static string[] BeachNames = { "Plastic", "Hideo Kojima's", "Smelly", "Dumb" };
    public static string[][] BiomesNames = { CityNames, SewersNames, ForestNames, MinesNames, MountainsNames, PlainsNames, BeachNames};

    public static string[] InnNames = { "That Crow Tavern", "The Drunk Melody", "The Not Alive Rat", "Cultist Tavern", "The Failed Reich", "Guineff Tavern", "The Galway Gurl",
        "At Deadpool's Wife's", "The Duss Inn", "The Spoon Shaker", "The Irish Coffee", "The Poisonous Venom", "At DOUG's Gate", "The Misty Skull", "The Illusory Wall",
        "Robinson's Inn", "The Return Of The Prince", "The Prince Of Dankness", "The Sleep Inn", "The Gag Ball", "The Magic Wand", "The Spartan", "The Arrow In The Knee",
        "The Gates Of Hell", "El Famoso Taüf", "The Feathers", "Last Calibur", "The Acid Knife", "The Good Boy", "The Unique Horn"};

    public static string[] MerchantNames = { "Merchant", "Vendor", "Trader", "Dealer", "Retailer", "Supplier", "Peddler", "Dude", "Robb... Reseller!" };

    public static int InnSleepBonusPercent = 25;
    public static int MerchentPriceBonusPercent = 15;

    public static Biome GetRandomBiome()
    {
        return GetBiomeFromType((MapType)Random.Range(0, Helper.EnumCount<MapType>()));
    }

    public static Biome GetBiomeFromType(MapType type)
    {
        return GetMountainsBiome();
    }

    public static Biome GetMountainsBiome()
    {
        var type = MapType.Mountains;
        return new Biome()
        {
            Name = BiomesNames[type.GetHashCode()][Random.Range(0, BiomesNames[type.GetHashCode()].Length)] + " " + type,
            MapType = type,
            InnPercent = Random.Range(10, 16),
            GoodInnPercentage = 20,
            MediocreInnPercentage = 20,
            MerchantPercent = Random.Range(10, 16),
            OnSaleMerchantPercentage = 25,
            FraudulentMerchantPercentage = 25,
            Destinations = 2,
            Steps = MaxStepFromMapType(type)
        };
    }

    private static int MaxStepFromMapType(MapType mapType)
    {
        switch (mapType)
        {
            case MapType.City:
                return Random.Range(3, 5 + 1);
            case MapType.Sewers:
                return Random.Range(5, 7 + 1);
            case MapType.Forest:
                return Random.Range(7, 10 + 1);
            case MapType.Mines:
                return Random.Range(5, 7 + 1);
            case MapType.Mountains:
                return Random.Range(7, 10 + 1);
            case MapType.Plains:
                return Random.Range(10, 13 + 1);
            case MapType.Beach:
                return Random.Range(5, 7 + 1);
        }
        return 1;
    }
}
