using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeScriptableObjectProfile
{
    [MenuItem("Assets/Create/Addressables/Create ProfileSetupObject")]
    public static void CreateMyAsset()
    {
        ProfileSetupObject asset = ScriptableObject.CreateInstance<ProfileSetupObject>();

        string name =
            AssetDatabase.GenerateUniqueAssetPath(
                "Assets/FalconPuzzlePackages/Socials/UserData/Profile/ProfileSetupObject.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}