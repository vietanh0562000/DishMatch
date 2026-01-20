using UnityEngine;

public static class RandomUtils
{
    public static int RangeInt(int max)
    {
        return Random.Range(0, max);
    }

    public static int RangeInt(int min, int max)
    {
        return Random.Range(min, max);
    }

    public static float RangeFloat(float max)
    {
        return Random.Range(0f, max);
    }

    public static float RangeFloat(float min, float max)
    {
        return Random.Range(min, max);
    }


    public static int RoundingValue(float value)
    {
        int r = (int)value;
        float a = value - r;

        if (RangeFloat(0f, 1f) < a)
        {
            r++;
        }
        return r;
    }
}