using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using BasePuzzle.PuzzlePackages;
using PuzzleGames;
using UnityEngine;

public class UserResourceController : NMSingleton<UserResourceController>
{
    private string key = "resource";

    private UserResource _userResource;

    public UserResource UserResource
    {
        get => _userResource;
    }

    public Action onUpdateResource;

    protected override void Init()
    {
        InitData();
        CheckingDataFromRemoteConfig();
    }

    public void CheckingDataFromRemoteConfig()
    {
        var key = "gold_remote";
        /*
        if (RemoteConfigController.Config.goldInit != goldInit && !SaveLoadHandler.Exist(key))
        {
            var valueChange = RemoteConfigController.Config.goldInit - goldInit;
            _userResource.gold += valueChange;
            _userResource.gold = Mathf.Max(_userResource.gold, 0);

            Save();
            SaveLoadHandler.Save(key, 1);
        }
        */
    }

    private int goldInit
    {
        get => 150;
    }


    private void InitData()
    {
        if (SaveLoadHandler.Exist(key))
        {
            _userResource = SaveLoadHandler.Load<UserResource>(key);
        }
        else
        {
            _userResource = new UserResource()
            {
                gold = goldInit,
                heart = 5,
                timeInfiHeart = 0,
                star = 0
            };

            Save();
            var s = "gold_init";
    
        }

        CheckAndUpdateHeart();
    }

    public bool IsInfiHeart()
    {
        return DateTimeUtils.UtcNow < GetTargetTimeInfiHeart();
    }

    public DateTime GetTargetTimeInfiHeart()
    {
        return DateTimeUtils.GetDateTimeFromMiliSecond(_userResource.timeInfiHeart);
    }

    public TimeSpan GetTargetTimeSpanInfiHearth()
    {
        return DateTimeUtils.GetDateTimeFromMiliSecond(_userResource.timeInfiHeart) - DateTimeUtils.UtcNow;
    }

    public void AddInfiHeart(int minute)
    {
        var time = DateTimeUtils.GetDateTimeFromMiliSecond(_userResource.timeInfiHeart);
        if (time > DateTimeUtils.UtcNow)
        {
            time = time.AddMinutes(minute);
        }
        else
        {
            time = DateTimeUtils.UtcNow.AddMinutes(minute);
        }

        _userResource.timeInfiHeart = DateTimeUtils.GetMiliSecondFromDateTime(time);
        Save();
    }


    private const int NUM_SECOND_TO_INCREASE_HEART = 1800;

    public int NumSecondToInCreaseHeart
    {
        get { return NUM_SECOND_TO_INCREASE_HEART; }
    }

    public TimeSpan TimeSpanUpdateHeart
    {
        get
        {
            var nextTime = DateTimeUtils.GetDateTimeFromMiliSecond(_userResource.lastUpdateHeart);
            nextTime = nextTime.AddSeconds(NumSecondToInCreaseHeart);
            return nextTime - DateTimeUtils.UtcNow;
        }
    }

    public bool CanPlayLevel()
    {
        return _userResource.heart > 0 || IsInfiHeart();
    }

    public int GetMaxHeart()
    {
        return /*MasterPassController.instance.IsBuyPremium() ? MasterPassController.instance.GetMaxLive : */5;
    }

    public int NumHeartRestorePerLoop
    {
        get { return 1; }
    }

    private DateTime _targetTime;

