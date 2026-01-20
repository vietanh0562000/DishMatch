using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ServerClassConverter
{
    public static void GetUpdateDataFromServer(GameInfo userData)
    {
        UserInfoController.instance.UpdateDataFromServer(userData.userInfo, userData.changeName);
        LevelDataController.instance.UpdateDataFromServer(userData.level);
        PowerUpDataController.instance.UpdateDataFromServer(userData.powerup);
        UserResourceController.instance.UpdateDataFromServer(userData.userResource);
        //LeaderBoardFakeController.instance.UpdateDataFromServer(userData.leaderBoardFake);

        if (LevelDataController.instance.Level == 1)
        {
            SceneController.instance.LoadLevel();
        }
        else
        {
            SceneController.instance.LoadMenu();
        }
    }
}