using System.Reflection;

public class Soul
{
    public int Xp = 0;
    public int XpKept = 0;
    public static string[] SoulStats = { "RunAwayPercent", "LootPercent", "CritChance", "InvPlace", "InvWeight", "Gold", "XpKeptPercent",
                                         "StartingLevel", "MerchantDeal", "Health", "Pm", "Pa", "NbCharChoice"};
    public static string[] SoulStatsNames = { "Cowardice", "Luckiness", "Precision", "Large Pockets", "Strong Back", "Birthright", "Heredity",
                                              "Nature Strength", "Haggler", "Constitution", "Agility", "Attunement", "Diffusion"};
    public static string[] SoulStatsDescriptions = { "Add chance to run away.", "Increase your chance to loot after a fight.", "Increase your chance to deal critical hits.",
                                                     "Add slots to your inventory.", "Increase the weight you can carry.", "Start your journey with some gold.",
                                                     "Keep some of your experience after being killed.", "Find hosts with better starting levels.",
                                                     "Bargain better deals with merchants.", "Find hosts with more health points.", "Grant you a mouvement points.",
                                                     "Grant you action points.", "Increase the number of potential hosts."};
    public static string[] SoulStatsUnit = { "%", "%", "%", "Slot", Constants.UnitWeight, "<material=\"LongGold\">" + Constants.UnitGold + "</material>",
                                             "<material=\"LongOrange\">" + Constants.UnitXp + "</material>", "Level", "%", "<material=\"LongRed\">HP</material>",
                                             "<material=\"LongGreen\">MP</material>", "<material=\"LongBlue\">AP</material>", "Choice"};

    // FIGHT
    public int RunAwayPercent = 0,  RunAwayPercent_Add = 5, RunAwayPercent_Level = 0,   RunAwayPercent_Max = 10,    RunAwayPercent_Price = 80,  RunAwayPercent_Id = 0;
    public int LootPercent = 0,     LootPercent_Add = 5,    LootPercent_Level = 0,      LootPercent_Max = 10,       LootPercent_Price = 120,    LootPercent_Id = 1;
    public int CritChance = 0,      CritChance_Add = 3,     CritChance_Level = 0,       CritChance_Max = 5,         CritChance_Price = 250,     CritChance_Id = 2;
    public int Pm = 0,              Pm_Add = 1,             Pm_Level = 0,               Pm_Max = 1,                 Pm_Price = 20000,           Pm_Id = 10;
    public int Pa = 0,              Pa_Add = 1,             Pa_Level = 0,               Pa_Max = 2,                 Pa_Price = 15000,           Pa_Id = 11;
    // JOURNEY
    public int InvPlace = 0,        InvPlace_Add = 1,       InvPlace_Level = 0,         InvPlace_Max = 2,           InvPlace_Price = 1000,      InvPlace_Id = 3;
    public int InvWeight = 0,       InvWeight_Add = 5,      InvWeight_Level = 0,        InvWeight_Max = 4,          InvWeight_Price = 500,      InvWeight_Id = 4;
    public int Gold = 0,            Gold_Add = 20,          Gold_Level = 0,             Gold_Max = 6,               Gold_Price = 100,           Gold_Id = 5;
    public int XpKeptPercent = 0,   XpKeptPercent_Add = 10, XpKeptPercent_Level = 0,    XpKeptPercent_Max = 5,      XpKeptPercent_Price = 250,  XpKeptPercent_Id = 6;
    // CHARACTER
    public int StartingLevel = 1,   StartingLevel_Add = 1,  StartingLevel_Level = 0,    StartingLevel_Max = 8,      StartingLevel_Price = 300,  StartingLevel_Id = 7;
    public int MerchantDeal = 0,    MerchantDeal_Add = 5,   MerchantDeal_Level = 0,     MerchantDeal_Max = 10,      MerchantDeal_Price = 120,   MerchantDeal_Id = 8;
    public int Health = 0,          Health_Add = 20,        Health_Level = 0,           Health_Max = 5,             Health_Price = 300,         Health_Id = 9;
    public int NbCharChoice = 1,    NbCharChoice_Add = 1,   NbCharChoice_Level = 0,     NbCharChoice_Max = 5,       NbCharChoice_Price = 500,   NbCharChoice_Id = 12;

    public int GetStatCurrentValue(string name)
    {
        int stat = (int)GetFieldValue(name);
        int statLevel = (int)GetFieldValue(name + "_Level");
        int statAdd = (int)GetFieldValue(name + "_Add");
        return stat + (statLevel * statAdd);
    }

    public object GetFieldValue(string propertyName)
    {
        var tmp = GetType().GetField(propertyName);
        return tmp.GetValue(this);
    }
}
