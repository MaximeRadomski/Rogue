using UnityEngine;

public class Journey
{
    public int Day;
    public int Hour;
    public int Minutes;
    public Biome Biome;
    public int Step;
    public int CurrentBiomeChoice;

    public Journey(Character character)
    {
        Day = 1;
        Hour = 8;
        Minutes = 0;
        Biome = BiomesData.GetBiomeFromType(MapType.Mountains);//Biome = BiomesData.GetBiomeFromType(character.StrongIn);
        Step = 1;
    }

    public void UpdateTime(int minutesPassed)
    {
        Minutes += Helper.TimeFromMinutes(minutesPassed, TimeUnit.Minute);
        if (Minutes >= 60)
        {
            Minutes -= 60;
            ++Hour;
        }
        Hour += Helper.TimeFromMinutes(minutesPassed, TimeUnit.Hour);
        if (Hour >= 24)
        {
            Hour -= 24;
            ++Day;
        }
        Day += Helper.TimeFromMinutes(minutesPassed, TimeUnit.Day);
    }
}
