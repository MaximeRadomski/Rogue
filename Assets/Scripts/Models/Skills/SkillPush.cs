using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPush : Skill
{
    public SkillPush()
    {
        Name = SkillsData.NormalSkillsNames[1];
        Type = SkillType.NotRatial;
        Nature = SkillNature.Offensive;
        Effect = SkillEffect.None;
        Rarity = Rarity.Normal;
        CooldownType = CooldownType.Normal;
        CooldownMax = 2;
        PaNeeded = 2;
        MinRange = 1;
        MaxRange = 1;
        RangeType = RangeType.Normal;
        RangePositions = new List<int> { 0,1, 1,0, 0,-1, -1,0 };
        IconId = 11;
        BasePrice = 100;

        Description = "Push your opponent of one cell";
    }

    private CharacterBhv _pushedOpponentBhv;

    public override void Activate(int x, int y)
    {
        base.Activate(x, y);
        CharacterBhv.StartCoroutine(Helper.ExecuteAfterDelay(PlayerPrefsHelper.GetSpeed(), () =>
        {
            var result = Push(GridBhv.IsOpponentOnCell(x, y));
            if (result == false)
                AfterPush();
            return true;
        }));
    }

    public bool Push(CharacterBhv pushedOpponentBhv)
    {
        _pushedOpponentBhv = pushedOpponentBhv;
        if (_pushedOpponentBhv == null)
            return false;
        int x = _pushedOpponentBhv.X - CharacterBhv.X;
        int y = _pushedOpponentBhv.Y - CharacterBhv.Y;
        if (!Helper.IsPosValid(_pushedOpponentBhv.X + x, _pushedOpponentBhv.Y + y)
            || GridBhv.Cells[_pushedOpponentBhv.X + x, _pushedOpponentBhv.Y + y].GetComponent<CellBhv>().Type != CellType.On
            || GridBhv.IsOpponentOnCell(_pushedOpponentBhv.X + x, _pushedOpponentBhv.X + x, true))
        {
            if ((Helper.IsPosValid(_pushedOpponentBhv.X + x, _pushedOpponentBhv.Y + y)
                && GridBhv.Cells[_pushedOpponentBhv.X + x, _pushedOpponentBhv.Y + y].GetComponent<CellBhv>().Type == CellType.Off)
                || GridBhv.IsOpponentOnCell(_pushedOpponentBhv.X + x, _pushedOpponentBhv.X + x, true))
            {
                var floatAmount = 30.0f * CharacterBhv.Character.GetDamageMultiplier();
                pushedOpponentBhv.TakeDamages(new Damage((int)floatAmount));
            }
            else if (!Helper.IsPosValid(_pushedOpponentBhv.X + x, _pushedOpponentBhv.Y + y)
                ||GridBhv.Cells[_pushedOpponentBhv.X + x, _pushedOpponentBhv.Y + y].GetComponent<CellBhv>().Type == CellType.Impracticable)
                pushedOpponentBhv.LosePm(1);
            return false;
        }
        _pushedOpponentBhv.AfterMouvementDelegate = AfterPush;
        _pushedOpponentBhv.MoveToPosition(_pushedOpponentBhv.X + x, _pushedOpponentBhv.Y + y, false);
        return true;
    }

    private void AfterPush()
    {
        if (_pushedOpponentBhv != null)
            _pushedOpponentBhv.AfterMouvementDelegate = null;
        AfterActivation();
    }
}
