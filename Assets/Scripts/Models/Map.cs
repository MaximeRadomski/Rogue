using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
