using System;
using System.Collections.Generic;
#if EXIST_FB
using Facebook.Unity;
#endif
using UnityEngine;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
using UnityEngine.iOS;
#endif

public class FacebookController : PersistentSingleton<FacebookController>, IBindData
{
    private static readonly string MessengerUserId = "408808638980275";
    
    public static void OpenFacebookMessenger()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // Attempt to open in Messenger app on iOS.
            Application.OpenURL("fb-messenger://user-thread/" + MessengerUserId);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            // Attempt to open in Messenger app on Android.
            Application.OpenURL("fb://messaging/" + MessengerUserId);
        }
        else
        {
            // If we're neither on iOS nor Android, open the web version of Messenger.
            Application.OpenURL("https://www.messenger.com/t/" + MessengerUserId);
        }
    }
    
#if EXIST_FB
    // Start is called before the first frame update
    void Start()
    {
        if (!FB.IsInitialized)
        {
            LogUtils.LogError("InitFB");
            // Initialize the Facebook SDK
            FB.Init(FBInitCallback, FBOnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
            OnInitSuccess();
        }
    }

    private void FBInitCallback()
    {
        LogUtils.LogError("FBInitCallback");
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...      
            OnInitSuccess();
        }
        else
        {
            LogUtils.LogError("Failed to Initialize the Facebook SDK");
        }
    }

    private void FBOnHideUnity(bool isGameShown)
    {
        LogUtils.LogError("FBOnHideUnity");
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1f;
        }
    }

    public const string QUERY_DB = "/me?fields=id,name,picture.width(256).height(256).type(normal),friends";

    private void OnInitSuccess()
    {
        LogUtils.LogError("OnInitSuccess + FB.IsLoggedIn : " + FB.IsLoggedIn);
        if (FB.IsLoggedIn)
        {
#if UNITY_ANDROID
            FB.API(QUERY_DB, HttpMethod.GET, result =>
            {
                FBUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<FBUser>(result.RawResult);
                if (user != null && !string.IsNullOrEmpty(user.picture.data.url))
                {
                    string s = user.picture.data.url;
                    if (UserInfoController.instance.UserInfo.avatar_url != s)
                    {
                        UserInfoController.instance.SetUrl(s);
                    }
                }
            });
#elif UNITY_IOS
            if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED)
            {
                FB.API(QUERY_DB, HttpMethod.GET, result =>
                {
                    FBUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<FBUser>(result.RawResult);
                    if (user != null && !string.IsNullOrEmpty(user.picture.data.url))
                    {
                        string s = user.picture.data.url;
                        if (UserInfoController.instance.UserInfo.avatar_url != s)
                        {
                            UserInfoController.instance.SetUrl(s);
                        }
                    }
                });
            }else
            {
                var profile = FB.Mobile.CurrentProfile();
                if (profile != null && !string.IsNullOrEmpty(profile.ImageURL))
                {
                    if (UserInfoController.instance.UserInfo.avatar_url != profile.ImageURL)
                    {
                        UserInfoController.instance.SetUrl(profile.ImageURL);
                    }
                }
                else
                {
                    LogUtils.LogError("Facebook profile is null");
                }
            }

#endif
        }
    }

#endif


    #region LogIn/LogOut

    private Action<DataBinding> _onSuccess;
    private Action _onFail;

    public void SignIn(Action<string, string, string> onSuccess, Action onFail)
    {
    }

    public void RequestSignIn(Action<DataBinding> onSuccess, Action onFail)
    {
#if EXIST_FB
        _onSuccess = onSuccess;
        _onFail = onFail;
        var perms = new List<string>() { "gaming_profile", "user_friends" };
#if UNITY_ANDROID
        FB.LogInWithReadPermissions(perms, AuthCallback);
#elif UNITY_IOS
        var isLimited =
 ATTrackingStatusBinding.GetAuthorizationTrackingStatus() != ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED;
        FB.Mobile.LoginWithTrackingPreference(isLimited ? LoginTracking.LIMITED : LoginTracking.ENABLED, perms, "nonce123", AuthCallback);
#endif
#endif
    }

    public void SignOut(Action onSuccess)
    {
#if EXIST_FB
        FB.LogOut();
#endif
        onSuccess?.Invoke();
    }

    #endregion

#if EXIST_FB
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            UpdateUserInfo();
        }
        else
        {
            LogUtils.Log("AuthCallback: " + "login failed!");
            LogUtils.Log(result.RawResult);
            _onFail?.Invoke();
        }
    }
#endif

    public void UpdateUserInfo()
    {
#if EXIST_FB
#if UNITY_ANDROID
        FB.API(QUERY_DB, HttpMethod.GET, result =>
        {
            FBUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<FBUser>(result.RawResult);
            if (user == null)
            {
                LogUtils.LogError("FBUser is null");
                return;
            }

            string url = user.picture.data.url;
            DataBinding dataBinding = new DataBinding()
            {
                name = user.name,
                id = user.id,
                ava_url = user.picture.data.url,
            };

            AddFriendByFacebook(user.friends.data);
            _onSuccess?.Invoke(dataBinding);
        });

#elif UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED)
        {
            FB.API(QUERY_DB, HttpMethod.GET, result =>
            {
                FBUser user = Newtonsoft.Json.JsonConvert.DeserializeObject<FBUser>(result.RawResult);
                if (user == null)
                {
                    LogUtils.LogError("Facebook profile is null");
                }
                else
                {
                    string url = user.picture.data.url;
                    DataBinding dataBinding = new DataBinding()
                    {
                        name = user.name,
                        id = user.id,
                        ava_url = user.picture.data.url,
                    };

                    AddFriendByFacebook(user.friends.data);
                    _onSuccess?.Invoke(dataBinding);
                }

            });
        }else
        {
            var profile = FB.Mobile.CurrentProfile();
            if (profile != null)
            {
                DataBinding dataBinding = new DataBinding()
                {
                    name = profile.Name,
                    id = profile.UserID,
                    ava_url = profile.ImageURL,
                };

                AddFriendByFacebook(profile.FriendIDs);
                _onSuccess?.Invoke(dataBinding);
            }
            else
            {
                LogUtils.LogError("Facebook profile is null");
            }
        }

#endif
#endif
    }

    public void AddFriendByFacebook(List<FBUser.Friends.Data> friends)
    {
        if (friends == null)
            return;
        int count = friends.Count;
        for (int i = 0; i < count; i++)
        {
            //FriendDataController.instance.AddFriendFacebook(friends[i].id);
        }
    }

    public void AddFriendByFacebook(string[] FriendIDs)
    {
        if (FriendIDs == null)
            return;
        int count = FriendIDs.Length;
        for (int i = 0; i < count; i++)
        {
            //FriendDataController.instance.AddFriendFacebook(FriendIDs[i]);
        }
    }


    public void FBLogOut()
    {
    }
}


public class FBUser
{
    public string id;
    public string name;
    public Picture picture = new Picture();
    public Friends friends = new Friends();

    public class Picture
    {
        public Data data = new Data();

        public class Data
        {
            public int height;
            public bool is_silhouette;
            public string url;
            public int width;
        }
    }

    public class Friends
    {
        public List<Data> data = new List<Data>();
        public Paging paging = new Paging();
        public Summary summary = new Summary();

        public class Data
        {
            public string name;
            public string id;
        }

        public class Paging
        {
            public Cursors cursors = new Cursors();

            public class Cursors
            {
                public string before;
                public string after;
            }
        }

        public class Summary
        {
            public int total_count;
        }
    }
}