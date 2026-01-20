namespace BasePuzzle
{
    using System;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Build;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEngine;

    public static class AddressableBuildEditor
    {
        private static string BUILD_SCRIPT  = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";
        private static string SETTING_ASSET = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

        private static string                   PROFILE_NAME = "Default";
        private static AddressableAssetSettings SETTINGS;

        static void GetSettingsObject(string settingsAsset)
        {
            // This step is optional, you can also use the default settings:
            //
            SETTINGS = AddressableAssetSettingsDefaultObject.Settings;

            /*SETTINGS
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset)
                    as AddressableAssetSettings;*/

            if (SETTINGS == null)
                Debug.LogError($"{settingsAsset} couldn't be found or isn't " +
                               $"a settings object.");
        }

        static void GetDefaultSettingsObject()
        {
            // This step is optional, you can also use the default settings:
            //
            SETTINGS = AddressableAssetSettingsDefaultObject.Settings;

            if (SETTINGS == null)
                Debug.LogError($"Failed to get settings addressable");
        }

        static void SetProfile(string profile)
        {
            string profileId = SETTINGS.profileSettings.GetProfileId(profile);
            if (String.IsNullOrEmpty(profileId))
                Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                                 $"using current profile instead.");
            else
                SETTINGS.activeProfileId = profileId;
        }

        static void SetBuilder(IDataBuilder builder)
        {
            int index = SETTINGS.DataBuilders.IndexOf((ScriptableObject) builder);

            if (index > 0)
                SETTINGS.ActivePlayerDataBuilderIndex = index;
            else
                Debug.LogWarning($"{builder} must be added to the " +
                                 $"DataBuilders list before it can be made " +
                                 $"active. Using last run builder instead.");
        }

        static bool BuildAddressableContent()
        {
            AddressableAssetSettings.CleanPlayerContent();

            AddressablesPlayerBuildResult result = null;
            AddressableAssetSettings.BuildPlayerContent(out result);

            bool success = string.IsNullOrEmpty(result.Error);

            if (!success)
            {
                Debug.LogError("Addressables build error encountered: " + result.Error);
            }
            else
            {
                AssetDatabase.Refresh();
            }

            return success;
        }

        [MenuItem("Window/Asset Management/Addressables/Build Addressables only")]
        public static bool BuildAddressables()
        {
            GetDefaultSettingsObject();

            /*IDataBuilder builderScript
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(BUILD_SCRIPT) as IDataBuilder;

            if (builderScript == null)
            {
                Debug.LogError(BUILD_SCRIPT + " couldn't be found or isn't a build script.");
                return false;
            }

            SetBuilder(builderScript);*/

            return BuildAddressableContent();
        }

        [MenuItem("Window/Asset Management/Addressables/Build Addressables and Player")]
        public static void BuildAddressablesAndPlayer()
        {
            var contentBuildSucceeded = BuildAddressables();

            if (contentBuildSucceeded)
            {
                var options = new BuildPlayerOptions();
                BuildPlayerOptions playerSettings
                    = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(options);

                BuildPipeline.BuildPlayer(playerSettings);
            }
        }
    }
}