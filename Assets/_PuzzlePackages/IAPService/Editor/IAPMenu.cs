using System.IO;
using UnityEditor;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    public class IAPMenu : Editor
    {
        [MenuItem("Puzzle/IAPService/IAP Settings")]
        public static void InAppPurchaseSettings()
        {
            if (!Directory.Exists(ProductSettings.SETTINGS_NAME))
            {
                Directory.CreateDirectory(ProductSettings.SETTINGS_PATH);
            }
            
            var settings = Resources.Load<ProductSettings>(ProductSettings.SETTINGS_NAME);
            if (settings == null)
            {
                settings = CreateInstance<ProductSettings>();
                AssetDatabase.CreateAsset(settings, ProductSettings.SETTINGS_PATH + "/" + ProductSettings.SETTINGS_NAME + ".asset");
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
            }

            Selection.activeObject = settings;
        }
    }
}
