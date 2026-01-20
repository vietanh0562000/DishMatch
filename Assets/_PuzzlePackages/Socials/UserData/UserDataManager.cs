using System.Collections;
using UnityEngine;
using CodeStage.AntiCheat.Storage;
using System;
using BasePuzzle.Core.Scripts.Repositories;


namespace BasePuzzle.PuzzlePackages.Core.UserData
{
    using BasePuzzle.Core.Scripts.Repositories;
    using PuzzleGames;

    public class UserDataManager : PersistentSingleton<UserDataManager>
    {
        public const   string ServerAccountIdKey="server_account_id";
        private static bool   loginSuccess;

        public static bool LoginSuccess { get { return loginSuccess; } }

        public static Action<bool> onLoginStateChange;

        public void SetLoginState(bool state)
        {
            loginSuccess=state;
            onLoginStateChange?.Invoke(loginSuccess);
        }

        //public const string keyDataAntiCheat = "keyDataAntiCheat";
        private bool cheated;

        protected override void Awake()
        {
            base.Awake();

            //NetMonobehavior.onUpdateSessionState += OnUpdateSessionState; QuanPH
        }

        private void OnUpdateSessionState(bool obj)
        {
            if(!obj)
            {
                SetLoginState(false);
            }
        }

        private void Start()
        {
            ObscuredFilePrefs.NotGenuineDataDetected       +=OnDataCheat;
            ObscuredFilePrefs.DataFromAnotherDeviceDetected+=OnLockCheat;

            if(AccountManager.instance.AccountInfo!=null)
            {
                StartCoroutine(WaitSetAccountID());
            }
        }

        IEnumerator WaitSetAccountID()
        {
            yield return new WaitUntil(() => AccountManager.instance.AccountInfo.code!=0);
            FDataPool.Instance.Save(ServerAccountIdKey,AccountManager.instance.AccountInfo.code.ToString());
        }

        private void OnDataCheat()
        {
            cheated=true;
            HackSuspicion();
        }

        private void OnLockCheat()
        {
            cheated=true;
            HackSuspicion();
        }

        private void HackSuspicion()
        {
            AccountManager.instance.HackSuspicion=true;
            // UpdateDataToServer();
        }

        public void StartSession() { StartCoroutine(IEWaitSessionActive()); }

        IEnumerator IEWaitSessionActive()
        {
            return null;
            // yield return new WaitUntil(() => NetMonobehavior.sessionActive);
            // AccountInfo userInfo = AccountManager.instance.AccountInfo;
            // new CsLogin(userInfo.code, userInfo.token, userInfo.device_id, AccountManager.instance.Sequence,
            //     AccountManager.instance.AccountData.apkInfo.app_version_int,
            //     AccountManager.instance.AccountData.deviceInfo.platform).Send();
        }


        // #region Data From Server

        // public void ScLoginResponse(ScLogin sc)
        // {
        //     if (sc.login_status == 1)
        //     {
        //         Debug.Log("ERROR____" + sc.error_message);
        //     }
        //     else if (sc.login_status == 0)
        //     {
        //         if (sc.update_status == 0)
        //         {
        //             // Server và client đã đồng bộ
        //             SetLoginState(true);
        //         }
        //         // Login success
        //         else if (sc.update_status == 1)
        //         {
        //             // sequence server nhỏ hơn local
        //             // cần gửi data cho server
        //             SetLoginState(true);
        //             //UpdateDataToServer();
        //         }
        //         else if (sc.update_status == 2)
        //         {
        //             // Sequence server lớn hơn local
        //             // Client update data từ server
        //             // Server tự trả về SCUpdateAllData
        //         }
        //         else if (sc.update_status == 3)
        //         {
        //             // new account
        //         }
        //     }
        // }

