using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
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

    public static RangePos DetermineRangePosFromRangeDirection(RangePos position, RangeDirection diretion)
    {
        //Up
        if (position.X == 0 && position.Y == 1)
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
        else if (position.X == 1 && position.Y == 0)
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
        else if (position.X == 0 && position.Y == -1)
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
        else if (position.X == -1 && position.Y == 0)
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
}
