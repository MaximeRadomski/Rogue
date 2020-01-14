using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Helper
{
    public static int CharacterAfterString(string str, string subStr)
    {
        return str.IndexOf(subStr) + subStr.Length;
    }

    public static Sprite GetSpriteFromSpriteSheet(string path)
    {
        var separatorId = path.IndexOf('_');
        var spriteSheetPath = path.Substring(0, separatorId);
        var spriteSheet = Resources.LoadAll<Sprite>(spriteSheetPath);
        var spriteId = int.Parse(path.Substring(separatorId + 1));
        if (spriteId >= spriteSheet.Length)
            return null;
        else
            return spriteSheet[spriteId];
    }
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
    {
        return source ?? Enumerable.Empty<T>();
    }

    public static bool IsPosValid(int x, int y)
    {
        if (x >= Constants.GridMax || y >= Constants.GridMax || x < 0 || y < 0)
            return false;
        return true;
    }

    public static int EnumCount<EnumType>()
    {
        return System.Enum.GetNames(typeof(EnumType)).Length;
    }

    public static int XpNeedForLevel(int level)
    {
        float floatXp = RacesData.LevelOneXpNeeded;
        for (int i = 1; i < level; ++i)
        {
            floatXp = floatXp + (floatXp * (0.5f - ((float)(i - 1) / 100)));
        }
        return RoundToNextDecade((int)floatXp);
    }

    public static int MaxHpFromLevelOne(int baseMaxHp, int level, int LevelingHealthPercent)
    {
        float floatMaxHp = baseMaxHp;
        for (int i = 1; i < level; ++i)
        {
            floatMaxHp = floatMaxHp * MultiplierFromPercent(1, LevelingHealthPercent);
        }
        return RoundToNextDecade((int)floatMaxHp);
    }

    public static float MultiplierFromPercent(float root, int percent)
    {
        return root + ((float)percent / 100.0f);
    }

    public static int RoundToNextDecade(int value)
    {
        return value + (value % 10 == 0 ? 0 : 10 - value % 10);
    }

    public static bool FloatEqualsPrecision(float float1, float float2, float precision)
    {
        return float1 >= float2 - precision && float1 <= float2 + precision;
    }

    public static bool VectorEqualsPrecision(Vector3 vector1, Vector3 vector2, float precision)
    {
        return (vector1.x >= vector2.x - precision && vector1.x <= vector2.x + precision)
            && (vector1.y >= vector2.y - precision && vector1.y <= vector2.y + precision)
            && (vector1.z >= vector2.z - precision && vector1.z <= vector2.z + precision);
    }

    public static bool ColorEqualsPrecision(Color color1, Color color2, float precision)
    {
        return (color1.r >= color2.r - precision && color1.r <= color2.r + precision)
            && (color1.g >= color2.g - precision && color1.g <= color2.g + precision)
            && (color1.b >= color2.b - precision && color1.b <= color2.b + precision)
            && (color1.a >= color2.a - precision && color1.a <= color2.a + precision);
    }

    public static RangePos DetermineRangePosFromRangeDirection(int x, int y, RangeDirection diretion)
    {
        if (x == 0 && y == 1)
        {
            switch (diretion)
            {
                case RangeDirection.Up:
                    return new RangePos(0, 1);
                case RangeDirection.Left:
                    return new RangePos(-1, 0);
                case RangeDirection.Right:
                    return new RangePos(1, 0);
                case RangeDirection.DiagonalLeft:
                    return new RangePos(-1, 1);
                case RangeDirection.DiagonalRight:
                    return new RangePos(1, 1);
            }
        }
        else if (x == 1 && y == 0)
        {
            switch (diretion)
            {
                case RangeDirection.Up:
                    return new RangePos(1, 0);
                case RangeDirection.Left:
                    return new RangePos(0, 1);
                case RangeDirection.Right:
                    return new RangePos(0, -1);
                case RangeDirection.DiagonalLeft:
                    return new RangePos(1, 1);
                case RangeDirection.DiagonalRight:
                    return new RangePos(1, -1);
            }
        }
        else if (x == 0 && y == -1)
        {
            switch (diretion)
            {
                case RangeDirection.Up:
                    return new RangePos(0, -1);
                case RangeDirection.Left:
                    return new RangePos(1, 0);
                case RangeDirection.Right:
                    return new RangePos(-1, 0);
                case RangeDirection.DiagonalLeft:
                    return new RangePos(1, -1);
                case RangeDirection.DiagonalRight:
                    return new RangePos(-1, -1);
            }
        }
        else if (x == -1 && y == 0)
        {
            switch (diretion)
            {
                case RangeDirection.Up:
                    return new RangePos(-1, 0);
                case RangeDirection.Left:
                    return new RangePos(0, -1);
                case RangeDirection.Right:
                    return new RangePos(0, 1);
                case RangeDirection.DiagonalLeft:
                    return new RangePos(-1, -1);
                case RangeDirection.DiagonalRight:
                    return new RangePos(-1, 1);
            }
        }
        return new RangePos(0, 0);
    }

    public static Color ColorFromTextType(int id)
    {
        TextType tmpType = (TextType)id;
        switch (tmpType)
        {
            case TextType.Normal:
                return Constants.ColorNormal;
            case TextType.Magical:
                return Constants.ColorMagic;
            case TextType.Rare:
                return Constants.ColorRare;
            case TextType.Legendary:
                return Constants.ColorLegendary;
            case TextType.Hp:
                return Constants.ColorLife;
            case TextType.Pa:
                return Constants.ColorPa;
            case TextType.Pm:
                return Constants.ColorPm;
        }
        return Constants.ColorTransparent;
    }

    public static void ReloadScene()
    {
        NavigationService.ReloadScene();
    }
}
