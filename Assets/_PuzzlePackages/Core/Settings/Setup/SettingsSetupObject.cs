using System.Collections;
using System.Collections.Generic;
using BasePuzzle.PuzzlePackages.Core;
using BasePuzzle.PuzzlePackages.Profile;
using BasePuzzle.PuzzlePackages.Settings;
using Sirenix.OdinInspector;
using UnityEngine;

public class SettingsSetupObject : ScriptableObject
{
    private const string _SETTINGS_ASSET_GROUP = "Settings";
    [SerializeField] private UI_Setting UIPopup_Setting;
    [SerializeField] private UI_Setting_SaveProgress UIPopup_Setting_SaveProgress;
    [SerializeField] private UI_Setting_SaveProgress_Status UIPopup_Setting_SaveProgress_Status;

#if UNITY_EDITOR
    [Button]
    public void Setup()
    {
        var pathsContainer = GetPathsContainer();
        pathsContainer.ResetData();

        AddressableHelper.MakeAssetAddressable(UIPopup_Setting, _SETTINGS_ASSET_GROUP);
        pathsContainer.Add(AssetCategory.Popup, AssetIDs.SETTINGS_ASSETS, UIPopup_Setting);

        AddressableHelper.MakeAssetAddressable(UIPopup_Setting_SaveProgress, _SETTINGS_ASSET_GROUP);
        pathsContainer.Add(AssetCategory.Popup, AssetIDs.SETTINGS_SAVE_PROGRESS_ASSETS, UIPopup_Setting_SaveProgress);

        AddressableHelper.MakeAssetAddressable(UIPopup_Setting_SaveProgress_Status, _SETTINGS_ASSET_GROUP);
        pathsContainer.Add(AssetCategory.Popup, AssetIDs.SETTINGS_SAVE_PROGRESS_STATUS_ASSETS,
            UIPopup_Setting_SaveProgress_Status);

        UnityEditor.EditorUtility.SetDirty(pathsContainer);
        UnityEditor.AssetDatabase.SaveAssets();
    }

    private PathsContainer GetPathsContainer()
    {
        if (!System.IO.Directory.Exists(SettingsAssetPaths.SETTINGS_PATH_CONTAINER_FOLDER))
        {
            System.IO.Directory.CreateDirectory(SettingsAssetPaths.SETTINGS_PATH_CONTAINER_FOLDER);
        }

        var pathsContainer =
            UnityEditor.AssetDatabase.LoadAssetAtPath<PathsContainer>(SettingsAssetPaths.SETTINGS_PATH_CONTAINER_PATH);
        if (pathsContainer == null)
        {
            pathsContainer = CreateInstance<PathsContainer>();
            UnityEditor.AssetDatabase.CreateAsset(pathsContainer, SettingsAssetPaths.SETTINGS_PATH_CONTAINER_PATH);
            UnityEditor.EditorUtility.SetDirty(pathsContainer);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
        }

        UnityEditor.Selection.activeObject = pathsContainer;
        return pathsContainer;
    }
#endif
}