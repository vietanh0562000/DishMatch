using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UI_CacheResource 
{
    private static int _preStar = 0;
    private static int _curStar = 0;
    private static int _preGold = 0;
    private static int _curGold = 0;

    public static int PreStar {
        get => _preStar;
    }

    public static int CurStar {
        get => _curStar;
    }

    public static int PreGold {
        get => _preGold;
    }

    public static int CurGold {
        get => _curGold;
    }

    public static bool IsWinLevel {
        get => GameController.Instance.IsWinLevel;
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitListener()
    {
        SceneController.instance.onChangeSceneState += OnChangeScene;
    }

    private static void OnChangeScene(SceneState state)
    {
        if (state == SceneState.Level)
        {
            _preStar = UserResourceController.instance.UserResource.star;
            _preGold = UserResourceController.instance.UserResource.gold;
        }
        else if (state == SceneState.Menu && SceneController.instance.PreSceneState == SceneState.Level)
        {
            _preGold = Mathf.Min(_preGold, UserResourceController.instance.UserResource.gold);
            _curStar = UserResourceController.instance.UserResource.star;
            _curGold = UserResourceController.instance.UserResource.gold;
        }
    }


    public static void ConfirmUpdate()
    {
        _preGold = _curGold;
        _preStar = _curStar;
    }
}
