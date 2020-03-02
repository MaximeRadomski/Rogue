using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapsData
{
    public static List<Map> EasyMaps = new List<Map>()
    {
        new Map("City 01", MapType.City, "1311131"+"1113111"+"1111111"+"0011100"+"1111111"+"1114111"+"1411141"),
        new Map("City 02", MapType.City, "1100011"+"3112113"+"1113111"+"1111111"+"1114111"+"4112114"+"1100011"),
        new Map("Sewers 01", MapType.Sewers, "0131310"+"1103011"+"1111111"+"1122211"+"1111111"+"1104011"+"0141410"),
        new Map("Sewers 02", MapType.Sewers, "1300011"+"1103011"+"1111111"+"1202021"+"1111111"+"1104011"+"1100041"),
        new Map("Forest 01", MapType.Forest, "1311131"+"2221311"+"1121111"+"1111111"+"1111211"+"1141222"+"1411141"),
        new Map("Forest 02", MapType.Forest, "1311111"+"1013111"+"1112011"+"2111112"+"1102111"+"1114101"+"1111141"),
        new Map("Mines 01", MapType.Mines, "1311300"+"1111111"+"0011111"+"0000111"+"0011111"+"1111111"+"1411400"),
        new Map("Mines 02", MapType.Mines, "1311111"+"1131111"+"0011100"+"1112111"+"0011100"+"1111411"+"1111141"),
        new Map("Mountains 01", MapType.Mountains, "0113111"+"1120321"+"1111022"+"1111111"+"2201111"+"1240211"+"1114110"),
        new Map("Mountains 02", MapType.Mountains, "1132311"+"1110111"+"1121111"+"1111111"+"1111211"+"1110111"+"1142411")
    };

    public const int NbOnTemplates = 8;
    public const int NbOffTemplates = 10;
}
