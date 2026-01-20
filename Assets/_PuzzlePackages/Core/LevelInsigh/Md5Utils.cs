using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Security.Cryptography;
using System.Text;

public static class Md5Utils
{
    public static string GetMd5First5Char(string input)
    {
        try
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Mã hóa byte array thành Base64
                string base64 = Convert.ToBase64String(hashBytes);

                // Lấy 5 ký tự đầu tiên của Base64
                return base64.Substring(0, 5);
            }
        }
        catch (Exception)
        {
            return null;
        }
    }
}

