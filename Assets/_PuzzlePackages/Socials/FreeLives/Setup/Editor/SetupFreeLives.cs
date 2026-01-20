using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeScriptableObjectFreeLives
{
    [MenuItem("Assets/Create/Addressables/Create FreeLivesSetupObject")]
    public static void CreateMyAsset()
    {
        FreeLivesSetupObject asset = ScriptableObject.CreateInstance<FreeLivesSetupObject>();

        string name = AssetDatabase.GenerateUniqueAssetPath("Assets/FalconPuzzlePackages/Socials/FreeLives/FreeLivesSetupObject.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}