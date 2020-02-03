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
            MerchantPercent = Random.Range(10, 16),
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
