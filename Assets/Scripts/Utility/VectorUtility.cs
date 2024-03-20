using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtility
{
    public static Vector2 GetUnidirectional(this Vector2 vector, float minThreshold = 0)
    {
        bool useXAxis = Mathf.Abs(vector.x) >= Mathf.Abs(vector.y);
        var valueToFormat = useXAxis ? vector.x : vector.y;

        var value = FormatValue(valueToFormat, minThreshold);

        if (useXAxis)
        {
            return new Vector2(value, 0);
        }
        else
        {
            return new Vector2(0, value);
        }
    }

    public static Vector2 GetUnidirectional01(this Vector2 vector, float minThreshold = 0)
    {
        Vector2 uni = vector.GetUnidirectional(minThreshold);
        if(uni.x != 0)
        {
            uni.x /= Mathf.Abs(uni.x);
        }

        if(uni.y != 0)
        {
            uni.y /= Mathf.Abs(uni.y);
        }
        return uni;
    }

    private static float FormatValue(float value, float threshold)
    {
        if (Mathf.Abs(value) > 1)
        {
            value = Mathf.Clamp(value, -1f, 1f);
        }

        if (Mathf.Abs(value) < threshold)
        {
            value = 0;
        }

        return value;
    }
}
