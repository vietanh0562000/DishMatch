using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AudioController : Singleton<AudioController>
{
    [SerializeField] private AudioSource _bgMusic;

    [SerializeField] private AudioClip[] _bgMusics;

    [SerializeField] private Sound[] _sounds;

    private Dictionary<SoundKind, Sound> _soundContainer;

    [SerializeField] private Sound[] _soundMatch3_0;

    [Button]
    public void Validate()
    {
        List<SoundKind> soundKinds = new List<SoundKind>();
        bool valid = true;
        foreach (var item in _sounds)
        {
            if (item.clip == null)
            {
                Debug.LogError($"{item.soundKind} is null");
                valid = false;
            }

            if (item.num <= 0)
            {
                Debug.LogError($"Num {item.soundKind} is {item.num}");
                valid = false;
            }

            if (item.volume <= 0)
            {
                Debug.LogError($"Volume {item.soundKind} is {item.volume}");
                valid = false;
            }

            if (soundKinds.Contains(item.soundKind))
            {
                Debug.LogError($"{item.soundKind} was added more time");
                valid = false;
            }
            else
            {
                soundKinds.Add(item.soundKind);
            }
        }

        if (valid)
        {
            Debug.Log("Data is valid");
        }
    }


    [Button]
    public void GetIndex(SoundKind soundKind)
    {
        for (int i = 0; i < _sounds.Length; i++)
        {
            if (_sounds[i].soundKind == soundKind)
            {
                Debug.LogError($"Index: {i}");
                return;
            }
        }

        Debug.LogError($"Not found");
    }

    protected override void Awake()
    {
        base.Awake();

        _soundContainer = new Dictionary<SoundKind, Sound>();

        foreach (Sound s in _sounds)
        {
            s.Init(transform);
            _soundContainer.Add(s.soundKind, s);
        }

        foreach (Sound s in _soundMatch3_0)
        {
            s.Init(transform);
        }

        // InGameEvent.onEndGame += EndGame;

        AudioDataController.instance.onChangeMusic += OnChange;

        SceneController.instance.onChangeSceneState += OnChangeScene;
    }

    private void OnChangeScene(SceneState state)
    {
        StopMusic();
    }

    private void OnChange()
    {
        if (AudioDataController.instance.IsActiveMusic())
        {
            PlayMusic();
        }
        else
        {
            StopMusic();
        }
    }

    protected override void OnDestroy()
    {
        // InGameEvent.onEndGame -= EndGame;
        AudioDataController.instance.onChangeMusic -= OnChange;
        SceneController.instance.onChangeSceneState -= OnChangeScene;
        base.OnDestroy();
    }

    Tween _tween;
    // private void EndGame(EndGameState state)
    // {
    //     if (state == EndGameState.Win)
    //     {
    //         _tween = _bgMusic.DOFade(0, 0.5f).OnComplete(()=>
    //         {
    //             StopMusic();
    //         });
    //     }
    // }

    private void OnDisable()
    {
        if (_tween != null && _tween.IsActive())
        {
            _tween.Kill();
        }
    }

    private void Start()
    {
        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(0.75f);
            OnChange();
        }

        StartCoroutine(Coroutine());
    }

    [Button]
    public void PlayMusic(int index = 0, float volume = 1)
    {
        if (!AudioDataController.instance.IsActiveMusic())
        {
            return;
        }
        
        _bgMusic.clip   = _bgMusics[index];
        _bgMusic.volume = volume;
        _bgMusic.Play();
    }

    [Button]
    public void StopMusic()
    {
        _bgMusic.Stop();
    }

    [Button]
    public static void PlaySound(SoundKind kind)
    {
        if (!AudioDataController.instance.IsActiveSound())
        {
            return;
        }

        if (!HasInstance)
        {
            LogUtils.LogError("AudioController not instantiated!");
            return;
        }

        if (!Instance._soundContainer.ContainsKey(kind))
        {
            LogUtils.LogError($"Sound {kind} not found !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return;
        }

        Instance._soundContainer[kind].Play();
    }

    [Button]
    public static void PlaySound(SoundKind kind, float volume)
    {
        if (!AudioDataController.instance.IsActiveSound())
        {
            return;
        }

        if (!HasInstance)
        {
            LogUtils.LogError("AudioController not instantiated!");
            return;
        }

        if (!Instance._soundContainer.ContainsKey(kind))
        {
            LogUtils.LogError($"Sound {kind} not found !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            return;
        }

        Instance._soundContainer[kind].Play(volume);
    }

    public static void PlaySoundMatch3(int combo)
    {
        if (!AudioDataController.instance.IsActiveSound())
        {
            return;
        }

        try
        {
            var soundKind = Instance._soundMatch3_0;
            var audio = soundKind[Mathf.Min(combo - 1, soundKind.Length - 1)];
            audio.Play();
        }
        catch (System.Exception)
        {
        }
    }

    public static void Stop(SoundKind kind)
    {
        if (!AudioDataController.instance.IsActiveSound())
        {
            return;
        }

        if (!HasInstance)
        {
            LogUtils.LogError("AudioController not instantiated!");
            return;
        }

        if (!Instance._soundContainer.ContainsKey(kind))
        {
            LogUtils.LogError($"Sound {kind} not found");
            return;
        }

        Instance._soundContainer[kind].Stop();
    }
}

public enum SoundKind
{
    InGameStartWave,
    InGameMatch3,
    InGamePickUp,
    InGamePutDown,
    InGameVictory,
    InGameMatch3X2,
    InGameMatch3X3,
    InGameMatch3X4,
    InGameMatch3X5,
    InGameMatch3X6,
    InGameMatch3X7,
    InGameMatch3X8,
    InGameTimeBonus,
    UserPowerup,
    PowerupMagicWard,
    PowerupSwap,
    PowerupFreeze,
    CellLockBreak,
    CellLockBreakAll,
    CellLockUnLock,
    UIClickButton,
    UIShowPopup,
    PowerupBreakItem,
    InGameMatch3X9,
    InGameMatch3X10,
    InGameMatch3X11,
    Booster_Hammer,
    Booster_x2_Star,
    Booster_Buff_Combo,
    Collect_Golden_Gift,
    LeaderBoard_Update_Rank,
    RecivedStar,
    UIProgressBar_Fill,
    UIBellSound,
    UIResourcesAppear,
    UIRecivedItem,
    UIRecivedGold,
    InGameVictoryLoop,
    UIChestOpen,
    InGamePreVictory,
    UIHiddenTreasureLifeBuoyDrop,
    UIHiddenTreasurePlayerDrop,
    UIHiddenTreasurePlayerJump,
    UIHiddenTreasureShellOpen,
    UIHiddenTreasurePlayerFall,
    UIHiddenTreasureLifeBuoyAppear,
    UIHiddenTreasureJumpBoard,
    UILineDraw,
    UIRollingWheel,
    UISingleConfetti,
    UICheer,
    UIStockingChallenge1xDrop,
    UIStockingChallenge2xDrop,
    UIStockingChallenge4xDrop,
    UIStockingChallenge6xDrop,
    UIStockingChallenge10xDrop,
    UIStockingChallengeFall,
    UIProgressBar_Reverse,
    InGameWarning,
    InGameRecivePowerUp,
    UICashCheck,
    UICheck,
    UIUnlock,
    UIDisappear,
    IngameRandomWheel,
    UICollectionPackShow,
    UICollectionCardShow,
    UICollectionCardFlip,
    UICollectionSpecialCard,
    UICollectionUnpack,
    LoadingLogoAppear,
    LoadingLogoDisappear,
    IngameCellDrop,
    UIBannerTeamBattleDoorOpen,
    UIBannerTeamBattleDoorClose,
    UILuckySpinning,
    UILevelUp,
    UIWheelRewardReceived,
    UISwitchScene,
    UIScoopSquadCartonBox,
    UITrickorDig_Dig,
    UITrickorDig_SuperDig,
    UITrickorDig_TreasureAppear,
    UITrickorDig_TreasureRecived,
    UITrickorDig_Chest_Open,
    IngameBombExplode,
    IngameBombFuse,
    UIRollingStop,
    UIUpgradeGoldPack,
    OnTouchHole,
    OnTouchHoleFailed,
    StickmanJump,
    HoleClosed,
    FloorAppear,
    TunnelSpawn,
    BarrierOn,
    BarrierOff,
    UseBoosterAddSlot,
    UseBoosterUFO,
    UseBoosterRainbow,
    StickmanInHole,
    FrozenMelting,
    Combo2,
}

[System.Serializable]
public class Sound
{
    public SoundKind soundKind;
    public int num;
    [Range(0, 1)] public float volume = 1;

    public AudioClip clip;


    private AudioSource[] sources;

    public void Init(Transform parent)
    {
        if (clip == null)
        {
            LogUtils.LogError($"{soundKind} is null !!!!!!!!!!!!!!!!!");
            return;
        }

        sources = new AudioSource[num];
        for (int i = 0; i < num; i++)
        {
            var obj = new GameObject(clip.name);
            obj.transform.SetParent(parent);

            var source = obj.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            sources[i] = source;
        }
    }

    public void Play()
    {
        for (int i = 0; i < num; i++)
        {
            if (!sources[i].isPlaying)
            {
                sources[i].volume = volume;
                sources[i].Play();
                return;
            }
        }
    }

    public void Play(float volume)
    {
        for (int i = 0; i < num; i++)
        {
            if (!sources[i].isPlaying)
            {
                sources[i].volume = volume;
                sources[i].Play();
                return;
            }
        }
    }

    public void Stop()
    {
        for (int i = 0; i < num; i++)
        {
            if (sources[i].isPlaying)
            {
                sources[i].Stop();
            }
        }
    }
}