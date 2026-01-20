using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

using TRS.CaptureTool.Extras;
using TRS.CaptureTool.Share;
namespace TRS.CaptureTool
{
    public class CaptureScriptEditor : Editor
    {
        protected const int maxErrorLogs = 50;

        protected enum Tab
        {
            Capture,
            Save,
            Share,
            Settings,
            Size
        }
        protected Tab selectedTab;
        protected Tab[] tabAtIndex;
        protected string[] tabNames;
        protected bool allowsEdit = true;

        protected ReorderableList cameraList;
        protected ReorderableList tempEnabledList;
        protected ReorderableList tempDisabledList;
        protected ReorderableList cutoutAdjustedRectTransformsList;

        protected ReorderableList captureTransformationList;
        protected ReorderableList editTransformationList;

        protected ShareScriptEditor shareScriptEditor;
        protected List<string> errorLogs = new List<string>();

        protected virtual void OnEnable()
        {
            if (target == null)
                return;

            cameraList = new ReorderableList(serializedObject,
                                 serializedObject.FindProperty("cameras"),
                                 true, true, true, true);
            cameraList.AddHeader("Camera");
            cameraList.AddStandardElementCallback();

            tempEnabledList = new ReorderableList(serializedObject,
                                 serializedObject.FindProperty("tempEnabledObjects"),
                                 true, true, true, true);
            tempEnabledList.AddHeader("Temporarily Enabled Objects (Logo overlay, debug info, etc.)");
            tempEnabledList.AddStandardElementCallback();

            tempDisabledList = new ReorderableList(serializedObject,
                     serializedObject.FindProperty("tempDisabledObjects"),
                     true, true, true, true);
            tempDisabledList.AddHeader("Temporarily Disabled Objects (Busy UI, debug info, etc.)");
            tempDisabledList.AddStandardElementCallback();

            cutoutAdjustedRectTransformsList = new ReorderableList(serializedObject,
                     serializedObject.FindProperty("cutoutAdjustedRectTransforms"),
                     true, true, true, true);
            cutoutAdjustedRectTransformsList.AddHeader("Adjusted Rect Transforms");
            cutoutAdjustedRectTransformsList.AddStandardElementCallback();

            captureTransformationList = new TextureTransformationReorderableList(serializedObject, serializedObject.FindProperty("captureTransformations"), "Transformations", true, true, true, true);
            editTransformationList = new TextureTransformationReorderableList(serializedObject, serializedObject.FindProperty("editTransformations"), "Transformations to Apply", true, true, true, true);

            Application.logMessageReceived += HandleLog;
            EditorApplication.playModeStateChanged += PlayModeStateChanged;

            ((CaptureScript)target).RefreshShareScript();
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type != LogType.Log)
            {
                if (errorLogs.Count >= maxErrorLogs)
                    errorLogs.RemoveAt(0);
                errorLogs.Add(logString + "\n" + stackTrace);
            }
        }

