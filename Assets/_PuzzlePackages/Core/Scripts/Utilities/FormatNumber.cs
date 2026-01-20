using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class FormatNumber
{
    public static string ToString(int number)
    {
        if(number < 9999)
        {
            return number.ToString();
        }

        // Tạo định dạng số với dấu cách làm ngăn cách hàng nghìn
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberGroupSeparator = " ";
        nfi.NumberDecimalDigits = 0; // Không hiển thị phần thập phân

        // Chuyển đổi số int thành string với định dạng đã chỉ định
        return number.ToString("N0", nfi);
    }

    public static string FormatKMBNumber(double num)
    {
        if (num >= 1_000_000_000)
        {
            return (num / 1_000_000_000D).ToString("0.##") + "b";
        }
        else if (num >= 1_000_000)
        {
            return (num / 1_000_000D).ToString("0.##") + "m";
        }
        else if (num >= 100_000)
        {
            return (num / 1_000D).ToString("0.#") + "k";
        }
        else
        {
            return ToString((int)num);
        }
    }

    public static string FormatKMBNumberNoSpace(double num)
    {
        if (num >= 1_000_000_000)
        {
            return (num / 1_000_000_000D).ToString("0.##") + "b";
        }
        else if (num >= 1_000_000)
        {
            return (num / 1_000_000D).ToString("0.##") + "m";
        }
        else if (num >= 100_000)
        {
            return (num / 1_000D).ToString("0.#") + "k";
        }
        else
        {
            return $"{num}";
        }
    }
}
