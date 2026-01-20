using System;
using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;
using static Lofelt.NiceVibrations.HapticPatterns;

public class HapticController : NMSingleton<HapticController>
{
    private const string key = "haptic";

    //public const float targetTime = 0.015f;
    //public const float targetAmplitude;
    //public const float targetFrequency;

    private int _state;
    public int State
    {
        get => _state;
    } 

    protected override void Init()
    {
        if (SaveLoadHandler.Exist(key))
        {
            _state = SaveLoadHandler.Load<int>(key, 1);
        }
        else
        {
            _state = 1;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitHapticNative()
    {
        Lofelt.NiceVibrations.HapticController.Init();
        var h = instance;
    }

    private void Save()
    {
        SaveLoadHandler.Save(key, _state);
    }

    public Action onChangeState;
    public void SetState(int state)
    {
        if (_state == state)
        {
            return;
        }
        _state = state;
        Save();
        onChangeState?.Invoke();
    }

    public bool IsActive()
    {
        return _state == 1;
    }

    public void Play(PresetType presetType = PresetType.Selection)
    {
        if (_state == 0)
        {
            return;
        }

        HapticPatterns.PlayPreset(presetType);
    }
}
