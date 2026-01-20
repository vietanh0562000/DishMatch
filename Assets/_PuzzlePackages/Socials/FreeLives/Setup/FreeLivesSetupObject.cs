using BasePuzzle.PuzzlePackages.Core;
using BasePuzzle.PuzzlePackages.Socials.FreeLives;
using PuzzleGames;
using Sirenix.OdinInspector;
using UnityEngine;

public class FreeLivesSetupObject : ScriptableObject
{
    private const string _FREELIVES_ASSET_GROUP = "FreeLives";
    [SerializeField] private UIPopup_Resource_FreeLives UIPopup_Resource_FreeLives;
    [SerializeField] private UIPopup_ShopLives UIPopup_ShopLives;

#if UNITY_EDITOR
    
    [Button]
    public void Setup()
    {
        var pathsContainer = GetPathsContainer();
        pathsContainer.ResetData();

        AddressableHelper.MakeAssetAddressable(UIPopup_Resource_FreeLives, _FREELIVES_ASSET_GROUP);
        pathsContainer.Add(AssetCategory.Popup, AssetIDs.FREELIVES_RESOURCE_ASSETS, UIPopup_Resource_FreeLives);

        AddressableHelper.MakeAssetAddressable(UIPopup_ShopLives, _FREELIVES_ASSET_GROUP);
        pathsContainer.Add(AssetCategory.Popup, AssetIDs.FREELIVES_SHOP_LIVE_ASSETS, UIPopup_ShopLives);

        UnityEditor.EditorUtility.SetDirty(pathsContainer);
        UnityEditor.AssetDatabase.SaveAssets();
    }

    private PathsContainer GetPathsContainer()
    {
        if (!System.IO.Directory.Exists(FreeLivesAssetPaths.FREELIVES_PATH_CONTAINER_FOLDER))
        {
            System.IO.Directory.CreateDirectory(FreeLivesAssetPaths.FREELIVES_PATH_CONTAINER_FOLDER);
        }

        var pathsContainer =
            UnityEditor.AssetDatabase.LoadAssetAtPath<PathsContainer>(FreeLivesAssetPaths
                .FREELIVES_PATH_CONTAINER_PATH);
        if (pathsContainer == null)
        {
            pathsContainer = CreateInstance<PathsContainer>();
            UnityEditor.AssetDatabase.CreateAsset(pathsContainer, FreeLivesAssetPaths.FREELIVES_PATH_CONTAINER_PATH);
            UnityEditor.EditorUtility.SetDirty(pathsContainer);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
        }

        UnityEditor.Selection.activeObject = pathsContainer;
        return pathsContainer;
    }
    
#endif
}