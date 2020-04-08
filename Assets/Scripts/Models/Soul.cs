public class Soul
{
    public int XpKept = 0;
    public static string[] SoulStats = { "RunAwayPercent", "LootPercent", "CritChance", "InvPlace", "InvWeight", "Gold", "XpKeptPercent",
                                         "StartingLevel", "MerchantDeal", "Health", "Pm", "Pa"};

    // FIGHT
    public int RunAwayPercent = 25, RunAwayPercent_Add = 5, RunAwayPercent_Level = 0,   RunAwayPercent_Max = 10,    RunAwayPercent_Price = 80,  RunAwayPercent_Id = 0;
    public int LootPercent = 25,    LootPercent_Add = 5,    LootPercent_Level = 0,      LootPercent_Max = 10,       LootPercent_Price = 120,    LootPercent_Id = 1;
    public int CritChance = 0,      CritChance_Add = 3,     CritChance_Level = 0,       CritChance_Max = 5,         CritChance_Price = 250,     CritChance_Id = 2;
    // JOURNEY
    public int InvPlace = 0,        InvPlace_Add = 1,       InvPlace_Level = 0,         InvPlace_Max = 2,           InvPlace_Price = 1000,      InvPlace_Id = 3;
    public int InvWeight = 0,       InvWeight_Add = 5,      InvWeight_Level = 0,        InvWeight_Max = 4,          InvWeight_Price = 500,      InvWeight_Id = 4;
    public int Gold = 0,            Gold_Add = 20,          Gold_Level = 0,             Gold_Max = 6,               Gold_Price = 100,           Gold_Id = 5;
    public int XpKeptPercent = 0,   XpKeptPercent_Add = 10, XpKeptPercent_Level = 0,    XpKeptPercent_Max = 5,      XpKeptPercent_Price = 250,  XpKeptPercent_Id = 6;
    // CHARACTER
    public int StartingLevel = 1,   StartingLevel_Add = 1,  StartingLevel_Level = 0,    StartingLevel_Max = 8,      StartingLevel_Price = 300,  StartingLevel_Id = 7;
    public int MerchantDeal = 0,    MerchantDeal_Add = 5,   MerchantDeal_Level = 0,     MerchantDeal_Max = 10,      MerchantDeal_Price = 120,   MerchantDeal_Id = 8;
    public int Health = 0,          Health_Add = 20,        Health_Level = 0,           Health_Max = 5,             Health_Price = 300,         Health_Id = 9;
    public int Pm = 0,              Pm_Add = 1,             Pm_Level = 0,               Pm_Max = 1,                 Pm_Price = 20000,           Pm_Id = 10;
    public int Pa = 0,              Pa_Add = 1,             Pa_Level = 0,               Pa_Max = 2,                 Pa_Price = 15000,           Pa_Id = 11;
}
