using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JourneyEventsData
{
    public static string[] CityEvent = { "Street Fight" };
    public static string[] SewersEvent = { "Forgotten Corpse" };
    public static string[] ForestEvent = { "Funny Mushrooms" };
    public static string[] MinesEvent = { "Stone Well" };
    public static string[] MountainsEvent = { "Monkey Bridge", "EdelweissField" };
    public static string[] PlainsEvent = { "Vegetable Field" };
    public static string[] BeachEvent = { "Stranded Whale" };
    public static string[][] BiomesEvent = { CityEvent, SewersEvent, ForestEvent, MinesEvent, MountainsEvent, PlainsEvent, BeachEvent };

    public static JourneyEvent GetRandomJourneyEventFromBiome(MapType type)
    {
        return GetJourneyEventFromName(BiomesEvent[type.GetHashCode()][UnityEngine.Random.Range(0, BiomesEvent[type.GetHashCode()].Length)]);
    }

    public static JourneyEvent GetJourneyEventFromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        var instance = Activator.CreateInstance(Type.GetType("JourneyEvent" + name.Replace(" ", "")));
        return (JourneyEvent)instance;
    }
}
