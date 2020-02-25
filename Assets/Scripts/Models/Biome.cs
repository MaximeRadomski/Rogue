using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome
{
    public string Name;
    public MapType MapType;
    public int Destinations;
    public int Steps;

    public bool EncounteredInn;
    public int InnPercent;
    public int GoodInnPercentage;
    public int MediocreInnPercentage;

    public bool EncounteredMerchant;
    public int MerchantPercent;
    public int OnSaleMerchantPercentage;
    public int FraudulentMerchantPercentage;
}