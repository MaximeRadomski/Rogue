using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Maps
{
    public static List<Map> EasyMaps = new List<Map>()
    {
        new Map("Tamplate Map 01", MapType.city, "111110"+"121111"+"011110"+"011110"+"111121"+"011111"),
        new Map("Tamplate Map 02", MapType.city, "011110"+"011110"+"110011"+"110011"+"011110"+"011110"),
        new Map("Tamplate Map 03", MapType.city, "011110"+"111111"+"001100"+"001100"+"111111"+"011110"),
        new Map("Tamplate Map 04", MapType.city, "111111"+"111111"+"000111"+"000111"+"111111"+"111111"),
        new Map("Tamplate Map 05", MapType.city, "111000"+"111111"+"000111"+"000111"+"111111"+"111000"),
        new Map("Tamplate Map 06", MapType.city, "111111"+"111111"+"111111"+"111111"+"111111"+"111111"),
        new Map("Tamplate Map 07", MapType.city, "011110"+"011110"+"011110"+"011110"+"011110"+"011110"),
    };

    public class Map
    {
        public Map(string name, MapType type, string cells)
        {
            Name = name;
            Type = type;
            Cells = cells;
        }

        public string Name;
        public MapType Type;
        public string Cells;
    }

    public enum MapType
    {
        city,
        caves,
        forest,
        mines,
        montains
    }
}
