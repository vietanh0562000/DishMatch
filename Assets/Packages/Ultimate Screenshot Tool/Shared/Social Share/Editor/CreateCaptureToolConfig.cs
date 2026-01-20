using UnityEngine;
using UnityEditor;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool.Share
{
    public class CreateCaptureToolConfig : EditorWindow
    {
        const string folderName = "Configs";

        [MenuItem("Assets/Create/Capture Tool Config")]
        public static CaptureToolConfig Create()
        {
            return CreateWithFileName("NewConfig.asset");
        }

        public static CaptureToolConfig CreateWithFileName(string fileName)
        {
            if (!fileName.EndsWith(".asset", System.StringComparison.InvariantCulture))
                fileName += ".asset";

            CaptureToolConfig asset = ScriptableObject.CreateInstance<CaptureToolConfig>();
            string finalFilePath = AssetDatabase.GenerateUniqueAssetPath(AssetDatabaseDirectory() + "/" + fileName);
            AssetDatabase.CreateAsset(asset, finalFilePath);
            AssetDatabase.SaveAssets();
            Debug.Log("Saved CaptureToolConfig to: " + finalFilePath);
            return asset;
        }

        public static string AssetDatabaseDirectory()
        {
            return NewScriptableObjectPath.AssetDatabaseDirectory(folderName);
        }

    }
}