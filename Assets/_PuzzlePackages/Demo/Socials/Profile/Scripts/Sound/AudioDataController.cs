using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDataController : NMSingleton<AudioDataController>
{
    private const string key = "audio";

    private TurnOnOffAudio _turnOnOffAudio;

    protected override void Init()
    {
        InitData();
    }

    private void InitData()
    {
        if (SaveLoadHandler.Exist(key))
        {
            _turnOnOffAudio = SaveLoadHandler.Load<TurnOnOffAudio>(key);
        }
        else
        {
            _turnOnOffAudio = new TurnOnOffAudio(){
                music = 1,
                sound = 1
            };
        }
    }

    private void Save()
    {
        SaveLoadHandler.Save(key, _turnOnOffAudio);
    }

    public Action onChange;

    public Action onChangeMusic;
    public void SetMusic(int state)
    {
        if (_turnOnOffAudio.music == state)
        {
            return;
        }
        _turnOnOffAudio.music = state;
        Save();
        onChange?.Invoke();
        onChangeMusic?.Invoke();
    }

    public void SetSound(int state)
    {
        if (_turnOnOffAudio.sound == state)
        {
            return;
        }
        _turnOnOffAudio.sound = state;
        Save();
        onChange?.Invoke();
    }

    public bool IsActiveMusic()
    {
        return _turnOnOffAudio.music == 1;
    }

    public bool IsActiveSound()
    {
        return _turnOnOffAudio.sound == 1;
    }


}

[Serializable]
public class TurnOnOffAudio
{
    public int music;
    public int sound;

    public TurnOnOffAudio()
    {

    }
}
