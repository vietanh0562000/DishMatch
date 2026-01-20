using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using System;
using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
using PuzzleGames;

public class LevelDataController : NMSingleton<LevelDataController>
{
    private const string key_en = "level_en";

    private LevelData _levelData;

    public LevelData LevelData
    {
        get => _levelData;
    }

    public int Level
    {
        get => _levelData.level;
    }

    public int NumPlay
    {
        get => _levelData.numPlay;
    }

    protected override void Init()
    {
        InitData();
    }

    public LevelDifficulty LevelDifficulty;

    private void InitData()
    {
        if (SaveLoadHandler.Exist(key_en))
        {
            _levelData = SaveLoadHandler.Load<LevelData>(key_en);
        }
        else
        {
            _levelData = new LevelData()
            {
                level = 1,
                numPlay = 0
            };
        }
        
        Debug.Log("Lost " + _levelData.numberLostAfterSupportPack);

        if (SaveLoadHandler.NormalExist(key_max_level_server))
        {
            _maxLevelFromServer = SaveLoadHandler.NormalLoad<int>(key_max_level_server);
        }
        else
        {
            _maxLevelFromServer = 0;
        }
    }

    public void Save()
    {
        SaveLoadHandler.Save(key_en, _levelData);
    }

    public static Action onPlay;

    public void Play()
    {

        _levelData.numPlay++;
        Save();
        onPlay?.Invoke();
    }

    public static Action onCompleteLevel;

    public int GetMaxLevel()
    {
        return ServerConfig.Instance<ValueRemoteConfig>().maxLevelForRandomLoad;
    }

    private const string key_max_level_server = "max_level_server";
    private int _maxLevelFromServer;
    public int MaxLevelFromServer => _maxLevelFromServer;
    
    public bool IsFirstTryLevel => _levelData.numPlay == 0;

    public void CheckSetMaxLevelFromServer(int maxLevel)
    {
        if (maxLevel <= _maxLevelFromServer)
        {
            return;
        }

        _maxLevelFromServer = maxLevel;
        SaveLoadHandler.NormalSave(key_max_level_server, _maxLevelFromServer);
    }

    /// <summary>
    /// chỉ dùng ở màn endgame
    /// </summary>
    /// <returns></returns>
    public int GetLevelJustPassed()
    {
        return _levelJustPassed;
    }

    private int _levelJustPassed;
    private bool _isPassedLevelMaxBefore;

    /// <summary>
    /// Chỉ dùng ở màn endgame
    /// </summary>
    /// <value></value>
    public bool IsPassedLevelMaxBefore
    {
        get => _isPassedLevelMaxBefore;
    }

    public bool IsPassLevelMax()
    {
        return _levelData.level >= GetMaxLevel() && SaveLoadHandler.Exist(GetKeyPassLevelMax());
    }

    private const string key_level_max_random = "lv_m_r";

    public int GetLevelRandomWhenMaxLevel()
    {
        if (!SaveLoadHandler.Exist(key_level_max_random))
        {
            GenNewLevelMax();
        }

        return SaveLoadHandler.Load<int>(key_level_max_random);
    }

    public void GenNewLevelMax()
    {
        var maxLevel = GetMaxLevel();
        var level = Mathf.Min(maxLevel, RandomUtils.RangeInt(41, maxLevel + 1));
        SaveLoadHandler.Save(key_level_max_random, level);
    }

    public void ResetLostBuy()
    {
        _levelData.numberLostAfterSupportPack = 0;
    }

    public bool IsAvailableForBuySupportPack()
    {
        Debug.Log("Lost " + _levelData.numberLostAfterSupportPack);
        return _levelData.numberLostAfterSupportPack >= ConstantValue.LOST_TIMES_TO_SHOW_SUPPORT_PACK;
    }

    private string GetKeyPassLevelMax()
    {
        return $"pass_lv_{GetMaxLevel()}";
    }

    public static Action onLose;

    public void Lose()
    {
        onLose?.Invoke();
        _levelData.totalLost++;
        _levelData.numberLostAfterSupportPack++;

        GameController.UpdateDataToServer();
    }

    public void CompleteLevel()
    {
        try
        {
            _isPassedLevelMaxBefore = false;
            bool log = false;
            _levelJustPassed = 0;
            if (_levelData.numPlay == 1)
            {
                _levelData.firstTryWins++;
            }

            if (_levelData.level < GetMaxLevel())
            {
                _levelData.level++;
                _levelData.numPlay = 0;
                _levelJustPassed = _levelData.level - 1;
                log = true;
            }
            else
            {
                var keySave = GetKeyPassLevelMax();
                if (!SaveLoadHandler.Exist(keySave))
                {
                    SaveLoadHandler.Save(keySave, 1);
                    log = true;
                }
                else
                {
                    _isPassedLevelMaxBefore = true;
                }

                _levelJustPassed = _levelData.level;
                // GenNewLevelMax();
            }


            if (log)
            {

            }

            Save();
            onCompleteLevel?.Invoke();

            GameController.UpdateDataToServer();

        }
        catch (System.Exception e)
        {
            LogUtils.LogError(e);
        }
    }

    public void UpdateDataFromServer(LevelDataServer levelData)
    {
        _levelData.Update(levelData);

        Save();
    }
}

public class LevelData
{
    public ObscuredInt level;
    public int         numPlay;
    public int         firstTryWins;
    public int         totalLost;
    public int         numberLostAfterSupportPack;

    public LevelData()
    {
        numberLostAfterSupportPack = ConstantValue.LOST_TIMES_TO_SHOW_SUPPORT_PACK;
    }

    public void Update(LevelDataServer levelData)
    {
        level        = levelData.level;
        numPlay      = levelData.numPlay;
        firstTryWins = levelData.firstTryWins;
        totalLost    = levelData.totalLost;
        
        if (totalLost <= 0)
        {
            numberLostAfterSupportPack = ConstantValue.LOST_TIMES_TO_SHOW_SUPPORT_PACK;
        }
        else
        {
            numberLostAfterSupportPack = levelData.numberLostAfterSupportPack;
        }
    }
}