        public void UpdateAllDataFromServer(ClientData clientData)
        {
            Debug.LogError(clientData.sequence);
            Debug.LogError(clientData.accountInfo);
            Debug.LogError(clientData.gameInfo);
            AccountManager.instance.Sequence   =clientData.sequence;
            AccountManager.instance.AccountInfo=clientData.accountInfo;
            ServerClassConverter.GetUpdateDataFromServer(clientData.gameInfo);
            //RestartGameNow();
        }
    }
//
//         public void UpdateGameDataFromServer(ScUpdateGameData sc)
//         {
//             RestartGameNow();
//             AccountManager.instance.Sequence = sc.sequence;
//             ServerClassConverter.GetUpdateDataFromServer(sc.gameData);
//         }
//
//         public void InitToken(ScNewAccount sc)
//         {
//             AccountManager.instance.AccountInfo.code = sc.code;
//             AccountManager.instance.AccountInfo.token = sc.token;
//             AccountManager.instance.AccountInfo.country_code = sc.countryCode;
//             AccountManager.instance.SaveAccountInfo();
//             SetLoginState(true);
//             UpdateDataToServer();
//         }
//
//         public void BindDataFromServer(ClientData clientData)
//         {
//             UpdateAllDataFromServer(clientData);
//         }
//
//         /// <summary>
//         /// Data from server, update data from server to local 
//         /// </summary>
//         /// <param name="sc"></param>
//         public void BindReponse(SCBindDataRsp sc)
//         {
//             if (sc.bind_status.ToUpper().Equals("FIRST_TIME"))
//             {
//                 // clientData.accountInfo.bind_type = sc.bind_type;
//                 // clientData.accountInfo.bind_id = sc.bind_id;
//                 // clientData.accountInfo.bind_data = sc.bind_data;
//
//                 // if (!sc.bind_data.Equals(""))
//                 // {
//                 //     try
//                 //     {
//                 //         BindData bindData = JsonUtility.FromJson<BindData>(sc.bind_data);
//                 //         clientData.accountInfo.fb_id = bindData.fb_id;
//                 //         clientData.accountInfo.google_id = bindData.google_id;
//                 //         clientData.accountInfo.apple_id = bindData.apple_id;
//                 //         //clientData.userInfo.avatar_url = bindData.avatar_url;
//                 //     }
//                 //     catch
//                 //     {
//                 //     }
//                 // }
//
//                 Debug.Log("Bind______succes");
//                 // update user data
//                 // EventManager.Instance.OnBindDataSuccess?.Invoke();
//
//                 RestartGameNow();
//             }
//         }
//
//         #endregion
//
//
//         /// <summary>
//         /// Save data to ObscuredFilePrefs and PUSH to server. Setting IsIncreaseSequence to true will increment variable A, and false will leave variable A unchanged.
//         /// </summary>
//         /// <param name="isincreaseSequence"></param>
//         [Sirenix.OdinInspector.Button]
//         public static void UpdateDataToServer(bool isincreaseSequence = true)
//         {
//             if (isincreaseSequence)
//             {
//                 AccountManager.instance.Sequence++;
//             }
//
//             if (NetMonobehavior.sessionActive && loginSuccess)
//             {
//                 var clientData = new ClientData(AccountManager.instance.AccountData);
//                 clientData.sequence = AccountManager.instance.Sequence;
//                 clientData.gameInfo = ClientClassConverter.GetUserData();
//                 new CsUpdateData(clientData).Send();
//             }
//         }
//
//
//         /// <summary>
//         /// Call when change server
//         /// </summary>
//         public void ClearToken()
//         {
//             AccountManager.instance.AccountInfo.token = "";
//             AccountManager.instance.AccountInfo.code = 0;
//         }
//
//         public static void RestartGameNow()
//         {
//             NetMonobehavior.Instance.Reconnect();
//             LoadSceneManager.Instance.LoadScene("Home");
//         }
//
//         #region Login 3rd
//
//         public void BinDataWithGooglePlay()
//         {
// #if UNITY_ANDROID
//             // PlayGamesPlatform.Instance.Authenticate((success) =>
//             // {
//             //     if (success == SignInStatus.Success)
//             //     {
//             //         Debug.Log("Login with Google Play games successful.");
//
//             //         PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
//             //         {
//             //             Debug.Log("Authorization code: " + code);
//             //             // This token serves as an example to be used for SignInWithGooglePlayGames
//             //         });
//
//             //         if (NetMonobehavior.sessionActive)
//             //         {
//             //             BindData bindData = new BindData(google_id: PlayGamesPlatform.Instance.GetUserId(),
//             //                 avatar_url: PlayGamesPlatform.Instance.GetUserImageUrl());
//             //             string dataJson = JsonUtility.ToJson(bindData);
//             //             new CsBindData(BindId.gg_id.ToString(), BindType.google.ToString(), dataJson).Send();
//             //         }
//             //     }
//             //     else
//             //     {
//             //         Debug.Log("Login Unsuccessful");
//             //     }
//             // });
// #endif
//         }
//
//         public void LoginFb(string userId, string userName, string avatar_url)
//         {
//             if (NetMonobehavior.sessionActive)
//             {
//                 // clientData.userInfo.displayName = userName;
//                 // clientData.userInfo.avatar_url = avatar_url;
//                 AccountManager.instance.AccountInfo.fb_id = userId;
//                 UpdateDataToServer();
//
//                 BindData bindData = new BindData(fb_id: userId, avatar_url: avatar_url);
//                 string dataJson = JsonUtility.ToJson(bindData);
//                 new CsBindData(BindType.facebook.ToString(), userId, dataJson).Send();
//             }
//         }
//
//         #endregion
//     }
}

public enum BindId
{
    fb_id,
    gg_id,
    apple_id
}

public enum BindType
{
    facebook,
    google,
    apple
}