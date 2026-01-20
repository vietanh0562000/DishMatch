using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionUtils
{
    public static List<T> GetRandomElement<T>(List<T> list, int size)
    {
        int count = list.Count;
        if (size > count)
        {
            return list;
        }
        List<T> copy = new List<T>(list);
        ShuffleCurrentList(copy);
        copy.RemoveRange(size, count - size);
        return copy;
    }

    public static void Swap<T>(this List<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    public static IEnumerable<T> Shuffle<T>(IEnumerable<T> items)
    {
        var result = items.ToArray();
        for (int i = items.Count<T>(); i > 1; i--)
        {
            int j = RandomUtils.RangeInt(i);
            var t = result[j];
            result[j] = result[i - 1];
            result[i - 1] = t;
        }

        return result;
    }

    public static List<T> Shuffle<T>(List<T> list)
    {
        var r = new List<T>(list);
        int n = r.Count;
        while (n > 1)
        {
            n--;
            int k = RandomUtils.RangeInt(n + 1);
            T value = r[k];
            r[k] = r[n];
            r[n] = value;
        }

        return r;
    }

    public static void ShuffleCurrentList<T>( IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = RandomUtils.RangeInt(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T GetRandomElementInList<T>(IList<T> list, bool isRemove = false)
    {
        var t = list[RandomUtils.RangeInt(0, list.Count)];
        if (isRemove)
        {
            list.Remove(t);
        }
        return t;
    }

    public static T GetRandomElementInArray<T>(T[] array)
    {
        var t = array[RandomUtils.RangeInt(0, array.Length)];

        return t;
    }

    public static int GetTotalSumArray(int[] arrayFloat)
    {
        int r = 0;

        for (int i = 0; i < arrayFloat.Length; i++)
        {
            r += arrayFloat[i];
        }

        return r;
    }

    public static float GetTotalSumArray(this float[] arrayFloat)
    {
        float r = 0;

        for (int i = 0; i < arrayFloat.Length; i++)
        {
            r += arrayFloat[i];
        }

        return r;
    }

    public static float GetTotalSumArray(this List<float> listFloat)
    {
        float r = 0;

        for (int i = 0; i < listFloat.Count; i++)
        {
            r += listFloat[i];
        }

        return r;
    }

    public static int GetIndexByRatioInArray(float[] array)
    {
        float totalRatio = array.GetTotalSumArray();
        var r = RandomUtils.RangeFloat(0, totalRatio);
        for (int i = 0; i < array.Length; i++)
        {
            r -= array[i];
            if (r <= 0)
            {
                return i;
            }
        }

        return 0;
    }

    public static int GetIndexByRatioInArray(List<float> list)
    {
        float totalRatio = list.GetTotalSumArray();
        var r = RandomUtils.RangeFloat(0, totalRatio);
        for (int i = 0; i < list.Count; i++)
        {
            r -= list[i];
            if (r <= 0)
            {
                return i;
            }
        }

        return 0;
    }

    /// <summary>
    /// listIndexIgnore cần được sắp xếp tăng dần
    /// </summary>
    /// <param name="array"></param>
    /// <param name="listIndexIgnore"></param>
    /// <returns></returns>
    public static int GetIndexByRatioInArray(float[] array, List<int> listIndexIgnore)
    {
        if (listIndexIgnore == null || listIndexIgnore.Count == 0)
        {
            return GetIndexByRatioInArray(array);
        }

        if (array.Length == listIndexIgnore.Count) return -1;

        var listRatio = array.ToList();
        var listIndex = new List<int>();
        for (int i = array.Length - 1; i >= 0; i--)
        {
            if (listIndexIgnore.Contains(i))
            {
                listRatio.RemoveAt(i);
            }
            else
            {
                listIndex.Insert(0, i);
            }
        }

        int index = GetIndexByRatioInArray(listRatio);

        return listIndex[index];
    }

    public static List<int> GetListRandomKWithRangeN(int k, int n)
    {
        List<int> results = new List<int>();
        for (int i = 0; i < n; i++) results.Add(i);
        results = Shuffle(results).ToList();
        results = results.Take<int>(k).ToList();
        return results;
    }

    public static List<int> GetListRandomKWithRange(int num, int min, int max)
    {
        List<int> results = new List<int>();
        for (int i = min; i <= max; i++) results.Add(i);
        results = Shuffle(results).ToList();
        results = results.Take<int>(num).ToList();
        return results;
    }

    public static List<T> GetListEnum<T>() where T : Enum
    {
        var t = new List<T>();
        foreach (T item in (T[])Enum.GetValues(typeof(T)))
        {
            t.Add(item);
        }
        return t;
    }

    public static int GetNumElementEnum<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Length;
    }

}
