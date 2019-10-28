using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int X;
    public int Y;

    public string Name;
    public CharacterRace Race;
    public int Level;
    public int Experience;
    public int Gold;
    public int Hp;
    public int Pm;
    public List<Weapon> Weapons;

    void Start()
    {
        
    }

    public enum CharacterRace
    {
        Human = 0,
        Gobelin = 1,
        Elf = 2,
        Dwarf = 3,
        Orc = 4
    }
}
