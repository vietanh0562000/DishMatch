using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BasePuzzle.PuzzlePackages.Core.UserData;
using UnityEngine;

public class AccountManager : NMSingleton<AccountManager>
{
    private const string key_sequence = "sequence";
    private int _sequence = 0;

    public int Sequence
    {
        get
        {
            if (_sequence <= 0 && SaveLoadHandler.NormalExist(key_sequence))
            {
                _sequence = SaveLoadHandler.NormalLoad<int>(key_sequence);
            }

            return _sequence;
        }
        set
        {
            _sequence = value;
            SaveSequence();
            AccountData.sequence = _sequence;
        }
    }

    private void SaveSequence()
    {
        SaveLoadHandler.NormalSave(key_sequence, _sequence);
    }

    private const string key_account_info = "account_info";
    private AccountInfo _accountInfo;

    public AccountInfo AccountInfo
    {
        get
        {
            if(_accountInfo==null)
            {
                if(SaveLoadHandler.NormalExist(key_account_info))
                {
                    _accountInfo=SaveLoadHandler.NormalLoad<AccountInfo>(key_account_info);
                    bool needSave=false;
                    
                        SaveAccountInfo();
                }
                else
                {
                    _accountInfo=new AccountInfo();
                    SaveAccountInfo();
                }
            }

            return _accountInfo;
        }
        set
        {
            // update data ở server thì không ghi đè fb, gg, apple id
            var fbId   =FB_id;
            var ggId   =Google_id;
            var appleId=Apple_id;
            _accountInfo=value;
            if(!string.IsNullOrEmpty(fbId))
            {
                _accountInfo.fb_id=fbId;
            }

            if(!string.IsNullOrEmpty(ggId))
            {
                _accountInfo.google_id=ggId;
            }

            if(!string.IsNullOrEmpty(appleId))
            {
                _accountInfo.apple_id=appleId;
            }

            SaveAccountInfo();
            AccountData.accountInfo=_accountInfo;
        }
    }

    public void SaveAccountInfo()
    {
        SaveLoadHandler.NormalSave(key_account_info, _accountInfo);
    }


    private AccountData _accountData;

    public AccountData AccountData
    {
        get
        {
            if (_accountData == null)
            {
                _accountData = new AccountData
                {
                    sequence = Sequence,
                    accountInfo = AccountInfo,
                    //thirdPartyData = GetThirdPartyData(),
                    //apkInfo = GetApkInfo(),
                    //deviceInfo = GetDeviceInfo()
                };
            }

            _accountData.sequence = Sequence;
            _accountData.accountInfo = AccountInfo;
            return _accountData;
        }
    }

    public void OnBindData(string bind_type, string bind_id, string bind_data)
    {
        AccountInfo.bind_data = bind_data;
        AccountInfo.bind_id = bind_id;
        AccountInfo.bind_data = bind_data;
        SaveAccountInfo();
    }

    public string Google_id
    {
        set
        {
            AccountInfo.google_id = value;
            SaveAccountInfo();
        }
        get { return AccountInfo.google_id; }
    }

    public string FB_id
    {
        set
        {
            AccountInfo.fb_id = value;
            SaveAccountInfo();
        }
        get { return AccountInfo.fb_id; }
    }

    public string Apple_id
    {
        set
        {
            AccountInfo.apple_id = value;
            SaveAccountInfo();
        }
        get { return AccountInfo.apple_id; }
    }

    public void OnFbLogin(string id)
    {
        if (FB_id != id)
        {
            FB_id = id;

            RestartNetManagerWhenInited();
        }
    }

    public void OnGoogleLogin(string id)
    {
        if (Google_id != id)
        {
            Google_id = id;

            RestartNetManagerWhenInited();
        }
    }

    public void OnAppleLogin(string id)
    {
        if (Apple_id != id)
        {
            Apple_id = id;

            RestartNetManagerWhenInited();
        }
    }

