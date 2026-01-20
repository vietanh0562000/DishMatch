using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindDataController : NMSingleton<BindDataController>
{
    private Dictionary<string, IBindData> _dicBindDataByName;
    private Dictionary<BindDataType, IBindData> _dicBindData;

    public Action<BindDataType> onSignIn;
    public Action<BindDataType> onSignInFail;
    public Action onSignOut;

    protected override void Init()
    {
        _dicBindData = new Dictionary<BindDataType, IBindData>
        {
            { BindDataType.Facebook, FacebookController.Instance }
        };
        _dicBindDataByName = new Dictionary<string, IBindData>()
        {
            { BindDataType.Facebook.ToString().Trim(), FacebookController.Instance}
        };
    }

    public void SignIn(BindDataType bindDataType)
    {
        // AdsManager.isPauseByWebView = true;
        _dicBindData[bindDataType].RequestSignIn((data) =>
        {
            LoadTextureUtils.ReleasCacheAvatar();
            if (!string.IsNullOrEmpty(data.name))
            {
                UserInfoController.instance.SetName(data.name);
            }
            if (!string.IsNullOrEmpty(data.ava_url))
            {
                UserInfoController.instance.SetAvatar(-1, data.ava_url);
            }
            switch (bindDataType)
            {
                case BindDataType.Facebook:
                    AccountManager.instance.OnFbLogin(data.id);
                    break;
                case BindDataType.Google:
                    AccountManager.instance.OnGoogleLogin(data.id);
                    break;
                case BindDataType.Apple:
                    AccountManager.instance.OnAppleLogin(data.id);
                    break;
            }
            onSignIn?.Invoke(bindDataType);
        }, () => onSignInFail?.Invoke(bindDataType));
    }


    public void CancelBindData()
    {
        var bindDataState = GetBindDataState();
        if (bindDataState == BindDataState.None)
        {
            LogUtils.LogError("BindDataState is None");
            return;
        }
        int type = -1;
        string id = string.Empty;
        IBindData bindData = null;

        switch (bindDataState)
        {
            case BindDataState.Facebook:
                type = 0;
                id = AccountManager.instance.FB_id;
                bindData = _dicBindData[BindDataType.Facebook];
                break;
            case BindDataState.Google:
                type = 1;
                id = AccountManager.instance.Google_id;
                bindData = _dicBindData[BindDataType.Google];
                break;
            case BindDataState.Apple:
                type = 2;
                id = AccountManager.instance.Apple_id;
                bindData = _dicBindData[BindDataType.Apple];
                break;
        }

        if (string.IsNullOrEmpty(id))
        {
            LogUtils.LogError($"Id is null with BindDataState: {bindDataState}");
            return;
        }


        
    }
    public BindDataState GetBindDataState()
    {
        if (!string.IsNullOrEmpty(AccountManager.instance.FB_id))
        {
            return BindDataState.Facebook;
        }

        if (!string.IsNullOrEmpty(AccountManager.instance.Google_id))
        {
            return BindDataState.Google;
        }

        if (!string.IsNullOrEmpty(AccountManager.instance.Apple_id))
        {
            return BindDataState.Apple;
        }

        return BindDataState.None;
    }

}

public enum BindDataType
{
    Facebook,
    Google,
    Apple
}

public enum BindDataState
{
    None,
    Facebook,
    Google,
    Apple
}
