using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public CharacterGender Gender;
    public string Name;
    public CharacterRace Race;
    public CharacterRace StrongAgainst;
    public MapType StrongIn;
    public int Level;
    public int LevelingHealthPercent;
    public int LevelingDamagePercent;
    public int Experience;
    public int Gold;
    public int HpMax;
    public int Hp;
    public int PaMax;
    public int PmMax;
    public List<WeaponType> FavWeapons;
    public List<Weapon> Weapons;
    public List<SkillType> SkillsTypes;
    public List<Skill> Skills;
    public int Score;
    public List<InventoryItem> Inventory;
    public int InventoryPlace;
    public int WeightLimit;

    public List<string> BodyParts;

    public int RaceWeaponDamagePercent; //To Add
    public int NotRaceWeaponDamagePercent; //To Substract

    public int GetCurrentInventoryWeight()
    {
        int weight = 0;
        if (Inventory == null || Inventory.Count == 0)
            return weight;
        foreach (var item in Inventory)
        {
            if (item == null)
                continue;
            weight += item.Weight;
        }
        return weight;
    }

    public int GetCurrentWeaponsWeight()
    {
        int weight = 0;
        weight += Weapons[0].Weight;
        weight += Weapons[1].Weight;
        return weight;
    }

    public int GetCurrentSkillsWeight()
    {
        int weight = 0;
        weight += Skills[0].Weight;
        weight += Skills[1].Weight;
        return weight;
    }

    public int GetTotalWeight()
    {
        int weight = 0;
        weight += GetCurrentInventoryWeight();
        weight += GetCurrentWeaponsWeight();
        weight += GetCurrentSkillsWeight();
        return weight;
    }

    public int TakeDamages(int damages)
    {
        foreach (var skill in Skills)
        {
            if (skill != null)
                damages = skill.OnTakeDamage(damages);
        }
        Hp -= damages;
        return damages;
    }

    public int GainHp(int amount)
    {
        int amountToAdd = amount;
        if (Hp + amountToAdd > HpMax)
            amountToAdd = HpMax - Hp;
        Hp += amountToAdd;
        return amountToAdd;
    }

    public int GainXp(int amount)
    {
        int amountToAdd = amount;
        var needed = Helper.XpNeedForLevel(Level);
        if (Experience + amountToAdd > needed)
        {
            int reserve = Experience + amountToAdd - needed;
            ++Level;
            Experience = reserve;
        }
        else
        {
            Experience += amountToAdd;
        }
        return amountToAdd;
    }
}
