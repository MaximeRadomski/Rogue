using UnityEngine;

public class Journey
{
    public int Day;
    public int Hour;
    public int Minutes;
    public Biome Biome;
    public int Step;

    public Journey(Character character)
    {
        Day = 1;
        Hour = 8;
        Minutes = 0;
        Biome = BiomesData.GetBiomeFromType(MapType.Mountains);//Biome = BiomesData.GetBiomeFromType(character.StrongIn);
        Step = 1;
    }
}
