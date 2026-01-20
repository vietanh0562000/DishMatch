using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using System;
using PuzzleGames;

/// <summary>
/// https://docs.google.com/spreadsheets/d/1yyC2WUpHfm9LbUOkhmxB5Mz45azEvrOkc1xFC5src2I/edit#gid=417229805
/// </summary>
public class PowerUpDataController : NMSingleton<PowerUpDataController>
{
    private const string key = "power_up";

    private PowerupData _powerupData;

    public PowerupData PowerupData
    {
        get => _powerupData;
    }

    protected override void Init()
    {
        InitData();
        CheckingDataFromRemoteConfig();
    }

    public void CheckingDataFromRemoteConfig()
    {
        /*
        var key = "powerup_remote";
        if (RemoteConfigController.Config.numPowerupInit != numPowerupInit && !SaveLoadHandler.Exist(key))
        {
            var valueChange = RemoteConfigController.Config.numPowerupInit - numPowerupInit;
            for (int i = 0; i < _powerupData.powerups.Length; i++)
            {
                _powerupData.powerups[i] += valueChange;
                _powerupData.powerups[i] = Mathf.Max(_powerupData.powerups[i], 0);
            }
            Save();
            SaveLoadHandler.Save(key, 1);
        }
        */
    }

    private int numPowerupInit
    {
        get => 1;
    }

    private void InitData()
    {
        var numPowerup = CollectionUtils.GetNumElementEnum<PowerupKind>();
        if (SaveLoadHandler.Exist(key))
        {
            _powerupData = SaveLoadHandler.Load<PowerupData>(key);
            if (_powerupData.powerups.Length < numPowerup)
            {
                var oldPowerups = _powerupData.powerups;

                _powerupData.powerups = new ObscuredInt[numPowerup];

                for (int i = 0; i < oldPowerups.Length; i++)
                {
                    _powerupData.powerups[i] = oldPowerups[i];
                }

                for (int i = oldPowerups.Length; i < numPowerup; i++)
                {
                    _powerupData.powerups[i] = 0;
                }

                Save(); 
            }

            if (_powerupData.powerupFrees == null || _powerupData.powerupFrees.Length == 0)
            {
                _powerupData.powerupFrees = new ObscuredInt[numPowerup];
                for (int i = 0; i < _powerupData.powerupFrees.Length; i++)
                {
                    _powerupData.powerupFrees[i] = 1;
                }

                Save();
            }
        }
        else
        {
            _powerupData = new PowerupData();
            _powerupData.powerups = new ObscuredInt[numPowerup];
            _powerupData.powerupFrees = new ObscuredInt[numPowerup];
            for (int i = 0; i < _powerupData.powerupFrees.Length; i++)
            {
                var before = _powerupData.powerups[i];
                _powerupData.powerups[i] = numPowerupInit;
                var after = _powerupData.powerups[i];
                _powerupData.powerupFrees[i] = 1;
                // FalconDWHLog.ResourceLog(ResourceConstants.SOURCE, "pow_init", "pow_init", (PowerupKind)i,
                //     numPowerupInit), before, after);
                // new FResourceLog(FlowType.Source,"pow_init",((PowerupKind) i).ToString(),"pow_init",numPowerupInit)
                //     .Send();
            }

            Save();
        }
    }

    private void Save()
    {
        SaveLoadHandler.Save(key, _powerupData);
    }

    public int GetNumPowerup(PowerupKind powerup)
    {
        return _powerupData.powerups[(int)powerup];
    }

    public int GetNumPowerupFree(PowerupKind powerup)
    {
        return _powerupData.powerupFrees[(int)powerup];
    }

    public void AddPowerup(PowerupKind powerup, int num)
    {
        if (num <= 0) return;
        
        _powerupData.powerups[(int)powerup] += num;

        Save();
    }

    public void AddPowerupFree(PowerupKind powerup, int num)
    {
        if (num <= 0) return;
        
        _powerupData.powerupFrees[(int)powerup] += num;

        Save();
    }

    public bool CanUsePowerup(PowerupKind powerupKind)
    {
        return GetNumPowerupFree(powerupKind) > 0 || GetNumPowerup(powerupKind) > 0;
    }

    public void UserPowerup(PowerupKind powerup, int num, string where)
    {
        if (GetNumPowerupFree(powerup) > 0)
        {
            MinusPowerupFree(powerup, num);
        }
        else
        {
            MinusPowerup(powerup, num, where);
        }

        // TODO Current Level
        //new FPropertyLog("use_power_up", powerup.ToString(), 0, 0).Send();
        GameController.UpdateDataToServer();
    }

    public static Action<int> onUsePowerup;

    private void MinusPowerup(PowerupKind powerup, int num, string where)
    {
        var before = _powerupData.powerups[(int)powerup];
        _powerupData.powerups[(int)powerup] -= num;
        if (_powerupData.powerups[(int)powerup] < 0)
        {
            _powerupData.powerups[(int)powerup] = 0;
        }

        var after = _powerupData.powerups[(int)powerup];

        Save();
        onUsePowerup?.Invoke(num);

        // FalconDWHLog.ResourceLog(ResourceConstants.SINK, where, where, new ResourceValue(ResourceClassify.DicResourcePowerup[powerup], num), before, after);
        //new FResourceLog(FlowType.Sink, where, powerup.ToString(), where, num).Send();
    }

    private void MinusPowerupFree(PowerupKind powerup, int num)
    {
        _powerupData.powerupFrees[(int)powerup] -= num;
        if (_powerupData.powerupFrees[(int)powerup] < 0)
        {
            _powerupData.powerupFrees[(int)powerup] = 0;
        }

        Save();
        onUsePowerup?.Invoke(num);
    }
    
    public bool IsFree(PowerupKind powerupKind)
    {
        return GetNumPowerupFree(powerupKind) > 0;
    }
    
    private const int num_item_for_tut = 1;
    private PowerupKind? powerupKindTut = null;

    public void AddItemForTut(PowerupKind kind)
    {
        if (_powerupData.powerups[(int)kind] <= 0)
        {
            AddPowerup(kind, num_item_for_tut);
        }

        if (_powerupData.powerupFrees[(int)kind] <= 0)
        {
            AddPowerupFree(kind, 1);
        }

        powerupKindTut = kind;
    }

    public bool IsTut(PowerupKind kind)
    {
        return powerupKindTut == kind;
    }

    public void ActivePowerupTut()
    {
        powerupKindTut = null;
    }

    public void UpdateDataFromServer(PowerupDataServer powerup)
    {
        _powerupData.Update(powerup);
        Save();
    }

    public static readonly Dictionary<PowerupKind, string> DicPowerupNameToLog = new Dictionary<PowerupKind, string>
    {
        
    };
}

[Serializable]
public class PowerupData
{
    public ObscuredInt[] powerups;

    public ObscuredInt[] powerupFrees;

    public void Update(PowerupDataServer powerupData)
    {
        powerups = new ObscuredInt[powerupData.powerups.Length];
        for (int i = 0; i < powerups.Length; i++)
        {
            powerups[i] = powerupData.powerups[i];
        }

        powerupFrees = new ObscuredInt[powerupData.powerupFrees.Length];
        for (int i = 0; i < powerups.Length; i++)
        {
            powerupFrees[i] = powerupData.powerupFrees[i];
        }
    }
}