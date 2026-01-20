using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataConst
{
    public const string SCENE_MENU = "Home";
    public const string SCENE_LEVEL = "GamePlay";

    public static bool IsTurnOnRcmBooster()
    {
        return false;
    }

    public static bool IsNewTut()
    {
        return false;
    }

    public static bool IsForceTut()
    {
        return false;
    }

    public const string EVENT_UNLOCK = "event_unlock";
}