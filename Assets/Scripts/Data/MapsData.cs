using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapsData
{
    public static List<Map> EasyMaps = new List<Map>()
    {
        new Map("City 01", MapType.City, "2312132"+"1113111"+"1111111"+"0031400"+"1111111"+"1114111"+"2412142"),
        new Map("City 02", MapType.City, "1100011"+"3112113"+"1113111"+"1131411"+"1114111"+"4112114"+"1100011"),
        new Map("Sewers 01", MapType.Sewers, "0133310"+"1103011"+"1111111"+"1122211"+"1111111"+"1104011"+"0144410"),
        new Map("Sewers 02", MapType.Sewers, "3300011"+"1103031"+"1111111"+"1202021"+"1111111"+"1404011"+"1100044"),
        new Map("Forest 01", MapType.Forest, "3311132"+"2221311"+"1121111"+"1111111"+"1111211"+"1141222"+"2411144"),
        new Map("Forest 02", MapType.Forest, "1331111"+"1013311"+"1112011"+"2111112"+"1102111"+"1144101"+"1111441"),
        new Map("Mines 01", MapType.Mines, "1311300"+"1133111"+"0011111"+"0000111"+"0011111"+"1144111"+"1411400"),
        new Map("Mines 02", MapType.Mines, "1311131"+"1131311"+"0011100"+"1112111"+"0011100"+"1141411"+"1411141"),
        new Map("Mountains 01", MapType.Mountains, "0113311"+"1120323"+"1111022"+"1111111"+"2201111"+"1240211"+"4144110"),
        new Map("Mountains 02", MapType.Mountains, "1332331"+"1110111"+"1121111"+"1111111"+"1111211"+"1110111"+"1442441"),
        new Map("Plains 01", MapType.Plains, "3111113"+"1311131"+"0111111"+"1110111"+"1111110"+"1411141"+"4111114"),
        new Map("Plains 02", MapType.Plains, "1313131"+"1131111"+"2221111"+"1111111"+"1111222"+"1111411"+"1414141"),
        new Map("Beach 01", MapType.Beach, "2311112"+"2111111"+"2213131"+"2211101"+"2214141"+"2111111"+"2411112"),
        new Map("Beach 02", MapType.Beach, "2113332"+"2211011"+"2221311"+"2221111"+"2221411"+"2211011"+"2114442")
    };

    //new Map("TMP 01", MapType.TMP, "1111111"+"1111111"+"1111111"+"1111111"+"1111111"+"1111111"+"1111111"),

    public const int NbOnTemplates = 8;
    public const int NbOffTemplates = 10;
}
