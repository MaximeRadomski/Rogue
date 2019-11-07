using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillGrapplingHook : Skill
{
    public SkillGrapplingHook()
    {
        Name = RacesData.SkillsData.DwarfSkillsNames[1];
        Type = SkillType.Racial;
        Race = CharacterRace.Dwarf;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 4;
        Cooldown = 0;
        PaNeeded = 2;
        MinRange = 1;
        MaxRange = 2;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,2, 0,3, 0,4, 0,5, 0,6,
                                         2,0, 3,0, 4,0, 5,0, 6,0,
                                         0,-2, 0,-3, 0,-4, 0,-5, 0,-6,
                                         -2,0, -3,0, -4,0, -5,0, -6,0 };
    }

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        Grap(GridBhv.IsOpponentOnCell(x, y));
        GridBhv.ShowPm(CharacterBhv, OpponentBhvs);
    }

    public void Grap(CharacterBhv grabbedOpponentBhv)
    {
        if (grabbedOpponentBhv == null)
            return;
        int x = CharacterBhv.X - grabbedOpponentBhv.X;
        int y = CharacterBhv.Y - grabbedOpponentBhv.Y;
        if (x != 0) x = x < 0 ? ++x : --x;
        if (y != 0) y = y < 0 ? ++y : --y;
        while (x != 0)
        {
            if (GridBhv.Cells[grabbedOpponentBhv.X + x, grabbedOpponentBhv.Y + y].GetComponent<CellBhv>().Type == CellType.On)
            {
                grabbedOpponentBhv.MoveToPosition(grabbedOpponentBhv.X + x, grabbedOpponentBhv.Y + y, false);
                break;
            }
            x = x < 0 ? ++x : --x;
        }
        while (y != 0)
        {
            if (GridBhv.Cells[grabbedOpponentBhv.X + x, grabbedOpponentBhv.Y + y].GetComponent<CellBhv>().Type == CellType.On)
            {
                grabbedOpponentBhv.MoveToPosition(grabbedOpponentBhv.X + x, grabbedOpponentBhv.Y + y, false);
                break;
            }
            y = y < 0 ? ++y : --y;
        }
    }
}
