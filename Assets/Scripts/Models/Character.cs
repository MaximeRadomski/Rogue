using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public bool IsPlayer = false;
    public CharacterGender Gender;
    public string Name;
    public CharacterRace Race;
    public CharacterRace StrongAgainst;
    public MapType StrongIn;
    public Diet Diet;
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
    public int SleepHoursNeeded;
    public int SleepRestorationPercent;

    public List<string> BodyParts;

    public int RaceWeaponDamagePercent; //To Add
    public int NotRaceWeaponDamagePercent; //To Substract

    public GameObject Frame;
    private OrbBhv _orbHp;
    private ResourceBarBhv _healthBar;
    private ResourceBarBhv _xpBar;
    private TMPro.TextMeshPro _level;
    private TMPro.TextMeshPro _xp;
    private TMPro.TextMeshPro _gold;
    private Instantiator _instantiator;
    private Vector2 _ressourcePopPosition;

    private void GetPrivates()
    {
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
        if (IsPlayer)
            _orbHp = GameObject.Find("Hp")?.GetComponent<OrbBhv>();
        else
            _healthBar = GameObject.Find("HealthBar")?.GetComponent<ResourceBarBhv>();
        _xpBar = GameObject.Find("XpBar")?.GetComponent<ResourceBarBhv>();
        _level = GameObject.Find("LevelText")?.GetComponent<TMPro.TextMeshPro>();
        _xp = GameObject.Find("Xp")?.GetComponent<TMPro.TextMeshPro>();
        _gold = GameObject.Find("Gold")?.GetComponent<TMPro.TextMeshPro>();
        _ressourcePopPosition = new Vector2(0.0f, -2.6f);
    }

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
        if (IsPlayer)
        {
            if (_orbHp == null)
                GetPrivates();
            _orbHp?.UpdateContent(Hp, HpMax, _instantiator, TextType.Hp, -damages, Direction.Down);
        }
        else
        {
            if (_healthBar == null)
                GetPrivates();
            _healthBar.UpdateContent(Hp, HpMax, Name, Frame, Direction.Down);
        }
        return damages;
    }

    public int GainHp(int amount)
    {
        int amountToAdd = amount;
        if (Hp + amountToAdd > HpMax)
            amountToAdd = HpMax - Hp;
        Hp += amountToAdd;
        if (IsPlayer)
        {
            if (_orbHp == null)
                GetPrivates();
            if (amountToAdd > 0)
                _orbHp?.UpdateContent(Hp, HpMax, _instantiator, TextType.Hp, amountToAdd, Direction.Up);
        }
        else
        {
            if (_healthBar == null)
                GetPrivates();
            _healthBar.UpdateContent(Hp, HpMax, Name, Frame, Direction.Up);
        }
        return amountToAdd;
    }

    public int LooseGold(int amount)
    {
        if (Gold - amount < 0)
            amount = Gold;
        Gold -= amount;
        if (_instantiator == null) GetPrivates();
        _instantiator.PopText("-" + amount + " " + Constants.UnitGold, _gold?.transform.position ?? _ressourcePopPosition, TextType.Gold, TextThickness.Long);
        if (_gold != null) _gold.text = Gold.ToString() + " " + Constants.UnitGold;
        return amount;
    }

    public int GainGold(int amount)
    {
        int amountToAdd = amount;
        if (Gold + amountToAdd > Constants.MaxGold)
            amountToAdd = Constants.MaxGold - Gold;
        Gold += amountToAdd;
        if (_instantiator == null) GetPrivates();
        _instantiator.PopText("+" + amountToAdd + " " + Constants.UnitGold, _gold?.transform.position ?? _ressourcePopPosition, TextType.Gold, TextThickness.Long);
        if (_gold != null) _gold.text = Gold.ToString() + " " + Constants.UnitGold;
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
        if (_instantiator == null) GetPrivates();
        _instantiator.PopText("+" + amountToAdd + " " + Constants.UnitXp, _xp?.transform.position ?? _ressourcePopPosition, TextType.Xp, TextThickness.Long);
        if (_xp != null) _xp.text = Experience.ToString() + "/" + Helper.XpNeedForLevel(Level) + " " + Constants.UnitXp;
        var needed = Helper.XpNeedForLevel(Level);
        _xpBar?.UpdateContent(Experience + amountToAdd, needed, Name, null, Direction.Up);
        if (Experience + amountToAdd >= needed)
        {
            Constants.InputLocked = true;
            _instantiator.StartCoroutine(Helper.ExecuteAfterDelay(1.0f, () =>
            {
                _xpBar?.UpdateContent(0, 1, Name, null, Direction.None);
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
                if (_instantiator == null) GetPrivates();
                _instantiator.PopText("LEVEL " + Level, _level?.transform.position ?? _ressourcePopPosition, TextType.Magical);
                if (_level != null) _level.text = Level.ToString();
                _xpBar?.UpdateContent(Experience, needed, Name, null, Direction.Up);
                Constants.InputLocked = false;
                return true;
            }));
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

    public void AddToInventory(List<InventoryItem> items, Func<bool, object> afterInventoryWork)
    {
        int invId = Inventory.Count;
        List<InventoryItem> discardedItems = new List<InventoryItem>();
        for (int i = 0; i < items.Count; ++i)
        {
            if (invId < InventoryPlace)
            {
                Inventory.Add(items[i]);
            }
            else
            {
                discardedItems.Add(items[i]);
            }
            ++invId;
        }
        if (discardedItems.Count > 0)
        {
            if (_instantiator == null) GetPrivates();
            LoopDiscarded(_instantiator, discardedItems, afterInventoryWork);
        }
        else
        {
            afterInventoryWork(true);
        }
    }

    private void LoopDiscarded(Instantiator instantiator, List<InventoryItem> discardedItems, System.Func<bool, object> afterInventoryWork)
    {
        if (discardedItems.Count == 0)
        {
            afterInventoryWork(true);
            return;
        }
            
        instantiator.NewPopupYesNo("Full Inventory",
                "Your inventory is full. Do you wish to manage your items in order to make some place for:\n" + discardedItems[0].GetNameWithColor() + "?",
                "No", "Yes", OnAcceptManage);

        object OnAcceptManage(bool result)
        {
            if (result)
            {
                instantiator.NewPopupInventory(this, null, OnManage);
                return true;
            }
            discardedItems.RemoveAt(0);
            LoopDiscarded(instantiator, discardedItems, afterInventoryWork);
            return false;
        }

        object OnManage(bool result)
        {
            //if (result)
            //{
            //    Inventory.Add(discardedItems[id]);
            //    LoopDiscarded(instantiator, discardedItems, id + 1, afterInventoryWork);
            //    return true;
            //}
            //LoopDiscarded(instantiator, discardedItems, id + 1, afterInventoryWork);
            //return false;
            AddToInventory(discardedItems, afterInventoryWork);
            return result;
        }
    }
}
