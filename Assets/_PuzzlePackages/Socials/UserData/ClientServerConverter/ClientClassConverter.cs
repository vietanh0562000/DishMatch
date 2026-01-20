using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientClassConverter
{
    private static GameInfo _userData = new GameInfo()
    {
        userInfo = new UserProfile(),
        level = new LevelDataServer(),
        userResource = new UserResourceServer(),
        powerup = new PowerupDataServer()
    };

    public static GameInfo GetUserData()
    {
        _userData.hackSuspicion = AccountManager.instance.HackSuspicion;
        _userData.userInfo.Update(UserInfoController.instance.UserInfo);
        _userData.changeName = UserInfoController.instance.ChangeName;
        _userData.level.Update(LevelDataController.instance.LevelData);
        _userData.powerup.Update(PowerUpDataController.instance.PowerupData);

        return _userData;
    }
}