        void PlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.EnteredEditMode)
                SelectTabIndex(0);
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            serializedObject.Update();

            GeneralUpdates();
            UpdateSubEditors();

            EditorGUILayout.Space();

            TabSelect();

            //EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

            if (!Application.isPlaying)
            {
                if (Time.timeScale != 1f)
                {
                    if (GUILayout.Button("Reset Time Scale"))
                        Time.timeScale = 1;
                }
            }

            EditorGUILayout.BeginVertical("Box");
            if (selectedTab == Tab.Capture)
                CaptureTab();
            else if (selectedTab == Tab.Save)
                SaveTab();
            else if (selectedTab == Tab.Share)
                ShareTab();
            else if (selectedTab == Tab.Settings)
                SettingsTab();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            GUI.enabled = true;
        }

        void TabSelect()
        {
            int oldSelectedTabIndex = serializedObject.FindProperty("selectedTabIndex").intValue;
            int selectedTabIndex = GUILayout.Toolbar(oldSelectedTabIndex, tabNames);

            if (selectedTabIndex != oldSelectedTabIndex || selectedTab != tabAtIndex[selectedTabIndex])
                SelectTabIndex(selectedTabIndex);
        }

        protected void SelectTab(Tab tab)
        {
            for (int i = 0; i < tabAtIndex.Length; ++i)
            {
                if (tab == tabAtIndex[i])
                {
                    SelectTabIndex(i);
                    return;
                }
            }
        }

        protected void SelectTabIndex(int tabIndex)
        {
            if (serializedObject == null) return;
            if (tabAtIndex == null)
            {
                UpdateSubEditors();
                if (tabAtIndex == null) return;
            }

            serializedObject.FindProperty("selectedTabIndex").intValue = tabIndex;
            selectedTab = tabAtIndex[tabIndex];
            serializedObject.ApplyModifiedProperties();
        }

        #region Tab Stubs
        protected virtual void CaptureTab()
        {
            throw new UnityException("Capture Tab not implemented");
        }

        protected virtual void SaveTab()
        {
            throw new UnityException("Save Tab not implemented");
        }

        protected virtual void ShareTab()
        {
            if (shareScriptEditor != null)
                shareScriptEditor.FullShareUI();
        }

        void SettingsTab()
        {
            CaptureMode();

            SaveObjectDirectorySetting();

            if (!((CaptureScript)target).editorWindowMode)
                HotKeys();

            AudioSettings();
            MouseSettings();
            DontDestroyOnLoadSettings();

            SupportButtons();
        }
        #endregion

        #region Capture Tab Settings
        protected void TempEnabledObjects()
        {
            bool showEnabledObjects = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showEnabledObjects", "Extra Capture List");
            if (showEnabledObjects)
                tempEnabledList.DoLayoutList();
        }

        protected void TempDisabledObjects()
        {
            bool showDisabledObjects = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showDisabledObjects", "Do Not Capture List");
            if (showDisabledObjects)
                tempDisabledList.DoLayoutList();
        }

        protected void Timing(bool allowStopTime)
        {
            bool showTiming = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showTiming", "Timing");
            if (showTiming)
            {
                if (allowStopTime)
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("stopTimeDuringCapture"), new GUIContent("Stop Time to Capture"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("delayBeforeCapture"), new GUIContent("Delay Before Capture", "In Seconds"));

                if (serializedObject.FindProperty("delayBeforeCapture").floatValue > 0)
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("realtimeDelay"), new GUIContent("Realtime Delay", "If false, uses game time which may be scaled."));

                if (Application.isPlaying)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Time Scale Override");
                    serializedObject.FindProperty("timeScaleOverride").floatValue = EditorGUILayout.Slider(serializedObject.FindProperty("timeScaleOverride").floatValue, 0f, 2f);
                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        ((CaptureScript)target).TimeScaleOverrideChanged();
                        EditorUtility.SetDirty(target);
                    }

                    float timeScale = Time.timeScale;
                    if (timeScale != 0)
                    {
                        if (GUILayout.Button("Pause"))
                        {
                            serializedObject.FindProperty("prePauseTimeScale").floatValue = timeScale;
                            Time.timeScale = 0;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Unpause"))
                            Time.timeScale = serializedObject.FindProperty("prePauseTimeScale").floatValue;
                    }

                    bool wouldNotOverride = timeScale == 0 && serializedObject.FindProperty("timeScaleOverride").floatValue == 1f && serializedObject.FindProperty("prePauseTimeScale").floatValue == 1f;
                    if (timeScale != 1 && !wouldNotOverride)
                    {
                        if (GUILayout.Button("Reset to Time Scale 1"))
                        {
                            serializedObject.FindProperty("timeScaleOverride").floatValue = 1f;
                            serializedObject.FindProperty("prePauseTimeScale").floatValue = 1f;
                            Time.timeScale = 1;
                        }
                    }
                }
            }
        }
        #endregion

        #region Save Tab Settings
        protected void OpenFileOrFolderButtons()
        {
            string filePath = serializedObject.FindProperty("lastSaveFilePath").stringValue;
            SerializedProperty captureFileSettingsProperty = serializedObject.FindProperty("fileSettings");
            string saveType = captureFileSettingsProperty.FindPropertyRelative("saveType").stringValue;
            if (string.IsNullOrEmpty(saveType)) Debug.LogError("Why is this empty?");
            saveType = !string.IsNullOrEmpty(saveType) ? saveType.Substring(0, saveType.Length - 1) : ""; // remove extra s
            string folderPath = captureFileSettingsProperty.FindPropertyRelative("cachedEditorDirectory").stringValue;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !string.IsNullOrEmpty(filePath);
            if (GUILayout.Button("View Last " + saveType, GUILayout.MinHeight(40)))
            {
                Application.OpenURL("file:///" + System.Uri.EscapeUriString(filePath));
                Debug.Log("Opening File " + filePath);
            }

            GUI.enabled = originalGUIEnabled && !string.IsNullOrEmpty(folderPath);
            if (GUILayout.Button("View " + saveType + "s Folder", GUILayout.MinHeight(60)))
            {
                Application.OpenURL("file:///" + System.Uri.EscapeUriString(folderPath));
                Debug.Log("Opening Directory " + folderPath);
            }
            GUI.enabled = originalGUIEnabled;
        }
        #endregion

        #region Settings Tab Settings
        protected void CaptureMode()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Capture Mode");
            CaptureMode captureMode = (CaptureMode)serializedObject.FindProperty("__captureMode").intValue;
            serializedObject.FindProperty("__captureMode").intValue = (int)((CaptureMode)EditorGUILayout.EnumPopup(captureMode));
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                ((CaptureScript)target).CaptureModeChanged();
                EditorUtility.SetDirty(target);
            }

            captureMode = (CaptureMode)serializedObject.FindProperty("__captureMode").intValue;

            if (captureMode == TRS.CaptureTool.CaptureMode.Camera)
                EditorGUILayout.HelpBox("Uses render texture(s) to layer and combine camera(s). More complex. More customizable.", MessageType.Info);
