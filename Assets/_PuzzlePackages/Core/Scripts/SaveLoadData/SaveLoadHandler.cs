using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGamePro;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class SaveLoadHandler
{
    private const string RANDOM_KEY_NEW = "LoKhLSGWee{0}_20240228";
    private const string FORMAT_TWO_VALUE = "{0}{1}";
    private const string FORMAT_THREE_VALUE = "{0}{1}{2}";
    private static Dictionary<string, string> cache = new Dictionary<string, string>();
    private static Dictionary<string, SaveGameSettings> cacheCode = new Dictionary<string, SaveGameSettings>();

    public static void Save<T>(string key, T obj)
    {
        try
        {
            if (AccountManager.instance.Code > 0)
            {
                SaveGame.Save(GetEncryptKeyNew(key), obj, GetEncryptSettingNew());
            }
            else
            {
                SaveGame.Save(MD5(key), obj);
            }

        }
        catch (Exception ex)
        {
            string logger = string.Format("{0} - {1}", key, ex);

            LogUtils.LogError(logger);
        }
    }

    public static T Load<T>(string key, T defaultValue = default)
    {
        T obj = defaultValue;
        try
        {
            if (AccountManager.instance.Code > 0)
            {
                if (SaveGame.Exists(GetEncryptKeyNew(key), GetEncryptSettingNew()))
                {
                    obj = SaveGame.Load<T>(GetEncryptKeyNew(key), defaultValue, GetEncryptSettingNew());

                    if (SaveGame.Exists(MD5(key)))
                    {
                        SaveGame.Delete(MD5(key));
                    }
                }
                else
                {
                    obj = SaveGame.Load<T>(MD5(key), defaultValue);
                    Save<T>(key, obj);
                }
            }
            else
            {
                obj = SaveGame.Load<T>(MD5(key), defaultValue);
            }
        }
        catch (Exception ex)
        {
            string logger = string.Format("{0} - {1}", key, ex);
            Debug.LogError("Load - " + logger);
        }
        return obj;
    }

    public static bool Exist(string key)
    {
        bool existEncryptNew = SaveGame.Exists(GetEncryptKeyNew(key), GetEncryptSettingNew());
        bool exist = SaveGame.Exists(MD5(key));

        bool result = exist || existEncryptNew;
        return result;
    }

    public static void DeleteKey(string key)
    {
        if (SaveGame.Exists(GetEncryptKeyNew(key), GetEncryptSettingNew()))
        {
            SaveGame.Delete(GetEncryptKeyNew(key));
        }
        if (SaveGame.Exists(MD5(key)))
        {
            SaveGame.Delete(MD5(key));
        }

    }

    public static string GetEncryptKeyNew(string key)
    {
        string cacheKey = string.Format(FORMAT_THREE_VALUE, key, AccountManager.instance.Code, RANDOM_KEY_NEW);
        if (cache.ContainsKey(cacheKey))
        {
            return cache[cacheKey];
        }
        else
        {
            string encryptKey = MD5(string.Format(FORMAT_TWO_VALUE, key, string.Format(RANDOM_KEY_NEW, AccountManager.instance.Code)));
            cache.Add(key + AccountManager.instance.Code + RANDOM_KEY_NEW, encryptKey);
            return encryptKey;
        }
    }

    public static SaveGameSettings GetEncryptSettingNew()
    {
        string cacheKey = string.Format(FORMAT_TWO_VALUE, AccountManager.instance.Code, RANDOM_KEY_NEW);
        if (!cacheCode.ContainsKey(cacheKey))
        {
            SaveGameSettings newSetting = new SaveGameSettings();
            newSetting.Encrypt = true;
            newSetting.EncryptionPassword = MD5(string.Format(RANDOM_KEY_NEW, AccountManager.instance.Code));
            cacheCode.Add(cacheKey, newSetting);
        }
        return cacheCode[cacheKey];
    }


    public static FileInfo[] GetFiles()
    {
        return SaveGame.GetFiles();
    }

    private static Dictionary<string, string> cacheMD5 = new Dictionary<string, string>();
    public static string MD5(string pass)
    {
        if (cacheMD5.ContainsKey(pass))
            return cacheMD5[pass];
        MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(pass);
        bs = x.ComputeHash(bs);
        StringBuilder s = new StringBuilder();
        foreach (byte b in bs)
        {
            s.Append(b.ToString("x2").ToLower());
        }

        cacheMD5[pass] = s.ToString();
        return cacheMD5[pass];
    }

    #region NOT_ENCRYPT_METHOD

    static SaveGameSettings eSetting = new SaveGameSettings();
    static SaveLoadHandler()
    {
        eSetting.Encrypt = true;
        eSetting.EncryptionPassword = "FALCON2024";
    }
    public static void NormalSave<T>(string key, T obj)
    {
        try
        {
            SaveGame.Save<T>(MD5(key), obj, eSetting);
        }
        catch (Exception ex)
        {
            string logger = string.Format("{0} - {1}", key, ex);

            LogUtils.LogError("save - " + logger);
        }
    }

    public static T NormalLoad<T>(string key, T defaultValue = default)
    {
        T obj = defaultValue;

        if (SaveGame.Exists(MD5(key), eSetting))
        {
            obj = SaveGame.Load<T>(MD5(key), defaultValue, eSetting);
        }
        return obj;
    }
    public static bool NormalExist(string key)
    {
        bool result = SaveGame.Exists(MD5(key), eSetting);
        return result;
    }
    #endregion

}

