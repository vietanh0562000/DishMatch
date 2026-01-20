using System.Collections.Generic;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class PreviewSettings : ScriptableObject
    {
        [System.Serializable]
        public enum Rotation
        {
            Both,
            Portrait,
            Landscape,
        }

        const string folderName = "PreviewSettings";

        public const string DEFAULT_FILENAME = "Default.asset";

        public PreviewDevice[] previewDevices;
        public bool excludeFolderPathFromDeviceDisplayName = true;

        public bool useScreenshotScript;

        public string saveDirectory;
        public bool saveAsynchronously = true;

        public bool autoUpdate;
        public float autoUpdateDelay = 5.0f;

        public bool safeAreaEnabled;
        public Color32 safeAreaColor = new Color32(255, 0, 0, 128);

        public bool displayInGrid;
        public Rotation rotation = Rotation.Both;

        public float scale = 0.1f;

        public static PreviewSettings Default()
        {
            PreviewSettings previewSettings = null;
#if UNITY_EDITOR
            string separatorString = System.IO.Path.DirectorySeparatorChar.ToString();
            string finalFilePath = AssetDatabaseDirectory() + separatorString + DEFAULT_FILENAME;

            previewSettings = AssetDatabase.LoadAssetAtPath<PreviewSettings>(finalFilePath);
            if (previewSettings != null) return previewSettings;
            previewSettings = Create(DEFAULT_FILENAME);

            Dictionary<string, Resolution> resolutionForName = AdditionalResolutions.All(true, false);
            string[] mobileResolutionNames = AdditionalResolutions.resolutionGroup["Group/App Store/Both Platforms"];

            previewSettings.previewDevices = new PreviewDevice[mobileResolutionNames.Length];
            for (int i = 0; i < mobileResolutionNames.Length; ++i)
            {
                string mobileResolutionName = mobileResolutionNames[i];
                Resolution mobileResolution = resolutionForName[mobileResolutionName];

                PreviewDevice previewDevice = new PreviewDevice();
                previewDevice.deviceName = mobileResolutionName;
                previewDevice.width = mobileResolution.width;
                previewDevice.height = mobileResolution.height;
                previewSettings.previewDevices[i] = previewDevice;
            }
#else
            previewSettings = Create();
#endif

            return previewSettings;
        }

        public static PreviewSettings Create(string persistFileName = null)
        {
            if (!string.IsNullOrEmpty(persistFileName))
            {
#if UNITY_EDITOR
                return CreateWithFileName(persistFileName);
#else
                Debug.LogError("Cannot persist preview settings outside of the editor.");
#endif
            }

            return ScriptableObject.CreateInstance<PreviewSettings>();
        }


#if UNITY_EDITOR
            public static PreviewSettings CreateWithFileName(string fileName)
        {
            if (!fileName.EndsWith(".asset", System.StringComparison.InvariantCulture))
                fileName += ".asset";

            PreviewSettings asset = ScriptableObject.CreateInstance<PreviewSettings>();
            string finalFilePath = AssetDatabase.GenerateUniqueAssetPath(AssetDatabaseDirectory() + "/" + fileName);
            AssetDatabase.CreateAsset(asset, finalFilePath);
            AssetDatabase.SaveAssets();

            Debug.Log("Saved PreviewSettings to: " + finalFilePath);
            return asset;
        }
#endif

        public static string NameForRotation(Rotation rotation)
        {
            switch (rotation)
            {
                case Rotation.Portrait:
                    return "Portrait";
                case Rotation.Landscape:
                    return "Landscape";
                case Rotation.Both:
                    return "Both";
            }

            throw new UnityException("Unhandled rotation in NameForRotation switch statement");
        }

        public static string AssetDatabaseDirectory()
        {
            return NewScriptableObjectPath.AssetDatabaseDirectory(folderName);
        }
    }
}