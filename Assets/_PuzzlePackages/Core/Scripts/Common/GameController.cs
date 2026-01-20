using System;
using System.Collections;
using System.Collections.Generic;
using BasePuzzle.Core.Scripts;
using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
using UnityEngine;
using BasePuzzle.Core.Scripts.Services.GameObjs;
using BasePuzzle;
using BasePuzzle.PuzzlePackages.Core.UserData;
using Object = System.Object;

public class GameController : PersistentSingleton<GameController>
{
    private readonly static List<string> _deviceTests = new List<string>();
    private static bool _isDeviceTest;

    public static bool IsDeviceTest
    {
        get => _isDeviceTest;
    }


    public Action<bool> onApplicationPause;

    public int CountShowInterstitial;

    protected override void Awake()
    {
        base.Awake();
#if ENABLE_LOG || UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif

        CountShowInterstitial = ServerConfig.Instance<ValueRemoteConfig>().numInterstitialToShowRemoveAds;
        
        _isDeviceTest = false; //_deviceTests.Contains(AccountManager.instance.DeviceUniqueId);
        IsWinLevel = false;
        SceneController.instance.onChangeSceneState += ChangeSceneState;

        FGameObj.OnGameStop += GameStop;
    }

    private void GameStop(object sender, EventArgs e)
    {
        var sessionTime = GetTimeInScene();
        LogSession((int)sessionTime, SceneController.instance.CurrentSceneState);
    }

    public bool IsWinLevel;

    private void Start()
    {
        StartCoroutine(DelayStart());

        IEnumerator DelayStart()
        {
            while (!GameMain.InitComplete)
            {
                yield return null;
            }

            //FunnelLog.instance.CheckAndLogFirstOpen();
        }
    }

    private readonly Dictionary<SceneState, string> _dicSceneName = new Dictionary<SceneState, string>()
    {
        { SceneState.Menu, "menu" },
        { SceneState.Level, "level" }
    };

    private const string KEY_SESSION = "session";

    private DateTime _sessionDateTimeByScene;

    public void ChangeSceneState(SceneState sceneState)
    {
        if (SceneController.instance.PreSceneState != SceneState.None)
        {
            var sessionTime = GetTimeInScene();
            LogSession((int)sessionTime, SceneController.instance.PreSceneState);
        }

        _sessionDateTimeByScene = DateTimeUtils.UtcNow;

        if (sceneState == SceneState.Level)
        {
            IsWinLevel = false;
        }

        LevelDataController.onCompleteLevel += OnCompleteLevel;
    }

    private void OnCompleteLevel()
    {
        IsWinLevel = true;
    }

    public double GetTimeInScene()
    {
        return (DateTimeUtils.UtcNow - _sessionDateTimeByScene).TotalSeconds;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (SceneController.instance.CurrentSceneState == SceneState.None)
        {
            return;
        }

        onApplicationPause?.Invoke(pauseStatus);

        if (!pauseStatus)
        {
            _sessionDateTimeByScene = DateTimeUtils.UtcNow;
        }
    }

    public void LogSession(int sessionTime, SceneState sceneState)
    {
        if (sessionTime > 3600)
        {
            return;
        }

        if (!_dicSceneName.ContainsKey(sceneState))
        {
            return;
        }

        string scene = _dicSceneName[sceneState];
        //FalconDWHLog.SessionLog(sessionTime, scene);
        _sessionDateTimeByScene = DateTimeUtils.UtcNow;
    }

    private struct SessionScene
    {
        public int scene;
        public long session;
    }


    private static Coroutine _coroutineUpdateToServer;

    public static void UpdateDataToServer()
    {
        if (_coroutineUpdateToServer != null)
        {
            Instance.StopCoroutine(_coroutineUpdateToServer);
        }

        LogUtils.LogError("UpdateDataToServer");
        _coroutineUpdateToServer = Instance.StartCoroutine(DelayUpdateToServer());
    }

    //private static WaitForEndOfFrame wait = new WaitForEndOfFrame();

    private static IEnumerator DelayUpdateToServer()
    {
        yield return null;
        //UserDataManager.UpdateDataToServer();
    }


        public static void SendMessage(Object message)
    {

    }

    public static bool CanSendMessage()
    {
        return false;
    }
}