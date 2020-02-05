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

    public void RegenerationFromMinutes(int minutesPassed)
    {
        var test = (minutesPassed / Constants.RegenerationTimeLaps) * Constants.RegenerationHp;
        GainHp(test);
    }

    public int GainXp(int amount)
    {
        int amountToAdd = amount;
        var needed = Helper.XpNeedForLevel(Level);
        if (Experience + amountToAdd >= needed)
        {
            while (Experience + amountToAdd >= needed)
            {
                int reserve = Experience + amountToAdd - needed;
                LevelUp();
                needed = Helper.XpNeedForLevel(Level);
                if (reserve >= needed)
                {
                    Experience = 0;
                    amountToAdd = reserve;
                }
                else
                {
                    Experience = reserve;
                    amountToAdd = 0;
                }
            }
        }
        else
        {
            Experience += amountToAdd;
        }
        return amountToAdd;
    }

    private void LevelUp()
    {
        ++Level;
        Hp = (int)(Hp * Helper.MultiplierFromPercent(1, LevelingHealthPercent));
        HpMax = (int)(HpMax * Helper.MultiplierFromPercent(1, LevelingHealthPercent));
        if (HpMax - Hp > 0) //Bonus de vie par niveau
            GainHp((int)(HpMax * Helper.MultiplierFromPercent(0, LevelingHealthPercent)));

    }

    public float GetDamageMultiplier()
    {
        return Helper.MultiplierFromPercent(1, LevelingDamagePercent * (Level - 1));
    }
}
