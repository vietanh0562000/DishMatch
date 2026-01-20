namespace Builder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using BasePuzzle;
    using SRDebugger.Editor;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    public class BuildProject
    {
        private static readonly string BUILD_COUNTER_ARG     = "-buildCounter";
        private static readonly string ADDRESSABLE_BUILD_ARG = "-addressableBuild";
        private static readonly string BUILD_TESTING_ARG     = "-buildTesting";
        private static readonly string VERSION_CODE_ARG      = "-versionCode";
        private static readonly string BUNDLE_VER_ARG        = "-bundleVersion";
        
        private static readonly string UNITY_TEST = "UNITY_TEST";
        private static readonly string FALCON_LOG = "ACCOUNT_TEST";

        private static string _buildCounter = "0";
        private static bool   _buildTesting;
        private static bool   _addressableBuild;
        
        private static int    _versionCode;
        private static string _bundleVersion;
        
        public static string[] GetScenePaths()
        {
            List<string> scenes = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];
                if (scene.enabled)
                {
                    scenes.Add(scene.path);
                }
            }

            return scenes.ToArray();
        }
        
        private static void GetAndroidSettings(out int oldCode, out string oldVersion)
        {
            //Available PlayerSettings: https://docs.unity3d.com/ScriptReference/PlayerSettings.Android.html
            
            oldCode    = PlayerSettings.Android.bundleVersionCode;
            oldVersion = PlayerSettings.bundleVersion;
        }
        

        private static void ApplyArguments()
        {
            var args = System.Environment.GetCommandLineArgs();
            _buildCounter = GetArgumentValue(args, BUILD_COUNTER_ARG, "0");
            
            GetAndroidSettings(out int oldCode, out string oldVersion);
            var code = GetArgumentValue(args, VERSION_CODE_ARG, oldCode.ToString());
            int.TryParse(code, out _versionCode);
            
            _bundleVersion = GetArgumentValue(args, BUNDLE_VER_ARG, oldVersion);

            var buildTesting = GetArgumentValue(args, BUILD_TESTING_ARG, "false");
            bool.TryParse(buildTesting, out _buildTesting);
            
            if (_buildTesting)
            {
                ChangeToTestingMode();
            }
            else
            {
                ChangeToReleaseMode();
            } 
            
            PlayerSettings.SplashScreen.showUnityLogo = false;

            var buildAddressable = GetArgumentValue(args, ADDRESSABLE_BUILD_ARG, "false");
            bool.TryParse(buildAddressable, out _addressableBuild);

            if (_addressableBuild)
            {
                BuildAddressable();
            }
        }
        
        public static void ChangeToTestingMode()
        {
            string defineSymbol = string.Empty;
#if UNITY_ANDROID
            defineSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
#elif UNITY_IOS
            defineSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
#endif
            Debug.Log(defineSymbol);
            var split = defineSymbol.Split(';').ToList();
            if (!split.Contains(UNITY_TEST))
            {
                defineSymbol += $";{UNITY_TEST}";
            }
            
            if (!split.Contains(FALCON_LOG))
            {
                defineSymbol += $";{FALCON_LOG}";
            }
            
            Debug.LogWarning(defineSymbol);
            
#if UNITY_ANDROID
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineSymbol);
#elif UNITY_IOS
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineSymbol);
#endif
        }
        
        public static void ChangeToReleaseMode()
        {
            string defineSymbol = string.Empty;
#if UNITY_ANDROID
            defineSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
#elif UNITY_IOS
            defineSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
#endif
            Debug.Log(defineSymbol);

            var split = defineSymbol.Split(';').ToList();
            if (split.Contains(UNITY_TEST) || split.Contains(FALCON_LOG))
            {
                split.RemoveAll(_ => _ == UNITY_TEST);
                split.RemoveAll(_ => _ == FALCON_LOG);

                var newDefine = "";
                foreach (var sym in split)
                {
                    newDefine += sym + ";";
                }

                newDefine = newDefine.Remove(newDefine.Length - 1, 1);
                Debug.LogWarning(newDefine);
                
#if UNITY_ANDROID
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, newDefine);
#elif UNITY_IOS
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, newDefine);
#endif
            }
        }
        
        public static void PreBuildUpdate()
        {
            string linkPath = Application.dataPath + Path.Combine("AddressableAssetsData", "link.xml");
            if (!File.Exists(linkPath))
            {
                string saved = Application.dataPath + Path.Combine("0_Test", "link.xml");
                if (File.Exists(saved))
                {
                    File.Copy(saved, linkPath);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
            }
        }

        public static bool IsUnityTestEnabled()
        {
            string defineSymbol = string.Empty;
#if UNITY_ANDROID
            defineSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
#elif UNITY_IOS
            defineSymbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
#endif
            var split = defineSymbol.Split(';').ToList();
            return split.Contains(UNITY_TEST);
        }

        private static void BuildAddressable()
        {
            if (AddressableBuildEditor.BuildAddressables())
            {
                Debug.Log("--- Build Addressable Succeeded ---");
            }
        }

        private static string GetArgumentValue(string[] args, string paramName, string defaultValue = default)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == paramName && i + 1 < args.Length)
                    return args[i + 1];
            }
            return defaultValue;
        }

        private static string GetFinalPath(string extension)
        {
            string output = Application.dataPath.Replace("Assets", "Builds") + $"/{_buildCounter}";
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }
            
            string extStr = $"{(_buildTesting ? "T" : "R")}-{(_addressableBuild ? "A" : "N")}";
            string value = $"{output}/{KeystoreName}-{_buildCounter}-{_bundleVersion}-{extStr}-{DateTime.Now.ToString("HHmmss")}{extension}";

            return value;
        }
        
        public static void BuildAndroidAPK()
        {
            ApplyArguments();
            PreBuildUpdate();
            
            // APK Expansion: https://docs.unity3d.com/Manual/android-OBBsupport.html
            PlayerSettings.Android.splitApplicationBinary = false;
            
            EditorUserBuildSettings.buildAppBundle   = false;     // disable aab build
            
            ApplyKeySettings();

            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;

            //Get the apk file to be built from the command line argument
            string output = GetFinalPath(@".apk");

            PlayerSettings.Android.bundleVersionCode = _versionCode;
            PlayerSettings.bundleVersion             = _bundleVersion;
            
            AssetDatabase.SaveAssets();
            
            var report = BuildPipeline.BuildPlayer(GetScenePaths(), output, BuildTarget.Android, BuildOptions.CompressWithLz4HC);
            if (report.summary.result != BuildResult.Succeeded)
            {
                EditorApplication.Exit(1);
            }
        }

        public static void BuildAndroidAAB()
        {
            ApplyArguments();
            PreBuildUpdate();
            
            // APK Expansion: https://docs.unity3d.com/Manual/android-OBBsupport.html
            PlayerSettings.Android.splitApplicationBinary = true;
            
            EditorUserBuildSettings.buildAppBundle   = true;      // enable aab build
            
            ApplyKeySettings();

            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;

            //Get the apk file to be built from the command line argument
            string output = GetFinalPath(@".aab");

            PlayerSettings.Android.bundleVersionCode = _versionCode;
            PlayerSettings.bundleVersion             = _bundleVersion;
            
            AssetDatabase.SaveAssets();

            var report = BuildPipeline.BuildPlayer(GetScenePaths(), output, BuildTarget.Android, BuildOptions.CompressWithLz4HC);
            if (report.summary.result != BuildResult.Succeeded)
            {
                EditorApplication.Exit(1);
            }
        }

        public static void ApplyKeySettings()
        {
            //Available PlayerSettings: https://docs.unity3d.com/ScriptReference/PlayerSettings.Android.html
            EditorUserBuildSettings.androidCreateSymbolsZip = false;
            
            //set the other settings from environment variables
            
            var folderToFind = Application.dataPath.Replace("/Assets", "");

            string[] files = Directory.GetFiles(folderToFind, "*.keystore");

            if (files.Length > 0)
            {
                PlayerSettings.Android.keystoreName = files[0];
            }
            else
            {
                PlayerSettings.Android.keystoreName = KeystoreName;
            }
            
            PlayerSettings.Android.keystorePass = KeystorePass;
            PlayerSettings.Android.keyaliasName = KeystoreName;
            PlayerSettings.Android.keyaliasPass = KeystorePass;
        }
        
        private const string KeystoreName = "hole";
        private const string KeystorePass = "123456";
    }
}