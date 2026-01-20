using System.Collections;
using System.Collections.Generic;
using BasePuzzle.PuzzlePackages.Core;
using BasePuzzle.PuzzlePackages.Profile;
using BasePuzzle.PuzzlePackages.Socials.Profile;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProfileSetupObject : ScriptableObject
{
    private const string _PROFILE_ASSET_GROUP = "Profile";
    [SerializeField] private UIPopup_ViewProfile UIPopup_ViewProfile;
    [SerializeField] private UIPopup_Profile UIPopup_Profile;
    [SerializeField] private UIPopupChangeName UIPopup_ChangeName;
    [SerializeField] private UIPopup_ConfirmAction UIPopup_ConfirmAction;

#if UNITY_EDITOR
    [Button]
    public void Setup()
    {
        var pathsContainer = GetPathsContainer();
        pathsContainer.ResetData();

        AddressableHelper.MakeAssetAddressable(UIPopup_ViewProfile, _PROFILE_ASSET_GROUP);
        pathsContainer.Add(AssetCategory.Popup, AssetIDs.PROFILE_VIEW_PROFILE, UIPopup_ViewProfile);

        AddressableHelper.MakeAssetAddressable(UIPopup_Profile, _PROFILE_ASSET_GROUP);
        pathsContainer.Add(AssetCategory.Popup, AssetIDs.PROFILE_POPUP, UIPopup_Profile);

        AddressableHelper.MakeAssetAddressable(UIPopup_ChangeName, _PROFILE_ASSET_GROUP);
        pathsContainer.Add(AssetCategory.Popup, AssetIDs.PROFILE_CHANGE_NAME, UIPopup_ChangeName);

        AddressableHelper.MakeAssetAddressable(UIPopup_ConfirmAction, _PROFILE_ASSET_GROUP);
        pathsContainer.Add(AssetCategory.Popup, AssetIDs.PROFILE_CONFIRM_ACTION, UIPopup_ConfirmAction);

        UnityEditor.EditorUtility.SetDirty(pathsContainer);
        UnityEditor.AssetDatabase.SaveAssets();
    }

    private PathsContainer GetPathsContainer()
    {
        if (!System.IO.Directory.Exists(ProfileAssetPaths.PROFILE_PATH_CONTAINER_FOLDER))
        {
            System.IO.Directory.CreateDirectory(ProfileAssetPaths.PROFILE_PATH_CONTAINER_FOLDER);
        }

        var pathsContainer =
            UnityEditor.AssetDatabase.LoadAssetAtPath<PathsContainer>(ProfileAssetPaths.PROFILE_PATH_CONTAINER_PATH);
        if (pathsContainer == null)
        {
            pathsContainer = CreateInstance<PathsContainer>();
            UnityEditor.AssetDatabase.CreateAsset(pathsContainer, ProfileAssetPaths.PROFILE_PATH_CONTAINER_PATH);
            UnityEditor.EditorUtility.SetDirty(pathsContainer);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
        }

        UnityEditor.Selection.activeObject = pathsContainer;
        return pathsContainer;
    }
#endif
}