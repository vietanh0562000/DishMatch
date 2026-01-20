using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;

namespace BasePuzzle.PuzzlePackages.Core.UserData
{
    [Serializable]
    public class ClientData : AccountData
    {
        public GameInfo gameInfo;

        public ClientData()
        {
        }

        public ClientData(DeviceInfo deviceInfo, AccountInfo userInfo, GameInfo gameInfo, ApkInfo apkInfo,
            ThirdPartyData thirdPartyData)
        {
            this.deviceInfo = deviceInfo;
            this.accountInfo = userInfo;
            this.gameInfo = gameInfo;
            this.apkInfo = apkInfo;
            this.thirdPartyData = thirdPartyData;
        }


        public ClientData(AccountData accountData)
        {
            UpdateData(accountData);
        }

        public void UpdateData(AccountData accountData)
        {
            this.sequence = accountData.sequence;
            this.deviceInfo = accountData.deviceInfo;
            this.accountInfo = accountData.accountInfo;
            this.apkInfo = accountData.apkInfo;
            this.thirdPartyData = accountData.thirdPartyData;
        }
    }

    public class AccountData
    {
        public int sequence;
        public DeviceInfo deviceInfo;
        public AccountInfo accountInfo;
        public ApkInfo apkInfo;
        public ThirdPartyData thirdPartyData;
    }


    [Serializable]
    public class AccountInfo
    {
        public int code;
        public string token;
        public int accountId;
        public string device_id;
        public string fb_id;
        public string google_id;
        public string apple_id;
        public string advertising_id;
        public string bind_type;
        public string bind_id;
        public string bind_data;
        public string uuid;
        public string country;
        public string country_code;

        public AccountInfo()
        {
        }

        public AccountInfo(string device_id, string advertising_id)
        {
            this.device_id = device_id;
            this.advertising_id = advertising_id;

            code = 0;
            token = "";
            accountId = 0;
            fb_id = "";
            google_id = "";
            apple_id = "";

            bind_type = "";
            bind_id = "";
            bind_data = "";
            uuid = "";
        }
    }

    [Serializable]
    public class DeviceInfo
    {
        public string deviceModel;
        public string operatingSystem;
        public string systemLanguage;
        public string systemLanguageISO;
        public string resolution;
        public int deviceMemory;
        public int graphicsMemorySize;
        public string platform;

        public DeviceInfo()
        {
            deviceModel = "";
            operatingSystem = "";
            systemLanguage = "";
            systemLanguageISO = "";
            resolution = "";
            deviceMemory = 0;
            graphicsMemorySize = 0;
            platform = "";
        }
    }

    [Serializable]
    public class ApkInfo
    {
        public string app_version;
        public int app_version_int;
        public string install_vendor;
        public int number_lib_files;
        public long total_lib_file_size;
        public string lib_file_name_list;
        public string lib_folder;
        public string lib_md5;
        public string package_name;

        public ApkInfo()
        {
            app_version = "";
            app_version_int = 0;
            install_vendor = "";
            number_lib_files = 0;
            total_lib_file_size = 0;
            lib_file_name_list = "";
            lib_folder = "";
            lib_md5 = "";
            package_name = "";
        }
    }

    [Serializable]
    public class BindData
    {
        public string fb_id;
        public string google_id;
        public string apple_id;
        public string avatar_url;

        public BindData(string fb_id = "", string google_id = "", string apple_id = "", string avatar_url = "")
        {
            this.fb_id = fb_id;
            this.google_id = google_id;
            this.apple_id = apple_id;
            this.avatar_url = avatar_url;
        }
    }

    [Serializable]
    public class FirebaseData
    {
        public string token;

        public FirebaseData()
        {
        }

        public FirebaseData(string token)
        {
            this.token = token;
        }
    }

    [Serializable]
    public class AppsFlyerData
    {
        public string appsflyerID;
        public string appsflyerAdgroupID;
        public string appsflyerOrigCost;
        public string appsflyerAfCostCurrency;
        public bool appsflyerIsFirstLaunch;
        public string appsflyerCampaignID;
        public string appsflyerAfCid;
        public string appsflyerMediaSource;
        public string appsflyerAdvertisingID;
        public string appsflyerAfStatus;
        public string appsflyerCostCentsUsd;
        public string appsflyerAfCostValue;
        public string appsflyerAfCostModel;
        public string appsflyerAfAD;
        public bool appsflyerIsRetargeting;
        public string appsflyerAdgroup;

        public AppsFlyerData()
        {
            appsflyerID = "";
            appsflyerAdgroupID = "";
            appsflyerOrigCost = "";
            appsflyerAfCostCurrency = "";
            appsflyerIsFirstLaunch = false;
            appsflyerCampaignID = "";
            appsflyerAfCid = "";
            appsflyerMediaSource = "";
            appsflyerAdvertisingID = "";
            appsflyerAfStatus = "";
            appsflyerCostCentsUsd = "";
            appsflyerAfCostValue = "";
            appsflyerAfCostModel = "";
            appsflyerAfAD = "";
            appsflyerIsRetargeting = false;
            appsflyerAdgroup = "";
        }
    }

    [Serializable]
    public class ThirdPartyData
    {
        public FirebaseData firebase_data;
        public AppsFlyerData appsflyer_data;

        public ThirdPartyData(FirebaseData firebase_data, AppsFlyerData appsflyer_data)
        {
            this.firebase_data = firebase_data;
            this.appsflyer_data = appsflyer_data;
        }

        public ThirdPartyData()
        {
            firebase_data = new FirebaseData("None");
            appsflyer_data = new AppsFlyerData();
        }
    }
}