    private void CheckAndUpdateHeart()
    {
        if (_userResource.heart >= GetMaxHeart())
        {
            if (_userResource.lastUpdateHeart != 0)
            {
                _userResource.lastUpdateHeart = 0;
                UpdateCurrencyWithOutCheckTime();
            }

            if (_couroutineUpdateHeart != null)
            {
                GameController.Instance.StopCoroutine(_couroutineUpdateHeart);
            }

            return;
        }

        if (_userResource.lastUpdateHeart == 0)
        {
            _userResource.lastUpdateHeart = DateTimeUtils.GetMiliSecondFromDateTimeNowUTC();
        }
        else
        {
            var preTime  = DateTimeUtils.GetDateTimeFromMiliSecond(_userResource.lastUpdateHeart);
            var timeSpan = DateTimeUtils.UtcNow - preTime;
            var Heart    = (int)(timeSpan.TotalSeconds / NumSecondToInCreaseHeart) * NumHeartRestorePerLoop;
            if (Heart > 0)
            {
                _userResource.heart = Mathf.Min(GetMaxHeart(), _userResource.heart + Heart);
                if (_userResource.heart >= GetMaxHeart())
                {
                    _userResource.lastUpdateHeart = 0;
                    UpdateCurrencyWithOutCheckTime();
                    return;
                }

                var currentTime = preTime.AddSeconds(Heart * NumSecondToInCreaseHeart / NumHeartRestorePerLoop);
                _userResource.lastUpdateHeart = DateTimeUtils.GetMiliSecondFromDateTime(currentTime);
                UpdateCurrencyWithOutCheckTime();
            }
        }

        _targetTime = DateTimeUtils.GetDateTimeFromMiliSecond(_userResource.lastUpdateHeart)
            .AddSeconds(NumSecondToInCreaseHeart);

        if (_couroutineUpdateHeart != null)
        {
            GameController.Instance.StopCoroutine(_couroutineUpdateHeart);
        }

        _couroutineUpdateHeart = GameController.Instance.StartCoroutine(CoroutineUpdateHeart());
    }

    private Coroutine _couroutineUpdateHeart;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(1);

    private IEnumerator CoroutineUpdateHeart()
    {
        while (DateTimeUtils.UtcNow < _targetTime)
        {
            yield return waitForSeconds;
        }

        if (_userResource.heart < GetMaxHeart())
        {
            AddHeart(1);
            _userResource.lastUpdateHeart = DateTimeUtils.GetMiliSecondFromDateTimeNowUTC();
            UpdateCurrencyWithOutCheckTime();
        }
    }

    private void UpdateCurrencyWithOutCheckTime()
    {
        Save();
        onUpdateResource?.Invoke();
    }

    public ObscuredInt GOLD_TO_BUY_HEART = 450;

    public bool CanBuyHeart()
    {
        return _userResource.gold >= GOLD_TO_BUY_HEART;
    }

    public void BuyHeart()
    {
        if (!CanBuyHeart())
        {
            return;
        }

        MinusGold(GOLD_TO_BUY_HEART, "buy_heart", "buy_heart");
        if (!IsMaxHeart())
        {
            SetHeart(GetMaxHeart());
        }


        GameController.UpdateDataToServer();
    }

    public bool IsMaxHeart()
    {
        return _userResource.heart >= GetMaxHeart();
    }

    public void WatchAds_FreeHeart(Action onComplete)
    {

    }


    private void Save()
    {
        SaveLoadHandler.Save(key, _userResource);
    }

    private void UpdateResource()
    {
        CheckAndUpdateHeart();
        UpdateCurrencyWithOutCheckTime();
    }

    public void SetMaxHeart()
    {
        SetHeart(GetMaxHeart());
    }

    public void SetHeart(int amount)
    {
        _userResource.heart = amount;
        UpdateResource();
    }

    public void SetGold(int amount)
    {
        _userResource.gold = amount;
        UpdateResource();
    }

    public void SetStar(int amount)
    {
        _userResource.star = amount;
        UpdateResource();
    }

    public void AddHeart(int amount)
    {
        if (amount <= 0) return;
        
        _userResource.heart += amount;

        _userResource.heart = Mathf.Min(_userResource.heart, GetMaxHeart());
        
        UpdateResource();
    }