#if UNITY_2019_1_OR_NEWER
            else if (captureMode == TRS.CaptureTool.CaptureMode.Async)
                EditorGUILayout.HelpBox("Asynchronously grabs render texture of screen and returns it (to the CPU) when ideal. Macs must have Editor in metal mode. Does not work on iOS or WebGL.", MessageType.Info);
#endif
            else if (captureMode == TRS.CaptureTool.CaptureMode.ScreenCapture)
                EditorGUILayout.HelpBox("Capture exactly what is visible on screen. Simpler to use. Less customizable.\n\nRecommended for use with multiple cameras or if having issues with render textures (like with GUI, UI, or viewports).", MessageType.Info);
            else
                Debug.LogError("Unhandled capture mode: " + captureMode);

#if UNITY_EDITOR_OSX
            EditorGUILayout.HelpBox("Launching certain versions of Unity in Metal mode may cause issues. If you're having issues, please disable Metal mode and use Screen Capture mode.", MessageType.Info);
#endif

            if (captureMode == TRS.CaptureTool.CaptureMode.Camera)
            {
                Cameras();
                UICamera(serializedObject.FindProperty("fileSettings").FindPropertyRelative("saveType").stringValue);
            }
            else if (captureMode == TRS.CaptureTool.CaptureMode.ScreenCapture)
            {
                EditorGUI.BeginChangeCheck();
                ScreenCaptureConfig();
                if (EditorGUI.EndChangeCheck())
                    ScreenCaptureConfigChanged();
            }
        }

        protected virtual void ScreenCaptureConfigChanged() { }

        protected virtual void ScreenCaptureConfig(string additionalContext = "")
        {
            if (((CaptureScript)target).screenCaptureConfig == null)
            {
                ((CaptureScript)target).screenCaptureConfig = new ScreenCaptureConfig();
                EditorUtility.SetDirty(target);
                return;
            }

            string useAltScreenCaptureTooltip = "Enables StereoScreenCaptureMode and super sized texture scaling. " + additionalContext + "Scaling is only necessary if you want extremely large screenshots or get a warning similar to: 'GameView reduced to a reasonable size for this system'.";
            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("screenCaptureConfig").FindPropertyRelative("useAltScreenCapture"), new GUIContent("Use Alt Screen Capture", useAltScreenCaptureTooltip));

            if (serializedObject.FindProperty("screenCaptureConfig").FindPropertyRelative("useAltScreenCapture").boolValue)
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("screenCaptureConfig").FindPropertyRelative("altScreenCaptureModeScale"), new GUIContent("Scale"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("screenCaptureConfig").FindPropertyRelative("useStereoScreenCaptureMode"));
                if (serializedObject.FindProperty("screenCaptureConfig").FindPropertyRelative("useStereoScreenCaptureMode").boolValue)
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("screenCaptureConfig").FindPropertyRelative("stereoScreenCaptureMode"));

                if (serializedObject.FindProperty("screenCaptureConfig").FindPropertyRelative("altScreenCaptureModeScale").intValue != 1 && serializedObject.FindProperty("screenCaptureConfig").FindPropertyRelative("useStereoScreenCaptureMode").boolValue)
                    EditorGUILayout.HelpBox("Due to the Unity API, only a scalue value or a stereo screen capture mode may be set. The stereo screen capture mode will be used.", MessageType.Info);
            }
        }

        protected void Cameras()
        {
            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("captureAllCameras"));
            bool allCameras = serializedObject.FindProperty("captureAllCameras").boolValue;
            if (allCameras) return;

            bool showCameraList = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showCameraList", "Cameras");
            if (showCameraList)
            {
                EditorGUI.BeginChangeCheck();
                cameraList.DoLayoutList();
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    ((CaptureScript)target).AnyCameraChanged();
                    EditorUtility.SetDirty(target);
                }

                if (GUILayout.Button("Add All Active Cameras"))
                {
                    ((CaptureScript)target).AddAllActiveCameras();
                    EditorUtility.SetDirty(target);
                }
            }
        }

        protected void Background(bool withFoldout = false)
        {
            bool showBackground = true;
            if (withFoldout)
                showBackground = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showBackground", "Background");
            if (showBackground)
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("overrideBackground"));

                if (serializedObject.FindProperty("overrideBackground").boolValue)
                {
                    EditorGUI.BeginChangeCheck();
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundCamera"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        ((CaptureScript)target).BackgroundCameraChanged();
                        EditorUtility.SetDirty(target);
                    }

                    if (!serializedObject.FindProperty("backgroundCameraSelected").boolValue)
                        EditorGUILayout.HelpBox("Currently selected camera is the camera with lowest depth. It may not be right. (Select a camera to dismiss this message.)", MessageType.Info);

                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundColor"));
                }
            }
        }

        protected void UICamera(string saveType)
        {
            bool showUICamera = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showUICamera", "UI Camera");
            if (showUICamera)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Autoswitch Render Mode");
                serializedObject.FindProperty("autoSwitchRenderMode").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("autoSwitchRenderMode").boolValue);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("Recommended only if using overlay canvases. Switches canvases from overlay mode to camera mode during " + saveType.ToLower() + ".", MessageType.Info);

                if (serializedObject.FindProperty("autoSwitchRenderMode").boolValue)
                {
                    EditorGUI.BeginChangeCheck();
                    Camera currentUICamera = serializedObject.FindProperty("uiCamera").objectReferenceValue as Camera;
                    serializedObject.FindProperty("uiCamera").objectReferenceValue = EditorGUILayout.ObjectField("UI Camera", currentUICamera, typeof(Camera), true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        ((CaptureScript)target).UICameraChanged();
                        EditorUtility.SetDirty(target);
                    }

                    string uiCameraString = "The camera to display overlay UI on. A built-in camera with the preferred settings is used by default. However, additional cameras does result in slower rendering, so this option may be changed (to the Main Camera for instance) for optimization purposes.";
                    EditorGUILayout.HelpBox(uiCameraString, MessageType.Info);

                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("overridePlaneDistance"));
                    if (serializedObject.FindProperty("overridePlaneDistance").boolValue)
                    {
                        CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("planeDistanceOverride"));
                        EditorGUILayout.HelpBox("Plane distance to use for the canvas during " + saveType.ToLower() + ". Leave as 0 to keep the default value.", MessageType.Info);
                    }
                    else
                        EditorGUILayout.HelpBox("Plane distance to use for the canvases. No need to override if using default UI camera.", MessageType.Info);

                    if (GUILayout.Button("Refresh Canvas List", GUILayout.MinHeight(20)))
                    {
                        ((CaptureScript)target).RefeshCanvasList();
                        EditorUtility.SetDirty(target);
                    }
                    EditorGUILayout.HelpBox("The canvas list used by auto switch render mode may become out of date if new canvases are added. Refresh to get a new list.", MessageType.Info);
                }
            }
        }

        protected static string RawSaveObjectAssetRelativeDirectory()
        {
            if (!PlayerPrefs.HasKey(NewScriptableObjectPath.TRS_SAVE_SCRIPTABLE_OBJECT_DIRECTORY_KEY))
                PlayerPrefs.SetString(NewScriptableObjectPath.TRS_SAVE_SCRIPTABLE_OBJECT_DIRECTORY_KEY, NewScriptableObjectPath.defaultDirectory);
            return PlayerPrefs.GetString(NewScriptableObjectPath.TRS_SAVE_SCRIPTABLE_OBJECT_DIRECTORY_KEY);
        }

        protected virtual void SaveObjectDirectorySetting()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string tooltip = "The folder, relative to the assets folder, where newly created scriptable objects such as configs, palettes, or settings will be created.";
            string newAssetsRelativeDirectory = EditorGUILayout.TextField(new GUIContent("New Scriptable Object Folder", tooltip), RawSaveObjectAssetRelativeDirectory());
            if (EditorGUI.EndChangeCheck())
                PlayerPrefs.SetString(NewScriptableObjectPath.TRS_SAVE_SCRIPTABLE_OBJECT_DIRECTORY_KEY, newAssetsRelativeDirectory);

            if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
            {
                EditorGUILayout.EndHorizontal();

                string panelTitle = "New Scriptable Object Folder";
                string defaultPath = Application.dataPath;
                string separatorString = System.IO.Path.DirectorySeparatorChar.ToString();
                string saveObjectFolderInNativeFormat = defaultPath + separatorString + string.Join(separatorString, newAssetsRelativeDirectory.Split('/'));
                string newDirectory = EditorUtility.SaveFolderPanel(panelTitle, saveObjectFolderInNativeFormat, defaultPath);
                if (newDirectory.Length > 0)
                {
                    string newObjectFolder = newDirectory.Replace(defaultPath, "");
                    PlayerPrefs.SetString(NewScriptableObjectPath.TRS_SAVE_SCRIPTABLE_OBJECT_DIRECTORY_KEY, newObjectFolder);
                }
            }
            else
                EditorGUILayout.EndHorizontal();
        }

        protected virtual void HotKeys()
        {
            throw new UnityException("HotKeys must be overwritten by subclass");
        }

        protected virtual void AudioSettings()
        {
            throw new UnityException("AudioSettings must be overwritten by subclass");
        }

        protected void CutoutSetting()
        {
            bool showCutout = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showCutout", "Cutout");
            if (showCutout)
            {
                GUIContent useCutoutContent = new GUIContent("Use Cutout", CutoutTooltip());

                EditorGUI.BeginChangeCheck();
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("useCutout"), useCutoutContent);
                bool useCutout = serializedObject.FindProperty("useCutout").boolValue;
                if (EditorGUI.EndChangeCheck())
                {
                    if (!useCutout)
                    {
                        if (((CaptureScript)target).cutoutScript != null)
                            ((CaptureScript)target).cutoutScript.preview = false;
                        EditorUtility.SetDirty(target);
                    }
                }

                if (useCutout)
                {
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("cutoutScript"));
                    cutoutAdjustedRectTransformsList.DoLayoutList();
                    EditorGUILayout.HelpBox("Select transforms that should be adjusted to fit within the cutout area (for example logos, title text, or other overlays). This effect is done by temporarily parenting the rect transform within the cutout, so only the top level rect transform is needed.", MessageType.Info);
                }
            }
        }

        protected virtual string CutoutTooltip()
        {
            return "";
        }

        protected void MouseSettings()
        {
            bool showMouseSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showMouseSettings", "Mouse Settings");
            if (showMouseSettings && !((CaptureScript)target).editorWindowMode)
            {
                EditorGUI.BeginChangeCheck();
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("showInGameMouse"));
                if (serializedObject.FindProperty("showInGameMouse").boolValue)
                {
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("mouseFollowScript"), new GUIContent("Mouse Type"));
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("hideOriginalMouse"));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.FindProperty("hideOriginalMouse").boolValue &= serializedObject.FindProperty("showInGameMouse").boolValue;
                    serializedObject.ApplyModifiedProperties();
                    ((CaptureScript)target).UpdateMouse();
                    EditorUtility.SetDirty(target);
                }
            }
        }

        protected void DontDestroyOnLoadSettings()
        {
            bool showDontDestroyOnLoadSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showDontDestroyOnLoadSettings", "DontDestroyOnLoad Settings");
            if (showDontDestroyOnLoadSettings && !((CaptureScript)target).editorWindowMode)
            {
                bool originalGuiEnabled = GUI.enabled;
                if (Application.isPlaying && serializedObject.FindProperty("dontDestroyOnLoad").boolValue)
                    GUI.enabled = false;

                EditorGUI.BeginChangeCheck();
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("dontDestroyOnLoad"), new GUIContent("DontDestroyOnLoad"));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    ((CaptureScript)target).UpdateDontDestroyOnLoad();
                    EditorUtility.SetDirty(target);
                }
                GUI.enabled = originalGuiEnabled;

                if (serializedObject.FindProperty("dontDestroyOnLoad").boolValue)
                {
                    int oldValue = ((CaptureScript)target).GetMaxInstances();
                    int newValue = EditorGUILayout.IntField("Max Instances", oldValue);
                    if (newValue != oldValue)
                    {
                        ((CaptureScript)target).SetMaxInstances(newValue);
                        EditorUtility.SetDirty(target);
                    }

                    string fullMaxInstancesText = "Returning to the initial scene that creates this gameobject would normally cause multiple tools to be created. This avoids that by limiting the max number of instances available. Likely you only want one instance of the tool.";
                    fullMaxInstancesText += "\n\nIf you do have multiple instances in the scene, be sure to give them different hotkeys to avoid issues with multiple tools capturing at the same time.";
                    EditorGUILayout.HelpBox(fullMaxInstancesText, MessageType.Info);

                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("autoUpdateCameras"));
                    if (serializedObject.FindProperty("autoUpdateCameras").boolValue)
                    {
                        string buttonName = serializedObject.FindProperty("autoUpdateCamerasByTag").boolValue ? "Find Cameras by Tag" : "Find Cameras by Name";
                        if (GUILayout.Button(buttonName))
                        {
                            bool original = serializedObject.FindProperty("autoUpdateCamerasByTag").boolValue;
                            serializedObject.FindProperty("autoUpdateCamerasByTag").boolValue = !serializedObject.FindProperty("autoUpdateCamerasByTag").boolValue;
                            bool newNewValue = serializedObject.FindProperty("autoUpdateCamerasByTag").boolValue;
                            serializedObject.ApplyModifiedProperties();
                            ((CaptureScript)target).AnyCameraChanged();
                            EditorUtility.SetDirty(target);
                        }
                    }

                    EditorGUILayout.HelpBox("Automatically finds the replacement cameras after a scene transition. To identify the replacement camera, select the proper option and have the corresponding cameras share the same unique name or tag.", MessageType.Info);
                }
            }
        }

        protected void SupportButtons()
        {
            bool showSupportSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSupportSettings", "Support");
            if (showSupportSettings)
            {
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
                bool originalGUIEnabled = GUI.enabled;
                GUI.enabled &= errorLogs.Count > 0;
                if (GUILayout.Button("Upload Editor Error Logs", GUILayout.MinHeight(25)))
                {
                    serializedObject.FindProperty("uploadingEditorLogs").boolValue = true;
                    APIShare.UploadToFileIO(string.Join("\n", errorLogs.ToArray()), "1w", (resultUrl) =>
                    {
                        serializedObject.FindProperty("editorLogsUrl").stringValue = resultUrl;
                        serializedObject.FindProperty("uploadingEditorLogs").boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                GUI.enabled = originalGUIEnabled;

                if (!string.IsNullOrEmpty(serializedObject.FindProperty("editorLogsUrl").stringValue))
                {
                    if (GUILayout.Button("Copy Editor Error Logs Link", GUILayout.MinHeight(25)))
                    {
                        EditorGUIUtility.systemCopyBuffer = serializedObject.FindProperty("editorLogsUrl").stringValue;
                    }
                }

                bool uploadingEditorLogs = serializedObject.FindProperty("uploadingEditorLogs").boolValue;
                string editorLogsUrl = serializedObject.FindProperty("editorLogsUrl").stringValue;

                if (uploadingEditorLogs || !string.IsNullOrEmpty(editorLogsUrl))
                    EditorGUILayout.LabelField("Editor Logs Upload Url: " + (uploadingEditorLogs ? "Uploading..." : editorLogsUrl));
#endif
                if (GUILayout.Button("Email", GUILayout.MinHeight(25)))
                {
                    string subject = "Support";
                    if (ToolInfo.isScreenshotTool && ToolInfo.isGifTool)
                        subject = "Ultimate Gif & Screenshot Tool Support";
                    else if (ToolInfo.isGifTool)
                        subject = "Ultimate Gif Tool Support";
                    else if (ToolInfo.isScreenshotTool)
                        subject = "Ultimate Screenshot Tool Support";
                    string body = "Hey Jacob,\n\n\n" + DebugInformation(true, editorLogsUrl);

                    WebShare.ShareByEmail("", body, "jacob@tangledrealitystudios.com", subject);
                }

                //if (GUILayout.Button("Twitter", GUILayout.MinHeight(25)))
                //    WebShare.ShareToTwitter(editorLogsUrl, "@tangled_reality " + DebugInformation(false));
                if (GUILayout.Button("Discord", GUILayout.MinHeight(25)))
                    Application.OpenURL("https://discord.gg/nFuptUZ");
                //if (GUILayout.Button("Reddit", GUILayout.MinHeight(25)))
                //    WebShare.ShareToReddit("tangledreality", editorLogsUrl);
                EditorGUILayout.HelpBox("If you have any questions, comments, or requests, please feel free to reach out. Email is preferred. I try to respond within a couple days.", MessageType.Info);

                if (GUILayout.Button("Rate", GUILayout.MinHeight(25)))
                {
                    if (ToolInfo.isScreenshotTool && ToolInfo.isGifTool)
                        Application.OpenURL("https://assetstore.unity.com/packages/slug/123463");
                    else if (ToolInfo.isGifTool)
                        Application.OpenURL("https://assetstore.unity.com/packages/slug/123460");
                    else if (ToolInfo.isScreenshotTool)
                        Application.OpenURL("https://assetstore.unity.com/packages/slug/119675");
                }
                //if (GUILayout.Button("Donate", GUILayout.MinHeight(25)))
                //    Application.OpenURL("https://www.paypal.com/donate/?token=SWSWJKlDjPzKBTe55JGLGtdOchLRHAbJprXXhxpLA_HVRJUPmkoaJdk0NqIy5XeqDod9h0&country.x=US&locale.x=US");
                if (GUILayout.Button("Other Assets", GUILayout.MinHeight(25)))
                    Application.OpenURL("https://assetstore.unity.com/publishers/35845");
                EditorGUILayout.HelpBox("If you're happy with this asset, please give it a heart and 5 stars! You can also check out my other assets.", MessageType.Info);
            }
        }

        string DebugInformation(bool longform, string editorLogsUrl = "")
        {
            string os = SystemInfo.operatingSystem;
            if (!longform)
                os = os.Substring(0, os.IndexOf(" ", System.StringComparison.Ordinal));

            string debugInfo = "";
            if (longform)
            {
                debugInfo += "Debug Information:";
                if (!string.IsNullOrEmpty(editorLogsUrl))
                    debugInfo += editorLogsUrl;
                debugInfo += "\nModel: " + SystemInfo.deviceModel;
                debugInfo += "\nOS: ";
            }
            debugInfo += os;
            debugInfo += (longform ? "\n" : " ") + "Unity: " + Application.unityVersion;

            if (ToolInfo.isGifTool)
                debugInfo += (longform ? "\n" : " ") + "UGTv" + ToolInfo.GifVersion();
            if (ToolInfo.isScreenshotTool)
                debugInfo += (longform ? "\n" : " ") + "USTv" + ToolInfo.ScreenshotVersion();

            if (!longform && !string.IsNullOrEmpty(editorLogsUrl))
                debugInfo += editorLogsUrl;
            return debugInfo;
        }

        public static System.Type FindType(string qualifiedTypeName)
        {
            System.Type t = System.Type.GetType(qualifiedTypeName);

            if (t != null)
            {
                return t;
            }
            else
            {
                foreach (System.Reflection.Assembly asm in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = asm.GetType(qualifiedTypeName);
                    if (t != null)
                        return t;
                }
                return null;
            }
        }

        void SetEditorsLogUrl(string newEditorsLogUrl)
        {
            serializedObject.FindProperty("editorLogsUrl").stringValue = newEditorsLogUrl;
            serializedObject.FindProperty("uploadingEditorLogs").boolValue = false;
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region Helpers
        protected virtual void GeneralUpdates()
        {

        }

        protected virtual void UpdateSubEditors()
        {
            ShareScriptEditor oldShareScriptEditor = shareScriptEditor;
            if (((CaptureScript)target).weakShareScript != null && ((CaptureScript)target).weakShareScript.IsAlive)
            {
                if (shareScriptEditor == null)
                    shareScriptEditor = (ShareScriptEditor)Editor.CreateEditor(((CaptureScript)target).weakShareScript.Target as ShareScript);
            }
            else
                shareScriptEditor = null;

            if (tabNames == null || shareScriptEditor != oldShareScriptEditor)
                UpdateTabs();
        }

        protected virtual void UpdateTabs()
        {
            int numTabs = (int)Tab.Size;
            bool includeShare = shareScriptEditor != null;
            if (!includeShare)
                --numTabs;

            int arrayIndex = 0;
            tabNames = new string[numTabs];
            tabAtIndex = new Tab[numTabs];
            for (int i = 0; i < (int)Tab.Size; ++i)
            {
                Tab tab = (Tab)i;
                if (tab != Tab.Share || includeShare)
                {
                    tabNames[arrayIndex] = tab.ToString();
                    if (tab == Tab.Save && allowsEdit)
                        tabNames[arrayIndex] = "Edit & Save";
                    tabAtIndex[arrayIndex] = tab;
                    ++arrayIndex;
                }
            }
        }
        #endregion
    }
}