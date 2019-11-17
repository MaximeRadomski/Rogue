using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGrapplingHook : Skill
{
    public SkillGrapplingHook()
    {
        Name = RacesData.SkillsData.DwarfSkillsNames[1];
        Type = SkillType.Racial;
        Nature = SkillNature.MovementClose;
        Effect = SkillEffect.None;
        Race = CharacterRace.Dwarf;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 4;
        Cooldown = 0;
        PaNeeded = 2;
        MinRange = 1;
        MaxRange = 1;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,2, 0,3, 0,4, 0,5, 0,6,
                                         2,0, 3,0, 4,0, 5,0, 6,0,
                                         0,-2, 0,-3, 0,-4, 0,-5, 0,-6,
                                         -2,0, -3,0, -4,0, -5,0, -6,0 };
    }

    private CharacterBhv _grabbedOpponentBhv;

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        var result = Grap(GridBhv.IsOpponentOnCell(x, y));
        if (result == false)
            AfterGrap();
    }

    public bool Grap(CharacterBhv grabbedOpponentBhv)
    {
        _grabbedOpponentBhv = grabbedOpponentBhv;
        if (_grabbedOpponentBhv == null)
            return false;
        _grabbedOpponentBhv.AfterMouvementDelegate = AfterGrap;
        int x = CharacterBhv.X - _grabbedOpponentBhv.X;
        int y = CharacterBhv.Y - _grabbedOpponentBhv.Y;
        if (x != 0) x = x < 0 ? ++x : --x;
        if (y != 0) y = y < 0 ? ++y : --y;
        while (x != 0)
        {
            if (GridBhv.Cells[_grabbedOpponentBhv.X + x, _grabbedOpponentBhv.Y + y].GetComponent<CellBhv>().Type == CellType.On)
            {
                _grabbedOpponentBhv.MoveToPosition(_grabbedOpponentBhv.X + x, _grabbedOpponentBhv.Y + y, false);
                break;
            }
            x = x < 0 ? ++x : --x;
        }
        while (y != 0)
        {
            if (GridBhv.Cells[_grabbedOpponentBhv.X + x, _grabbedOpponentBhv.Y + y].GetComponent<CellBhv>().Type == CellType.On)
            {
                _grabbedOpponentBhv.MoveToPosition(_grabbedOpponentBhv.X + x, _grabbedOpponentBhv.Y + y, false);
                break;
            }
            y = y < 0 ? ++y : --y;
        }
        return true;
    }

    private void AfterGrap()
    {
        GridBhv.ShowPm(CharacterBhv, OpponentBhvs);
        _grabbedOpponentBhv.AfterMouvementDelegate = null;
    }
}