    private void RestartNetManagerWhenInited()
    {
        if (UserDataManager.LoginSuccess)
        {
            //UserDataManager.RestartGameNow();
        }
    }

    public bool DataBinded
    {
        private set { }
        get
        {
            if (!AccountInfo.apple_id.Equals("") || !AccountInfo.fb_id.Equals("") ||
                !AccountInfo.google_id.Equals(""))
                return true;
            return false;
        }
    }

    public int Code
    {
        get { return AccountInfo.code; }
    }

    public string CodeString
    {
        get => Code.ToString();
    }

    private const string key_hack_suspicion = "check_hack";
    private bool _hackSuspicion = false;

    public bool HackSuspicion
    {
        get
        {
            if (SaveLoadHandler.NormalExist(key_hack_suspicion))
            {
                _hackSuspicion = SaveLoadHandler.NormalLoad<bool>(key_hack_suspicion);
            }

            return _hackSuspicion;
        }
        set
        {
            _hackSuspicion = value;
            SaveLoadHandler.NormalSave(key_hack_suspicion, _hackSuspicion);
        }
    }

    private string _appsflyerId = null;

    public string AppsflyerId
    {
        get
        {
            if (_appsflyerId == null)
            {
              //  _appsflyerId = AppsFlyer.getAppsFlyerId();
            }

            return _appsflyerId;
        }
    }

 //   #region Init

    protected override void Init()
    {
        if (SaveLoadHandler.NormalExist(key_firebase))
        {
            _firebaseData = SaveLoadHandler.NormalLoad<FirebaseData>(key_firebase);
        }
        else
        {
            _firebaseData = new FirebaseData();
        }

        CheckToUpdateFirebaseData();

        if (SaveLoadHandler.NormalExist(key_appsflyer))
        {
            _appsFlyerData = SaveLoadHandler.NormalLoad<AppsFlyerData>(key_appsflyer);
        }
        else
        {
            _appsFlyerData = new AppsFlyerData();
        }

        //CheckToUpdateAppsflyer();
    }

    private const string key_firebase = "firebase_data";
    private FirebaseData _firebaseData;

    private void SaveFirebasData()
    {
        SaveLoadHandler.NormalSave(key_firebase, _firebaseData);
    }

    private void CheckToUpdateFirebaseData()
    {
        // void CheckAndUpdate()
        // {
        //     if(!FirebaseInit.firebaseToken.Equals(_firebaseData.token))
        //     {
        //         _firebaseData.token=FirebaseInit.firebaseToken;
        //         SaveFirebasData();
        //     }
        // }
        //
        // if(!string.IsNullOrEmpty(FirebaseInit.firebaseToken))
        // {
        //     CheckAndUpdate();
        // }
        // else
        // {
        //     FirebaseInit.onGetFirebaseToken+=() => { CheckAndUpdate(); };
        // }
    }

    private const string key_appsflyer = "appsflyer_data";
    private AppsFlyerData _appsFlyerData;

    private void SaveAppsflyer()
    {
        SaveLoadHandler.NormalSave(key_appsflyer, _appsFlyerData);
    }

