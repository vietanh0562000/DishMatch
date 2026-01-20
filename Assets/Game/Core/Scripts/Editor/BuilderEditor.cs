namespace BasePuzzle
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Builder;
    using Unity.CodeEditor;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public class BuilderEditor : EditorWindow
    {
        [MenuItem("File/Build Settings Custom... %#j", priority = 205)]
        static void OpenWindow()
        {
            var window = GetWindow<BuilderEditor>();
            window.titleContent.text = "Builder";
            window.minSize           = new Vector2(400, 400);
            window.maxSize           = window.minSize;
        }
        
        [MenuItem("Utils/Build Settings Custom... %j", priority = 205)]
        static void OpenBuilder()
        {
            OpenWindow();
        }

        private const string LastAndroidBuildVersionKey = "BuilderEditor.AndroidBuildVersion";
        private const string LastIOSBuildNumberKey      = "BuilderEditor.IOSBuildNumber";
        private const string LastAddressableBuildKey    = "BuilderEditor.AddressableBuild";


        private long   _lastAddressableBuild;
        private int    _androidBuildCode;
        private string _androidBuildVersion;
        private bool   _androidBuildBundle;
        
        private string _iosBuildNumber;
        
        private bool _isBuildTest;

        private void OnEnable()
        {
            var lastBuildAddressable = EditorPrefs.GetString(LastAddressableBuildKey, string.Empty);
            long.TryParse(lastBuildAddressable, out _lastAddressableBuild);

            _androidBuildCode    = PlayerSettings.Android.bundleVersionCode;
            _androidBuildVersion = Application.version;
            _androidBuildBundle  = EditorUserBuildSettings.buildAppBundle;

            _isBuildTest    = BuildProject.IsUnityTestEnabled();
            _iosBuildNumber = PlayerSettings.iOS.buildNumber;
        }

        private void DrawAddressable()
        {
            if (_lastAddressableBuild > 0)
            {
                var now = DateTime.Now - DateTimeUtils.BASE_DATE;
                
                long   gap = (long) now.TotalSeconds - _lastAddressableBuild;
                string gapTime;
                if (gap <= 60)
                {
                    gapTime = "Just now";
                }
                else
                {
                    gapTime = gap.TimestampToText() + " ago";
                }
                
                EditorGUILayout.LabelField($"(i) Last Addressable build: {gapTime}", WindowStyles.TextColorCustomSize(WindowStyles.Yellow, 11));
            }
            
            if (!ExistAddressable())
            {
                EditorGUILayout.LabelField("Addressable Build is NOT available", WindowStyles.BoldTextColor(WindowStyles.Red));
            }
        }

        private bool ExistAddressable()
        {
            var finalPath = GetAddressablePath();

            if (Directory.Exists(finalPath))
            {
                var direct = new DirectoryInfo(finalPath);
                return direct.GetFiles().Length > 0;
            }

            return false;
        }

        private string GetAddressablePath()
        {
            var path = Application.dataPath.Replace("Assets", Path.Combine("Library","com.unity.addressables","aa","_PLATFORM","_PLATFORM"));
            string p = "Android";
#if UNITY_IOS
            p = "iOS";
#endif
            var finalPath = path.Replace("_PLATFORM", p);
            return finalPath;
        }

        private void OnGUI()
        {
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.LabelField("Editor is playing...", WindowStyles.TextCenter);
                return;
            }
            
            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.LabelField("Editor is compiling...", WindowStyles.TextCenter);
                return;
            }
            
            DrawAddressable();

            bool testing = false;
            if (BuildProject.IsUnityTestEnabled())
            {
                testing = true;
                EditorGUILayout.LabelField($"UNITY_TEST enabled", WindowStyles.BoldTextColor(WindowStyles.Red));
            }

            if (testing)
            {
                EditorGUILayout.Space(10);
            }
            
            GUILayout.Label("Android", WindowStyles.BoldTextColor(WindowStyles.Green));
            EditorGUI.indentLevel++;
            _androidBuildVersion = EditorGUILayout.TextField("Android Build Version", _androidBuildVersion);
            _androidBuildCode    = EditorGUILayout.IntField("Android Build Code", _androidBuildCode);
            
            EditorGUILayout.LabelField($"(i) Last build version: {EditorPrefs.GetString(LastAndroidBuildVersionKey, _androidBuildVersion)}", WindowStyles.TextColorCustomSize(WindowStyles.Yellow, 11));

            _androidBuildBundle = EditorGUILayout.Toggle("Build AAB", _androidBuildBundle);
            
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(5);
            GUILayout.Label("iOS", WindowStyles.BoldTextColor(WindowStyles.Green));
            EditorGUI.indentLevel++;
            _iosBuildNumber = EditorGUILayout.TextField("iOS Build Number", _iosBuildNumber);
            EditorGUILayout.LabelField($"(i) Last build number: {EditorPrefs.GetString(LastIOSBuildNumberKey, _iosBuildNumber)}", WindowStyles.TextColorCustomSize(WindowStyles.Yellow, 11));

            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space(15);
            GUILayout.Label("Configuration", WindowStyles.BoldTextColor(WindowStyles.Green));
            EditorGUI.indentLevel++;
            
            _isBuildTest   = EditorGUILayout.Toggle("Build Test", _isBuildTest);
            if (_isBuildTest)
            {
                EditorGUILayout.LabelField("(i) Application will have FPS display, log and SRDebug", WindowStyles.TextColorCustomSize(WindowStyles.Blue, 11));
            }
            else
            {
                EditorGUILayout.LabelField("(i) All testing features will be removed", WindowStyles.TextColorCustomSize(WindowStyles.Blue, 11));
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Open Build Settings", GUILayout.Width(120)))
            {
#if UNITY_6000_0_OR_NEWER
                EditorApplication.ExecuteMenuItem(@"File/Build Profiles");
#else
                EditorApplication.ExecuteMenuItem(@"File/Build Settings...");
#endif
            }
            EditorGUILayout.Space(5);
            
            if (GUILayout.Button("Open Addressable", GUILayout.Width(120)))
            {
                EditorApplication.ExecuteMenuItem(@"Window/Asset Management/Addressables/Groups");
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Open Save Game", GUILayout.Width(120)))
            {
                EditorApplication.ExecuteMenuItem(@"Window/Save Game Pro/Settings");
            }
            EditorGUILayout.Space(5);
            
            if (GUILayout.Button("Open Addressable Build", GUILayout.Width(200)))
            {
                if (ExistAddressable())
                {
                    var finalPath = GetAddressablePath();
                    EditorUtility.RevealInFinder(finalPath);
                }
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel--;
            
            
            EditorGUILayout.Space(15);
            EditorGUILayout.BeginHorizontal();
            
            void UpdateConfigs()
            {
                ApplyBuildConfig();
                UpdateTesting();
            }
            
            if (GUILayout.Button("Update Config Only", GUILayout.Width(195), GUILayout.Height(50)))
            {
                UpdateConfigs();
                BuildAddressable();
                AssetDatabase.SaveAssets();
            }
            
            if (GUILayout.Button("Build", GUILayout.Width(195), GUILayout.Height(50)))
            {
                string message = _isBuildTest ? "Build with Testing?" : "Build Release";
                if (EditorUtility.DisplayDialog("Info", message, "OK", "Cancel"))
                {
                    UpdateTesting();
                    BuildProject.PreBuildUpdate();
                    Build();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void UpdateTesting()
        {
            if (_isBuildTest)
            {
                BuildProject.ChangeToTestingMode();
            }
            else
            {
                BuildProject.ChangeToReleaseMode();
            }
                
            PlayerSettings.SplashScreen.showUnityLogo = false;
        }

        

        private void BuildAddressable()
        {
            if (_lastAddressableBuild > 0)
            {
                var now = DateTime.Now - DateTimeUtils.BASE_DATE;
                
                long   gap     = (long) now.TotalSeconds - _lastAddressableBuild;
                string gapTime = gap.TimestampToText() + " ago";
                
                if (EditorUtility.DisplayDialog("Build Addressable", $"Addressable was built {gapTime}. Still rebuild?", "OK", "Cancel"))
                {
                    ActualAddressableBuild();
                }
            }
            else
            {
                ActualAddressableBuild();
            }
        }

        private void ActualAddressableBuild()
        {
            if (AddressableBuildEditor.BuildAddressables())
            {
                var next = DateTime.Now - DateTimeUtils.BASE_DATE;
                _lastAddressableBuild = (long) next.TotalSeconds;
                EditorPrefs.SetString(LastAddressableBuildKey, _lastAddressableBuild.ToString());
                EditorUtility.DisplayDialog("Info", "Build Addressable Succeeded", "OK");
            }
        }
        
        private void ApplyBuildConfig()
        {
#if UNITY_ANDROID
            GeneralAndroidSettings();
            if (!_androidBuildBundle)
            {
                PlayerSettings.Android.splitApplicationBinary = false;
                PlayerSettings.Android.targetArchitectures  = AndroidArchitecture.ARMv7;
            }
            else
            {
                PlayerSettings.Android.splitApplicationBinary = true;
                PlayerSettings.Android.targetArchitectures  = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            }
#elif UNITY_IOS
            EditorUserBuildSettings.buildAppBundle = true;
            PlayerSettings.iOS.buildNumber         = _iosBuildNumber;
#endif
            AssetDatabase.SaveAssets();
        }

        private void Build()
        {
#if UNITY_ANDROID
            BuildAndroid();
#elif UNITY_IOS
            BuildIOS();
#endif
        }
        
        private void BuildAndroid()
        {
            GeneralAndroidSettings();

            string output = Application.dataPath.Replace("Assets", "Builds") + $"/{_androidBuildVersion}-{DateTime.Now.ToString("HHmmss")}";

            if (!_androidBuildBundle)
            {
                PlayerSettings.Android.splitApplicationBinary =  false;
                PlayerSettings.Android.targetArchitectures  =  AndroidArchitecture.ARMv7;
                output                                      += ".apk";
            }
            else
            {
                
                PlayerSettings.Android.splitApplicationBinary =  true;
                PlayerSettings.Android.targetArchitectures  =  AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
                output                                      += ".aab";
            }

            Debug.Log(output);

            var options = BuildOptions.CompressWithLz4HC;

            // aab
            if (_androidBuildBundle)
            {
                options |= BuildOptions.AutoRunPlayer;
            }
            
            AssetDatabase.SaveAssets();
            var report = BuildPipeline.BuildPlayer(BuildProject.GetScenePaths(), output, BuildTarget.Android, options);
            if (report.summary.result == BuildResult.Succeeded)
            {
                EditorPrefs.SetString(LastAndroidBuildVersionKey, _androidBuildVersion);
                EditorUtility.RevealInFinder(output);
            }
        }
        
        private void GeneralAndroidSettings()
        {
            EditorUserBuildSettings.buildAppBundle = _androidBuildBundle;

            PlayerSettings.bundleVersion             = _androidBuildVersion;
            PlayerSettings.Android.bundleVersionCode = _androidBuildCode;
            
            BuildProject.ApplyKeySettings();
        }
        
        private void BuildIOS()
        {
            string output = Application.dataPath.Replace("Assets", "Builds");
            string path = OpenOrCreateNewIOSPathBuild(output);
            if (string.IsNullOrEmpty(path)) return;

            EditorUserBuildSettings.buildAppBundle = true;

            PlayerSettings.iOS.buildNumber         = _iosBuildNumber;

            Debug.Log(path);
            
            AssetDatabase.SaveAssets();
            var report = BuildPipeline.BuildPlayer(BuildProject.GetScenePaths(), path, BuildTarget.iOS, BuildOptions.CompressWithLz4HC);
            if (report.summary.result == BuildResult.Succeeded)
            {
                EditorPrefs.SetString(LastIOSBuildNumberKey, _iosBuildNumber);
                EditorUtility.RevealInFinder(path);
            }
        }

        private string OpenOrCreateNewIOSPathBuild(string startPath)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = "osascript"; // macOS AppleScript
            psi.Arguments = $"-e 'set myFolder to choose folder with prompt \"Select a folder:\" default location (POSIX file \"{startPath}\")' " +
                            $"-e 'POSIX path of myFolder' ";
            
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi);
            process.WaitForExit();

            return process.StandardOutput.ReadToEnd().Trim();
        }
    }
}