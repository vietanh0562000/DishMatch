using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoController : NMSingleton<UserInfoController>
{
    public const int num_avatar = 18;

    private const string key = "user_info_n";
    private const string key_change_name = "change_name";

    //Define the badges
    public List<int> badges = new() { 1, 2 };

    //Define the frames
    public int frame_master_pass = -1;

    public List<int> frame_star_collabs = new()
    {
        -2
    };

    public List<int> frame_diggings = new()
    {
        -3
    };

    public int frame_single_day = -4;

    public const int num_frame = 9;
    public List<int> frames;

    private UserProfile _userInfo;

    public UserProfile UserInfo
    {
        get => _userInfo;
    }

    public UserSimpleInfo UserSimpleInfo
    {
        get
        {
            UserSimpleInfo user = new UserSimpleInfo()
            {
                name = _userInfo.name,
                avatar_id = _userInfo.avatar_id,
                avatar_url = _userInfo.avatar_url,
                frame_id = _userInfo.frame_id,
                badge = _userInfo.badge,
                code = AccountManager.instance.Code,
                level = LevelDataController.instance.Level,
            };

            return user;
        }
    }

    private int _changeName;

    public int ChangeName
    {
        get => _changeName;
    }

    protected override void Init()
    {
        InitData();

        //Default frames
        frames = new List<int>(num_frame + 1);
        for (int i = 0; i < num_frame; i++)
        {
            frames.Add(i);
        }

        //Add the new frames
/*        frames.Add(frame_master_pass);
        frames.AddRange(frame_star_collabs);
        frames.AddRange(frame_diggings);
        frames.Add(frame_single_day);
        */
    }

    private void InitData()
    {
        if (SaveLoadHandler.Exist(key_change_name))
        {
            _changeName = SaveLoadHandler.Load<int>(key_change_name);
        }
        else
        {
            _changeName = 0;
        }

        if (SaveLoadHandler.Exist(key))
        {
            _userInfo = SaveLoadHandler.Load<UserProfile>(key);
        }
        else
        {
            _userInfo = new UserProfile()
            {
                name = $"Player#{RandomUtils.RangeInt(1000000, 10000000)}",
                avatar_id = 0
            };
            if (AccountManager.instance.Code > 0)
            {
                _userInfo.name = $"Player#{AccountManager.instance.Code}";
            }

            Save();
        }
    }

    public bool NeedShowChangeName()
    {
        return _changeName == 0 && _userInfo.name.StartsWith("Player#");
    }

    public void ShowChangeNameDone()
    {
        _changeName = 1;
        SaveChangeName();
    }

    public void SetName(string name)
    {
        name = name.Trim();
        if (!string.IsNullOrEmpty(name))
        {
            _userInfo.name = name;
        }

        Save();

        GameController.UpdateDataToServer();
    }

    public void SetAvatar(int id)
    {
        _userInfo.avatar_id = id;
        Save();
    }

    public void SetAvatar(int id, string url)
    {
        _userInfo.avatar_id = id;
        _userInfo.avatar_url = url;
        Save();
    }

    public void SetUrl(string url)
    {
        _userInfo.avatar_url = url;
        Save();
    }

    public void SetFrame(int frame)
    {
        _userInfo.frame_id = frame;
        Save();
        GameController.UpdateDataToServer();
    }

    public void SetBadge(int badge)
    {
        _userInfo.badge = badge;
        Save();
    }

    private void SaveChangeName()
    {
        SaveLoadHandler.Save(key_change_name, _changeName);
    }

    private void Save()
    {
        SaveLoadHandler.Save(key, _userInfo);
    }

    public bool HasSpecialAvatar()
    {
        return !string.IsNullOrEmpty(_userInfo.avatar_url);
    }

    public void UpdateDataFromServer(UserProfile userInfo, int changeName)
    {
        _userInfo.Update(userInfo);
        Save();

        _changeName = changeName;
        SaveChangeName();
    }

    private Action<UserInfoDetail> _onGetUserInfoDetail;
    private int _currentCode;
    private UserInfoDetail _userInfoDetail;

    public UserInfoDetail GetMyUserInfoDetail()
    {
        if (_userInfoDetail == null)
        {
            _userInfoDetail = new UserInfoDetail()
            {
                code = AccountManager.instance.Code,
                level = LevelDataController.instance.Level,
                avatar_id = UserInfo.avatar_id,
                frame_id = UserInfo.frame_id,
                badge = UserInfo.badge,
                avatar_url = UserInfo.avatar_url,
                name = UserInfo.name,
                //teamInfo = null,
                firstTryWins = LevelDataController.instance.LevelData.firstTryWins
            };
        }

        _userInfoDetail.code = AccountManager.instance.Code;
        _userInfoDetail.avatar_id = UserInfo.avatar_id;
        _userInfoDetail.avatar_url = UserInfo.avatar_url;
        _userInfoDetail.name = UserInfo.name;
        _userInfoDetail.frame_id = UserInfo.frame_id;
        _userInfoDetail.badge = UserInfo.badge;

        return _userInfoDetail;
    }

    public void GetUserinfoDetail(int code, Action<UserInfoDetail> callBack)
    {
        _currentCode = code;
        _onGetUserInfoDetail = callBack;
        if (_coroutineGetInfo != null)
        {
            GameController.Instance.StopCoroutine(_coroutineGetInfo);
        }

       // GameController.SendMessage(new CSGetUserInfoDetail(code));

        if (_currentCode == AccountManager.instance.Code)
        {
            _coroutineGetInfo = GameController.Instance.StartCoroutine(GetInfoTimeout());
        }
    }

    private Coroutine _coroutineGetInfo;

    private IEnumerator GetInfoTimeout()
    {
        yield return new WaitForSeconds(2);
        if (_currentCode == AccountManager.instance.Code)
        {
            _onGetUserInfoDetail?.Invoke(GetMyUserInfoDetail());
            _onGetUserInfoDetail = null;
        }
    }

    public void GetUserinfoDetailFromServer(UserInfoDetail userInfo)
    {
        if (_coroutineGetInfo != null)
        {
            GameController.Instance.StopCoroutine(_coroutineGetInfo);
        }

        _userInfoDetail = userInfo;
        if (userInfo.code == AccountManager.instance.Code)
        {
            userInfo = GetMyUserInfoDetail();
        }

        _onGetUserInfoDetail?.Invoke(userInfo);
        _onGetUserInfoDetail = null;
    }
}