    // private void CheckToUpdateAppsflyer()
    // {
    //     void CheckAndUpdate()
    //     {
    //         bool needSave=false;
    //
    //         bool CheckUpdate(ref string a,string b)
    //         {
    //             if(string.IsNullOrEmpty(b))
    //             {
    //                 return false;
    //             }
    //
    //             if(b.Equals(b))
    //             {
    //                 return false;
    //             }
    //
    //             a=b;
    //             return false;
    //         }

            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerID, FalconAppsFlyer.AppsflyerID);
            //     needSave = needSave ||
            //                CheckUpdate(ref _appsFlyerData.appsflyerAdgroupID, FalconAppsFlyer.AppsflyerAdgroupID);
            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerOrigCost, FalconAppsFlyer.AppsflyerOrigCost);
            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerAfCostCurrency,
            //         FalconAppsFlyer.AppsflyerAfCostCurrency);
            //     needSave = needSave ||
            //                CheckUpdate(ref _appsFlyerData.appsflyerCampaignID, FalconAppsFlyer.AppsflyerCampaignID);
            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerAfCid, FalconAppsFlyer.AppsflyerAfCid);
            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerMediaSource,
            //         FalconAppsFlyer.AppsflyerMediaSource);
            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerAdvertisingID,
            //         FalconAppsFlyer.AppsflyerAdvertisingID);
            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerAfStatus, FalconAppsFlyer.AppsflyerAfStatus);
            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerCostCentsUsd,
            //         FalconAppsFlyer.AppsflyerCostCentsUsd);
            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerAfCostValue,
            //         FalconAppsFlyer.AppsflyerAfCostValue);
            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerAfCostModel,
            //         FalconAppsFlyer.AppsflyerAfCostModel);
            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerAfAD, FalconAppsFlyer.AppsflyerAfAD);
            //     needSave = needSave || CheckUpdate(ref _appsFlyerData.appsflyerAdgroup, FalconAppsFlyer.AppsflyerAdgroup);
            //
            //     if (_appsFlyerData.appsflyerIsRetargeting != FalconAppsFlyer.AppsflyerIsRetargeting)
            //     {
            //         _appsFlyerData.appsflyerIsRetargeting = FalconAppsFlyer.AppsflyerIsRetargeting;
            //         needSave = true;
            //     }
            //
            //     if (_appsFlyerData.appsflyerIsFirstLaunch != FalconAppsFlyer.AppsflyerIsFirstLaunch)
            //     {
            //         _appsFlyerData.appsflyerIsFirstLaunch = FalconAppsFlyer.AppsflyerIsFirstLaunch;
            //         needSave = true;
            //     }
            //
            //     if (needSave)
            //     {
            //         SaveAppsflyer();
            //     }
            // }
            //
            // if (FalconAppsFlyer.isGetData)
            // {
            //     CheckAndUpdate();
            // }
            // else
            // {
            //     FalconAppsFlyer.onGetData += () => { CheckAndUpdate(); };
            // }
            //}

