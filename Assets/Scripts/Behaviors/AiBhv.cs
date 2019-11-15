using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBhv : MonoBehaviour
{
    private CharacterBhv _characterBhv;
    private GridBhv _gridBhv;

    private int AttackWeight;
    private int DefenseWeight;
    private int GetCloseWeight;
    private int GetFarWeight;
    private int Weapon0Weight;
    private int Weapon1Weight;
    private int Skill0Weight;
    private int Skill1Weight;
    

    public void SetPrivates()
    {
        _characterBhv = GetComponent<CharacterBhv>();
        _gridBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GridBhv>();
    }

    private void ResetWeight()
    {
        AttackWeight = 0;
        DefenseWeight = 0;
        GetCloseWeight = 0;
        GetFarWeight = 0;
        Weapon0Weight = 0;
        Weapon1Weight = 0;
        Skill0Weight = 0;
        Skill1Weight = 0;
    }

    public void StartReflexion()
    {
        Reflexion();
    }
    private void Reflexion() //What do I do
    {
        ResetWeight();
        SetAttackWeight();
    }

    private void SetAttackWeight()
    {
        int canI = CanIWeaponThePlayer();
        int shouldI = ShouldIWeaponThePlayer();
    }

    private int CanIWeaponThePlayer()
    {
        if (_characterBhv.Pa >= _characterBhv.Character.Weapons[0].PaNeeded &&
            _gridBhv.IsOpponentInWeaponRangeAndZone(_characterBhv, 0, _characterBhv.OpponentBhvs))
            Weapon0Weight += 10;
        if (_characterBhv.Pa >= _characterBhv.Character.Weapons[1].PaNeeded &&
            _gridBhv.IsOpponentInWeaponRangeAndZone(_characterBhv, 1, _characterBhv.OpponentBhvs))
            Weapon1Weight += 10;
        return Weapon0Weight + Weapon1Weight;
    }

    private int ShouldIWeaponThePlayer()
    {
        return 0;
    }
}

/*


    What do I do ?
        Should I Skill ?
            Is my skill offensive ?
                Am I in the right position ?
            Is my skill defensive ?
            Is my skill movement ?
            Is my skill control ?
            Is my skill buff ?
            Is my skill debuff ?
        Should I attack ?
            Do I have enough Pa ?
            Do I reach the player ?
        Should I Get Close ?
        Should I Get Far ?


    */