    public static Action<int> onAddGold;

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        
        _userResource.gold += amount;
        UpdateResource();
        onAddGold?.Invoke(amount);
    }

    public static Action<int> onAddStar;

    public void AddStar(int amount)
    {
        if (amount <= 0) return;
        
        _userResource.star += amount;
        UpdateResource();
        onAddStar?.Invoke(amount);
    }
    
    public void SetRemoveAds(bool value)
    {
        _userResource.removeAds = value;
        UpdateResource();
    }

    public bool HasRemoveAds()
    {
#if ACCOUNT_TEST
        return false;
#endif
        
        return _userResource.removeAds;
    }

    public void MinusHeart(int amount)
    {
        if (IsInfiHeart())
        {
            return;
        }
        
        if (amount <= 0) return;

        _userResource.heart -= amount;
        if (_userResource.heart < 0)
        {
            _userResource.heart = 0;
        }

        UpdateResource();
    }

    public static Action<int> onMinusGold;

    public void MinusGold(int amount, string itemType, string itemId)
    {
        if (amount <= 0) return;
        
        var before = _userResource.gold;
        _userResource.gold -= amount;
        var after = _userResource.gold;
        UpdateResource();
        onMinusGold?.Invoke(amount);
    }

    public void MinusStar(int amount)
    {
        if (amount <= 0) return;
        
        _userResource.star -= amount;
        UpdateResource();
    }

    public bool CanMinusHeart(int amount)
    {
        return _userResource.heart >= amount;
    }

    public bool CanMinusGold(int amount)
    {
        return _userResource.gold >= amount;
    }

    public bool CanMinusStar(int amount)
    {
        return _userResource.star >= amount;
    }

    public void AddInfiTime(int minute)
    {
        var time = DateTimeUtils.GetDateTimeFromMiliSecond(_userResource.timeX2Reward);
        if (time > DateTimeUtils.UtcNow)
        {
            time = time.AddMinutes(minute);
        }
        else
        {
            time = DateTimeUtils.UtcNow.AddMinutes(minute);
        }

        _userResource.timeX2Reward = DateTimeUtils.GetMiliSecondFromDateTime(time);
        Save();
    }

    public bool IsX2Reward()
    {
        return DateTimeUtils.GetDateTimeFromMiliSecond(_userResource.timeX2Reward) > DateTimeUtils.UtcNow;
    }

    public TimeSpan GetTimeX2Reward()
    {
        return DateTimeUtils.GetDateTimeFromMiliSecond(_userResource.timeX2Reward) - DateTimeUtils.UtcNow;
    }

    #region Get free live

    public void GetFreeLive()
    {
        if (freeLives != null)
        {
            onGetFreeLive?.Invoke();
            return;
        }

    }

    public List<FreeLiveRowData> freeLives;
    public Action onGetFreeLive;

    public void GetFreeLiveFromServer(List<FreeLiveRowData> datas)
    {
        freeLives = datas;
        onGetFreeLive?.Invoke();
    }

    // truyền player code vào
    public void ReceiveFreeLive(int code)
    {
    }

    public int GetMaxLiveCanReceive()
    {
        if (freeLives == null || freeLives.Count == 0)
        {
            return 0;
        }

        int num = GetMaxHeart() - _userResource.heart;
        num = Mathf.Min(num, freeLives.Count);
        return num;
    }

    public void ReceiveMaxLive()
    {
        int num = GetMaxLiveCanReceive();
        if (num <= 0)
        {
            return;
        }

    }

    public Action onReceiveMaxFreeLive;

    public void ReceiveMaxLiveFromServer(int status, List<FreeLiveRowData> data, int num)
    {
        if (status == 1)
        {
            freeLives = data;
            AddHeart(num);
        }

        onReceiveFreeLive?.Invoke();
        onReceiveMaxFreeLive?.Invoke();
    }

    public Action onReceiveFreeLive;

    public void ReceiveFreeLiveFromSerer(int status, int playerCode)
    {
        if (freeLives != null)
        {
            for (int i = freeLives.Count - 1; i >= 0; i--)
            {
                if (freeLives[i].player_code == playerCode)
                {
                    freeLives.RemoveAt(i);
                    break;
                }
            }
        }

        if (status > 0)
        {
            AddHeart(1);
        }

        onReceiveFreeLive?.Invoke();
    }

    #endregion

    public void UpdateDataFromServer(UserResourceServer userResource)
    {
        _userResource.Update(userResource);
        UpdateResource();
    }
}


[Serializable]
public class UserResource
{
    public ObscuredBool removeAds;
    public ObscuredInt heart;
    public ObscuredInt gold;
    public ObscuredInt star;

    public long lastUpdateHeart;
    public long timeInfiHeart;
    public long timeX2Reward;

    public void Update(UserResourceServer userResource)
    {
        removeAds = userResource.removeAds;
        heart = userResource.heart;
        gold = userResource.gold;
        star = userResource.star;

        lastUpdateHeart = DateTimeUtils.GetMiliSecondFromDateTimeNowUTC();
        timeInfiHeart   = userResource.timeInfiHeart;
        timeX2Reward    = userResource.timeX2Reward;
    }
}