//     private ThirdPartyData GetThirdPartyData()
//     {
//         ThirdPartyData thirdParty = new ThirdPartyData(_firebaseData, _appsFlyerData);
//
//         return thirdParty;
//     }
//
//     private string GetAdvertisingID()
//     {
//         return FalconAdvertisingId.falconAdvertisingId;
//     }
//
//     private DeviceInfo GetDeviceInfo()
//     {
//         DeviceInfo deviceInfo = new DeviceInfo();
// #if UNITY_EDITOR
//         deviceInfo.deviceModel = "editor";
//         deviceInfo.operatingSystem = "operatingSystem";
//         deviceInfo.systemLanguage = "VN";
//         deviceInfo.systemLanguageISO = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
//         deviceInfo.resolution = "1920x1080";
//         deviceInfo.deviceMemory = 1024;
//         deviceInfo.graphicsMemorySize = 2048;
//         deviceInfo.platform = "editor";
// #else
//             deviceInfo.deviceModel = SystemInfo.deviceModel;
//             deviceInfo.operatingSystem = SystemInfo.operatingSystem;
//             deviceInfo.systemLanguage = Application.systemLanguage.ToString();
//             deviceInfo.systemLanguageISO = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
//             deviceInfo.resolution =
//  Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height.ToString();
//             deviceInfo.deviceMemory = SystemInfo.systemMemorySize;
//             deviceInfo.graphicsMemorySize = SystemInfo.graphicsMemorySize;
//             deviceInfo.platform = Application.platform.ToString();
// #endif
//
// #if UNITY_ANDROID
//         deviceInfo.platform = "android";
// #elif UNITY_IOS
//             deviceInfo.platform = "ios";
// #endif
//
//         return deviceInfo;
//     }
//
//     public int GetVerInt()
//     {
//         string ver = Application.version;
//         ver = ver.Replace(".", "");
//
//         int verInt;
//         int.TryParse(ver, out verInt);
//         return verInt;
//     }
//
//     public ApkInfo GetApkInfo()
//     {
//         ApkInfo apkInfo = new ApkInfo();
//
// #if UNITY_ANDROID
//         try
//         {
//             var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//             var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
//             var applicationInfo = activity.Call<AndroidJavaObject>("getApplicationInfo");
//             var nativeLibPath = applicationInfo.Get<string>("nativeLibraryDir");
//             Debug.Log("[DebugNative] nativeLibPath:" + nativeLibPath);
//             var lastFolderName = Path.GetFileName(
//                 nativeLibPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
//
//             string[] fileList = Directory.GetFiles(nativeLibPath);
//             fileList = fileList.OrderBy(x => x).ToArray();
//             long totalFileSize = 0;
//             string result = lastFolderName;
//
//             Debug.LogError("Get Architecture: " + result);
//
//             var separator = Path.DirectorySeparatorChar + "";
//             for (int i = 0; i < fileList.Length; i++)
//             {
//                 var fileInfo = new System.IO.FileInfo(fileList[i]);
//                 totalFileSize += fileInfo.Length;
//                 result += CalculateMD5FromFile(fileList[i]);
//             }
//
//             fileList = fileList.Select(x => x.Replace(nativeLibPath, "").Replace(separator, "").Replace(".so", ""))
//                 .ToArray();
//             var comboString = string.Join(",", fileList);
//
//
//             apkInfo.app_version = Application.version;
//
//             apkInfo.number_lib_files = fileList.Length;
//             apkInfo.total_lib_file_size = totalFileSize;
//             apkInfo.lib_file_name_list = comboString;
//             apkInfo.lib_folder = lastFolderName;
//             apkInfo.lib_md5 = CalculateMD5(result + comboString + totalFileSize);
//             apkInfo.package_name = Application.identifier;
//             apkInfo.app_version_int = GetVerInt();
//
//
//             return apkInfo;
//         }
//         catch
//         {
//             apkInfo.app_version = Application.version;
//             apkInfo.package_name = Application.identifier;
//             apkInfo.app_version_int = GetVerInt();
//
//             return apkInfo;
//         }
// #elif UNITY_IOS
//             apkInfo.app_version = Application.version;
//             apkInfo.package_name = Application.identifier;
//             apkInfo.app_version_int = GetVerInt();
//             return apkInfo;
// #elif UNITY_EDITOR
//             apkInfo.app_version = Application.version;
//             apkInfo.package_name = "com.fc.black.hole.people.escape.puzzle";
//             apkInfo.app_version_int = GetVerInt();
//             return apkInfo;
// #endif
//     }
//
//     private string CalculateMD5(string s)
//     {
//         MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
//
//         byte[] bs = Encoding.UTF8.GetBytes(s);
//         bs = x.ComputeHash(bs);
//         StringBuilder result = new StringBuilder();
//         foreach (byte b in bs)
//         {
//             result.Append(b.ToString("x2").ToLower());
//         }
//
//         return result.ToString();
//     }
//
//     private string CalculateMD5FromFile(string filename)
//     {
//         using (var md5 = MD5.Create())
//         {
//             using (var stream = File.OpenRead(filename))
//             {
//                 return Encoding.Default.GetString(md5.ComputeHash(stream));
//             }
//         }
//     }
//
//     private string DeviceUniqueIdentifier()
//     {
//         {
//             var deviceId = "";
// #if UNITY_EDITOR
//             //for clear data
//             var dt1 = DateTime.Now;
//             var dt2 = new DateTime(2000, 1, 1);
//             TimeSpan ts = dt1.Subtract(dt2);
//             deviceId = SystemInfo.deviceUniqueIdentifier + "-editor" + ts.TotalSeconds;
//             // deviceId = SystemInfo.deviceUniqueIdentifier + "-editor";
//
//
// #elif UNITY_ANDROID
//                 try
//                 {
//                     AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//                     AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
//                     AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
//                     AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
//                     deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");
//                    
//
//                     if(string.IsNullOrEmpty(deviceId) || deviceId.ToLower().Equals("unknown") || deviceId.Length < 4)
//                     {
//                         AndroidJavaClass client =
//  new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
//                         AndroidJavaObject adInfo =
//  client.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", currentActivity);
//                         deviceId = adInfo.Call<string>("getId");
//                     }
//
//                     if (string.IsNullOrEmpty(deviceId) || deviceId.ToLower().Equals("unknown") || deviceId.Length < 4)
//                     {
//                         deviceId = CreateUniqueString();
//                     }
//
//                     if (string.IsNullOrEmpty(deviceId) || deviceId.ToLower().Equals("unknown") || deviceId.Length < 4)
//                     {
//                         deviceId = SystemInfo.deviceUniqueIdentifier;
//                     }
//
//                 }catch (Exception e)
//                 {
//                     if (string.IsNullOrEmpty(deviceId) || deviceId.ToLower().Equals("unknown") || deviceId.Length < 4)
//                     {
//                         deviceId = CreateUniqueString();
//                     }
//
//                     if (string.IsNullOrEmpty(deviceId) || deviceId.ToLower().Equals("unknown") || deviceId.Length < 4)
//                     {
//                         deviceId = SystemInfo.deviceUniqueIdentifier;
//                     }
//                 }
//                 
//
//             Debug.Log("deviceID___" + deviceId);
// #elif UNITY_WEBGL
//                 if (!PlayerPrefs.HasKey("UniqueIdentifier"))
//                     PlayerPrefs.SetString("UniqueIdentifier", System.Guid.NewGuid().ToString());
//                 deviceId = PlayerPrefs.GetString("UniqueIdentifier");
// #elif UNITY_IOS
//                 string key = "falcon_" + Application.identifier + "_uuid";
//                 deviceId = UUIDiOS.GetKeyChainValue(key);
//                 Debug.Log("Step1___" + deviceId);
//
//                 if (string.IsNullOrEmpty(deviceId) || deviceId.ToLower().Equals("unknown") || deviceId.Length < 4)
//                 {
//                     deviceId = Guid.NewGuid().ToString();
//                     UUIDiOS.SaveKeyChainValue(key, deviceId);
//                      Debug.Log("Step2___" + deviceId);
//                 }
//
//                 if (string.IsNullOrEmpty(deviceId) || deviceId.ToLower().Equals("unknown") || deviceId.Length < 4)
//                 {
//                     deviceId = UnityEngine.iOS.Device.vendorIdentifier;
//                      Debug.Log("Step3___" + deviceId);
//                 }
// #else
//                 deviceId = SystemInfo.deviceUniqueIdentifier;
// #endif
//
//             Debug.Log("device ID___" + deviceId);
//
//             return deviceId;
//         }
//     }
//
//     private string GenerateUserName()
//     {
//         const string characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
//         string str = "";
//
//         for (int i = 0; i < 7; i++)
//         {
//             int randomIndex = UnityEngine.Random.Range(0, characters.Length);
//             str += characters[randomIndex];
//         }
//
//         return str;
//     }
//
//     string CreateUniqueString()
//     {
//         // Lấy thông tin về thiết bị để tạo một chuỗi độc nhất
//         string deviceInfo = SystemInfo.deviceModel + SystemInfo.deviceType + SystemInfo.graphicsDeviceName +
//                             SystemInfo.graphicsDeviceType;
//
//         // Mã hóa chuỗi đó để tạo một chuỗi duy nhất
//         using (SHA1 sha1 = SHA1.Create())
//         {
//             byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(deviceInfo));
//             StringBuilder stringBuilder = new StringBuilder();
//             foreach (byte b in hashBytes)
//             {
//                 stringBuilder.Append(b.ToString("x2"));
//             }
//
//             return stringBuilder.ToString();
//         }
//     }
//
//     #endregion
}