using System.Collections;
using System.Collections.Generic;
using BasePuzzle.PuzzlePackages.Core;
using BasePuzzle.PuzzlePackages.Settings;
using UnityEngine;

public class btnSettings : MonoBehaviour
{
    public void OnClick_Show_UIPopup_Setting()
    {
        //UIManager.Instance.OpenPopup(SettingsAssetPaths.GetPath(AssetIDs.SETTINGS_ASSETS));
        WindowManager.Instance.OpenWindow<UI_Setting>();
    }
}