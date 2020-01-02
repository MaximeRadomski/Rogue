using UnityEngine;

public class Journey
{
    public int Day;
    public int Hour;
    public int Minutes;
    public MapType Biome;
    public int Step;
    public int MaxStep;

    public Journey(Character character)
    {
        Day = 1;
        Hour = 8;
        Minutes = 0;
        Biome = MapType.Mountains;//character.StrongIn;
        Step = 1;
        MaxStep = MaxStepFromMapType(Biome);
    }

    private int MaxStepFromMapType(MapType mapType)
    {
        switch (mapType)
        {
            case MapType.City:
                return Random.Range(3, 5 + 1);
            case MapType.Sewers:
                return Random.Range(5, 7 + 1);
            case MapType.Forest:
                return Random.Range(7, 20 + 1);
            case MapType.Mines:
                return Random.Range(3, 15 + 1);
            case MapType.Mountains:
                return Random.Range(7, 20 + 1);
        }
        return 1;
    }
}
