using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeScriptableObjectSettings
{
    [MenuItem("Assets/Create/Addressables/Create SettingsSetupObject")]
    public static void CreateMyAsset()
    {
        SettingsSetupObject asset = ScriptableObject.CreateInstance<SettingsSetupObject>();

        string name =
            AssetDatabase.GenerateUniqueAssetPath(
                "Assets/FalconPuzzlePackages/Core/Settings/SettingsSetupObject.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}