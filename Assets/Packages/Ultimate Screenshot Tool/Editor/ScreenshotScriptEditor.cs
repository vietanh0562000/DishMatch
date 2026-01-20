using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using System.Collections.Generic;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(ScreenshotScript))]
    public class ScreenshotScriptEditor : CaptureScriptEditor
    {
        const float MAX_HEIGHT = 250.0F;

        ReorderableList resolutionList;
        ReorderableList screenshotResolutionTransformationList;
        ReorderableList liveFrameList;
        ReorderableList screenshotResolutionLiveFrameList;

        ScreenshotSubComponentScriptEditor screenshotSeriesScriptEditor;
        ScreenshotSubComponentScriptEditor multiLangScreenshotScriptEditor;
        ScreenshotSubComponentScriptEditor screenshotBurstScriptEditor;
        ScreenshotSubComponentScriptEditor cutoutScreenshotSetScriptEditor;
        ScreenshotSubComponentScriptEditor gameObjectScreenshotScriptEditor;
        ScreenshotSubComponentScriptEditor multiCameraScreenshotScriptEditor;
        ScreenshotSubComponentScriptEditor multiCameraGameObjectScreenshotScriptEditor;

        protected override void OnEnable()
        {
            if (target == null)
                return;

            base.OnEnable();

            CreateScreenshotResolutionList();
            screenshotResolutionTransformationList = new ScreenshotResolutionTransformationReorderableList(serializedObject, serializedObject.FindProperty("screenshotResolutionTransformations"));
            liveFrameList = new LiveFrameReorderableList(serializedObject, serializedObject.FindProperty("liveFrames"));
            screenshotResolutionLiveFrameList = new ScreenshotResolutionLiveFrameReorderableList(serializedObject, serializedObject.FindProperty("screenshotResolutionLiveFrames"));

            ((ScreenshotScript)target).RefreshSubComponents();
        }

        #region Tabs
        protected override void CaptureTab()
        {
            Resolutions();

            ResolutionSettings();

            bool showCaptureTransformations = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showCaptureTransformations", "Capture Transformations");
            if (showCaptureTransformations)
                captureTransformationList.DoLayoutList();

            bool showScreenshotResolutionTransformations = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showScreenshotResolutionTransformations", "Screenshot Resolution Transformations");
            if (showScreenshotResolutionTransformations)
            {
                screenshotResolutionTransformationList.DoLayoutList();

                EditorGUILayout.HelpBox("Add frames and unique backdrops for each resolution! Layer with the downloaded device frames and the offset will be updated automatically.", MessageType.Info);
            }

            string saveRawScreenshotTooltip = "Whether to save the original screenshot prior to capturing it within the frame. The same property is exposed under Live Frames and Screenshot Resolutions Live Frames for convenience. Modifying one will change both.";
            bool showLiveFrames = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showLiveFrames", "Live Frames");
            if (showLiveFrames)
            {

                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("saveRawScreenshot"), new GUIContent("Save Raw Screenshot", saveRawScreenshotTooltip));

                liveFrameList.DoLayoutList();

                EditorGUILayout.HelpBox("Put last captured screenshot back in the scene to capture within a custom frame. A multi-language live frame could be useful for store screenshots", MessageType.Info);
            }

            bool showScreenshotResolutionLiveFrames = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showScreenshotResolutionLiveFrames", "Screenshot Resolution Live Frames");
            if (showScreenshotResolutionLiveFrames)
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("saveRawScreenshot"), new GUIContent("Save Raw Screenshot", saveRawScreenshotTooltip));

                screenshotResolutionLiveFrameList.DoLayoutList();

                EditorGUILayout.HelpBox("Put last captured screenshot back in the scene to capture within a custom frame for a specific screenshot resolution. A multi-language live frame could be useful for store screenshots", MessageType.Info);
            }

            Background(true);

            CutoutSetting();

            if (cutoutScreenshotSetScriptEditor != null)
                cutoutScreenshotSetScriptEditor.Settings();
            if (screenshotBurstScriptEditor != null)
                screenshotBurstScriptEditor.Settings();
            if (multiLangScreenshotScriptEditor != null)
                multiLangScreenshotScriptEditor.Settings();
            if (screenshotSeriesScriptEditor != null)
                screenshotSeriesScriptEditor.Settings();
            if (gameObjectScreenshotScriptEditor != null)
                gameObjectScreenshotScriptEditor.Settings();
            if (multiCameraScreenshotScriptEditor != null)
                multiCameraScreenshotScriptEditor.Settings();

            TempEnabledObjects();

            TempDisabledObjects();

            Timing(true);

            ShowButtonSettings();

            EditorGUILayout.Space();

            DownloadDeviceFrames();

            CaptureButtons();

            EditorGUILayout.Space();

            OpenFileOrFolderButtons();
        }

        protected override void SaveTab()
        {
            Edit();

            SaveSettings();
        }
        #endregion

        #region Capture Tab Settings
        void Resolutions()
        {
            bool showResolutionList = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showResolutionList", "Resolutions");
            if (showResolutionList)
            {
                EditorGUI.BeginChangeCheck();
                resolutionList.DoLayoutList();
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    ((ScreenshotScript)target).ScreenshotResolutionsChanged();
                    EditorUtility.SetDirty(target);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("None"))
                {
                    for (int i = 0; i < ((ScreenshotScript)target).screenshotResolutions.Count; ++i)
                    {
                        ScreenshotResolution screenshotResolution = ((ScreenshotScript)target).screenshotResolutions[i];
                        screenshotResolution.active = false;
                    }
                }

                if (GUILayout.Button("Portrait"))
                {
                    for (int i = 0; i < ((ScreenshotScript)target).screenshotResolutions.Count; ++i)
                    {
                        ScreenshotResolution screenshotResolution = ((ScreenshotScript)target).screenshotResolutions[i];
                        screenshotResolution.active = screenshotResolution.height >= screenshotResolution.width;
                    }
                }

                if (GUILayout.Button("Landscape"))
                {
                    for (int i = 0; i < ((ScreenshotScript)target).screenshotResolutions.Count; ++i)
                    {
                        ScreenshotResolution screenshotResolution = ((ScreenshotScript)target).screenshotResolutions[i];
                        screenshotResolution.active = screenshotResolution.width >= screenshotResolution.height;
                    }
                }

                if (GUILayout.Button("All"))
                {
                    for (int i = 0; i < ((ScreenshotScript)target).screenshotResolutions.Count; ++i)
                    {
                        ScreenshotResolution screenshotResolution = ((ScreenshotScript)target).screenshotResolutions[i];
                        screenshotResolution.active = true;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void ResolutionSettings()
        {
            bool showResolutionSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showResolutionSettings", "Resolution Settings");
            if (showResolutionSettings)
            {
                // Uses label and toggle for spacing reasons
                bool originalGUIEnabled = GUI.enabled;

                EditorGUILayout.BeginHorizontal();
                string getFileNameFromScreenshotResolutionTooltip = "Whether to use the last component of the screenshot resolution as the file name.";
                EditorGUILayout.LabelField(new GUIContent("Get File Name from Screenshot Resolution", getFileNameFromScreenshotResolutionTooltip));
                serializedObject.FindProperty("getFileNameFromScreenshotResolution").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("getFileNameFromScreenshotResolution").boolValue);
                EditorGUILayout.EndHorizontal();

                bool canCaptureWithHDR = ((ScreenshotScript)target).canCaptureWithHDR;
                GUI.enabled &= canCaptureWithHDR;
                string captureWithHDRTooltip = "Enable to capture in a HDR texture format. Required for EXR. Unavailable in ScreenCapture Mode (unless using HDR output on an HDR supported monitor with alt capture mode disabled).";
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Capture with HDR", captureWithHDRTooltip));
                if(canCaptureWithHDR)
                    serializedObject.FindProperty("captureWithHDRIfPossible").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("captureWithHDRIfPossible").boolValue);
                else
                    EditorGUILayout.Toggle(false);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = originalGUIEnabled;

                string adjustDelayTooltip = "Enable to allow a delay before taking a screenshot. Useful if need time for animations to occur between screenshots such as when rotating between portrait and landscape.";
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Adjust Delay", adjustDelayTooltip));
                serializedObject.FindProperty("adjustScreenshotResolutionDelay").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("adjustScreenshotResolutionDelay").boolValue);
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                    CreateScreenshotResolutionList();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                string shareResolutionsBetweenPlatformsTooltip = "Check this box if you want the same screenshot resolutions on all platforms. Useful if your game doesn't have code/features that depend on the platform.";
                EditorGUILayout.LabelField(new GUIContent("Share Resolutions Between Platforms", shareResolutionsBetweenPlatformsTooltip));
                serializedObject.FindProperty("shareResolutionsBetweenPlatforms").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("shareResolutionsBetweenPlatforms").boolValue);
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    ((ScreenshotScript)target).ScreenshotResolutionsChanged();
                    EditorUtility.SetDirty(target);
                }

                ScreenshotScript.ResolutionSelect currentResolutionSelect = (ScreenshotScript.ResolutionSelect)serializedObject.FindProperty("sceneViewScreenshotResolution").intValue;
                string sceneViewScreenshotResolutionTooltip = "Camera Resolution uses the current camera resolution, so the screenshot looks exactly as you see it in the view.\n\nGameView Resolution is the currently selected GameView resolution.\n\nDefault Resolution will use the first item in the resolution list (or the GameView resolution if none exists).";
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("SceneView Screenshot Resolution", sceneViewScreenshotResolutionTooltip));
                serializedObject.FindProperty("sceneViewScreenshotResolution").intValue = (int)((ScreenshotScript.ResolutionSelect)EditorGUILayout.EnumPopup(currentResolutionSelect));
                EditorGUILayout.EndHorizontal();
            }
        }

        void ShowButtonSettings()
        {
            bool originalGUIEnabled = GUI.enabled;

            bool showButtonSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showButtonSettings", "Button Settings");
            if (showButtonSettings)
            {
                bool hasLiveFrames = ((ScreenshotScript)target).liveFrames.Length > 0 || ((ScreenshotScript)target).screenshotResolutionLiveFrames.Length > 0;

                EditorGUILayout.BeginHorizontal();
                string showDownloadFramesButtonTooltip = "Whether to show a button to download frames.";
                EditorGUILayout.LabelField(new GUIContent("Show Download Frames Button", showDownloadFramesButtonTooltip));
                serializedObject.FindProperty("showDownloadFramesButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showDownloadFramesButton").boolValue);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                string showSceneCaptureButtonTooltip = "Whether to show a button to capture the scene view.";
                EditorGUILayout.LabelField(new GUIContent("Show Scene Capture Button", showSceneCaptureButtonTooltip));
                serializedObject.FindProperty("showSceneCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showSceneCaptureButton").boolValue);
                EditorGUILayout.EndHorizontal();

                GUI.enabled &= hasLiveFrames;
                EditorGUILayout.BeginHorizontal();
                string showLiveFramesCaptureButtonTooltip = "Whether to show a button to capture a screenshot with live frame(s). Button only shown if a live frame exists on the screenshot script.";
                EditorGUILayout.LabelField(new GUIContent("Show Live Frames Capture Button", showLiveFramesCaptureButtonTooltip));
                if(hasLiveFrames)
                    serializedObject.FindProperty("showLiveFramesCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showLiveFramesCaptureButton").boolValue);
                else
                    EditorGUILayout.Toggle(false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                string showAllResolutionLiveFramesCaptureButtonTooltip = "Whether to show a button to capture a screenshot at each resolution with live frame(s). Button only shown if a live frame exists on the screenshot script.";
                EditorGUILayout.LabelField(new GUIContent("Show All Resolution Live Frames Capture Button", showAllResolutionLiveFramesCaptureButtonTooltip));
                if (hasLiveFrames)
                    serializedObject.FindProperty("showAllResolutionLiveFramesCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showAllResolutionLiveFramesCaptureButton").boolValue);
                else
                    EditorGUILayout.Toggle(false);
                EditorGUILayout.EndHorizontal();
                GUI.enabled = originalGUIEnabled;

                if (screenshotBurstScriptEditor != null)
                    screenshotBurstScriptEditor.ButtonSettings();

                if (screenshotSeriesScriptEditor != null)
                    screenshotSeriesScriptEditor.ButtonSettings();

                if (multiLangScreenshotScriptEditor != null)
                    multiLangScreenshotScriptEditor.ButtonSettings();
            }
        }

        void DownloadDeviceFrames()
        {
            if (!((ScreenshotScript)target).showDownloadFramesButton) return;

            if (GUILayout.Button(new GUIContent("Download Device Frames", "Download outlines of devices to use for your screenshots. To use the outline automatically, add outlines to your project and add a screenshot resolution transformation of type layer behind with the device outline as the other layer."), GUILayout.MinHeight(40)))
            {
                Application.OpenURL("https://s3-us-west-1.amazonaws.com/fbdesignresources/Devices/Facebook+Devices.zip");
                Application.OpenURL("https://developer.apple.com/design/resources/#product-bezels");
#if UNITY_EDITOR_WIN
                Application.OpenURL("https://www.google.com/search?q=open+dmg+windows");
#elif UNITY_EDITOR_LINUX
                Application.OpenURL("https://www.google.com/search?q=open+dmg+linux");
#endif
                ((ScreenshotScript)target).showDownloadFramesButton = false;
            }

            EditorGUILayout.Space();
        }

        void CaptureButtons()
        {
            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !((ScreenshotScript)target).screenshotsInProgress;

            if (serializedObject.FindProperty("showSceneCaptureButton").boolValue)
            {
                bool allowSceneViewScreenshot = SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null;
                using (new EditorGUI.DisabledScope(allowSceneViewScreenshot == false))
                {
                    string tooltipString = "Take a screenshot of the scene view at the scene view's current resolution.";
                    if (!allowSceneViewScreenshot) tooltipString += " (Open scene view and refresh the tool UI (by toggling tabs) to enable.)";
                    if (GUILayout.Button(new GUIContent("Take SceneView Screenshot", tooltipString), GUILayout.MinHeight(40)))
                    {
                        FileSettingsEditorHelper.RequestSavePath(((ScreenshotScript)target).fileSettings, serializedObject.FindProperty("fileSettings"));
                        ((ScreenshotScript)target).TakeSceneViewScreenshot(true);
                    }
                }
            }

            if (GUILayout.Button(new GUIContent("Take GameView Screenshot", "Take a screenshot of the game view at the game view's current resolution."), GUILayout.MinHeight(40)))
            {
                if (GameView.CurrentGameViewSizeType() == GameView.GameViewSizeType.AspectRatio)
                    Debug.LogWarning("GameView has an Aspect Ratio size. Use a Fixed Resolution GameView size to capture your preferred resolution.");

                FileSettingsEditorHelper.RequestSavePath(((ScreenshotScript)target).fileSettings, serializedObject.FindProperty("fileSettings"));
                ((ScreenshotScript)target).TakeSingleScreenshot(true);
            }

            bool hasLiveFrames = ((ScreenshotScript)target).hasLiveFrames;
            if(hasLiveFrames && serializedObject.FindProperty("showLiveFramesCaptureButton").boolValue)
            {
                if (GUILayout.Button(new GUIContent("Take Screenshot with Live Frame(s)", "Take a screenshot with live frame(s)."), GUILayout.MinHeight(60)))
                {
                    FileSettingsEditorHelper.RequestSavePath(((ScreenshotScript)target).fileSettings, serializedObject.FindProperty("fileSettings"));
                    ((ScreenshotScript)target).TakeLiveFrameScreenshots(true);
                }
            }

            bool hasScreenshotResolutions = ((ScreenshotScript)target).hasScreenshotResolutions;
            if(hasScreenshotResolutions)
            {
                if (GUILayout.Button(new GUIContent("Take All Screenshot Resolutions", "Take a screenshot at each of the resolutions in the resolutions list."), GUILayout.MinHeight(60)))
                {
                    FileSettingsEditorHelper.RequestSavePath(((ScreenshotScript)target).fileSettings, serializedObject.FindProperty("fileSettings"));
                    ((ScreenshotScript)target).TakeAllScreenshots(true);
                }

                if (hasLiveFrames && serializedObject.FindProperty("showAllResolutionLiveFramesCaptureButton").boolValue)
                {
                    if (GUILayout.Button(new GUIContent("Take All Screenshot Resolutions with Live Frames", "Take a screenshot at each of the resolutions in the resolutions list with live frame."), GUILayout.MinHeight(60)))
                    {
                        FileSettingsEditorHelper.RequestSavePath(((ScreenshotScript)target).fileSettings, serializedObject.FindProperty("fileSettings"));
                        ((ScreenshotScript)target).TakeAllScreenshotsWithLiveFrames(true);
                    }
                }
            }

            if (screenshotBurstScriptEditor != null)
                screenshotBurstScriptEditor.Buttons();

            if (screenshotSeriesScriptEditor != null)
                screenshotSeriesScriptEditor.Buttons();

            if (multiLangScreenshotScriptEditor != null)
                multiLangScreenshotScriptEditor.Buttons();

            if (cutoutScreenshotSetScriptEditor != null)
                cutoutScreenshotSetScriptEditor.Buttons();

            if (gameObjectScreenshotScriptEditor != null)
                gameObjectScreenshotScriptEditor.Buttons();

            if (multiCameraScreenshotScriptEditor != null)
                multiCameraScreenshotScriptEditor.Buttons();

            if(multiCameraGameObjectScreenshotScriptEditor != null)
                multiCameraGameObjectScreenshotScriptEditor.Buttons();

            GUI.enabled = originalGUIEnabled;
        }
#endregion

#region Save Tab Settings
        void SaveSettings()
        {
            bool showSaveSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSaveSettings", "Save Settings");
            if (showSaveSettings)
            {
                SerializedProperty fileSettingsProperty = serializedObject.FindProperty("fileSettings");
                FileSettingsEditorHelper.ScreenshotFileSettingsFields(((ScreenshotScript)target).fileSettings, fileSettingsProperty, true, ((ScreenshotScript)target).editorWindowMode);
            }
        }
#endregion

#region Settings Tab Settings
        protected override void ScreenCaptureConfig(string additionalContext = "")
        {
            base.ScreenCaptureConfig("Scale can also be set on each screenshot resolution when enabled. ");
        }

        protected override void ScreenCaptureConfigChanged()
        {
            base.ScreenCaptureConfigChanged();
            CreateScreenshotResolutionList();
        }

        protected override void HotKeys()
        {
            bool showHotKeys = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showHotKeys", "HotKeys");
            if (showHotKeys)
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("takeSingleScreenshotKeySet"), true);
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("takeAllScreenshotsKeySet"), true);
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("previewCutoutKeySet"), true);
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("pauseKeySet"), true);

                EditorGUILayout.HelpBox("Hotkeys that overlap existing Unity Editor hotkeys can cause issues.", MessageType.Info);
                if (GUILayout.Button("Existing Unity Hotkeys"))
                    Application.OpenURL("https://docs.unity3d.com/Manual/UnityHotkeys.html");
            }
        }

        protected override void AudioSettings()
        {
            bool showAudioSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showAudioSettings", "Audio Settings");
            if (showAudioSettings)
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("screenshotAudioSource"), new GUIContent("Screenshot Sound"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("playScreenshotAudioInEditor"), new GUIContent("Play in Editor"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("playScreenshotAudioInGame"), new GUIContent("Play in Game"));
            }
        }
#endregion

#region Helpers
        void CreateScreenshotResolutionList()
        {
            TRS.CaptureTool.CaptureMode captureMode = (TRS.CaptureTool.CaptureMode)serializedObject.FindProperty("__captureMode").intValue;
            bool adjustScale = captureMode == TRS.CaptureTool.CaptureMode.ScreenCapture && serializedObject.FindProperty("screenCaptureConfig").FindPropertyRelative("useAltScreenCapture").boolValue;
            resolutionList = new ScreenshotResolutionReorderableList(serializedObject,
                                                         serializedObject.FindProperty("screenshotResolutions"),
                                                         adjustScale,
                                                         serializedObject.FindProperty("adjustScreenshotResolutionDelay").boolValue,
                                     true, true, true, true, serializedObject.FindProperty("shareResolutionsBetweenPlatforms"));

            if (gameObjectScreenshotScriptEditor == null)
                UpdateSubEditors();

            if (gameObjectScreenshotScriptEditor != null)
                gameObjectScreenshotScriptEditor.Refresh();
        }

        void Edit()
        {
            bool showEdit = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showEdit", "Edit");
            if (showEdit)
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("textureToEdit"));
                EditorGUILayout.HelpBox("Automatically updated to the last screenshot texture (a copy will be made prior to changes). Feel free to select any other texture.", MessageType.Info);

                Texture2D preview = ((ScreenshotScript)target).textureToEdit;
                if (preview != null)
                {
                    float previewScale = EditorGUIUtility.currentViewWidth / (float)preview.width;
                    Rect position = EditorGUILayout.GetControlRect(false, Mathf.Min(preview.height * previewScale, MAX_HEIGHT));
                    if (Event.current.type.Equals(EventType.Repaint))
                        GUI.DrawTexture(position, preview, ScaleMode.ScaleToFit);
                }

                bool showEditTransformations = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showEditTransformations", "Edit Transformations");
                if (showEditTransformations)
                    editTransformationList.DoLayoutList();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Apply Transformations", GUILayout.MinHeight(40)))
                {
                    bool isLastScreenshoTexture = ((ScreenshotScript)target).textureToEdit == ((ScreenshotScript)target).lastScreenshotTexture;
                    ((ScreenshotScript)target).textureToEdit = ((ScreenshotScript)target).textureToEdit.EditableTexture(isLastScreenshoTexture, true);
                    ((ScreenshotScript)target).textureToEdit = ((ScreenshotScript)target).textureToEdit.ApplyTransformations(((ScreenshotScript)target).editTransformations);
                    EditorUtility.SetDirty(target);
                }

                bool originalGUIEnabled = GUI.enabled;
                GUI.enabled &= ((ScreenshotScript)target).lastScreenshotTexture != null && ((ScreenshotScript)target).textureToEdit != ((ScreenshotScript)target).lastScreenshotTexture;
                if (GUILayout.Button("Restore to Last Screenshot", GUILayout.MinHeight(40)))
                {
                    ((ScreenshotScript)target).textureToEdit = ((ScreenshotScript)target).lastScreenshotTexture;
                    EditorUtility.SetDirty(target);
                }
                GUILayout.EndHorizontal();
                GUI.enabled = originalGUIEnabled;

                if (GUILayout.Button("Save", GUILayout.MinHeight(40)))
                    ((ScreenshotScript)target).Save(((ScreenshotScript)target).textureToEdit);
            }
        }

        protected override void GeneralUpdates()
        {
            ForceUpdates();
        }

        protected override void UpdateSubEditors()
        {
            base.UpdateSubEditors();

            ScreenshotSubComponentScript screenshotSubComponentScript;

            screenshotSubComponentScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.ScreenshotSeries);
            if (screenshotSubComponentScript != null)
            {
                if (screenshotSeriesScriptEditor == null)
                    screenshotSeriesScriptEditor = (ScreenshotSubComponentScriptEditor)Editor.CreateEditor(screenshotSubComponentScript);
            }
            else
                screenshotSeriesScriptEditor = null;

            screenshotSubComponentScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.GameObject);
            if (screenshotSubComponentScript != null)
            {
                if (gameObjectScreenshotScriptEditor == null)
                    gameObjectScreenshotScriptEditor = (ScreenshotSubComponentScriptEditor)Editor.CreateEditor(screenshotSubComponentScript);
            }
            else
                gameObjectScreenshotScriptEditor = null;

            screenshotSubComponentScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.MultiLanguage);
            if (screenshotSubComponentScript != null)
            {
                if (multiLangScreenshotScriptEditor == null)
                    multiLangScreenshotScriptEditor = (ScreenshotSubComponentScriptEditor)Editor.CreateEditor(screenshotSubComponentScript);
            }
            else
                multiLangScreenshotScriptEditor = null;

            screenshotSubComponentScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.Burst);
            if (screenshotSubComponentScript != null)
            {
                if (screenshotBurstScriptEditor == null)
                    screenshotBurstScriptEditor = (ScreenshotSubComponentScriptEditor)Editor.CreateEditor(screenshotSubComponentScript);
            }
            else
                screenshotBurstScriptEditor = null;

            screenshotSubComponentScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.CutoutSet);
            if (screenshotSubComponentScript != null)
            {
                if (cutoutScreenshotSetScriptEditor == null)
                    cutoutScreenshotSetScriptEditor = (ScreenshotSubComponentScriptEditor)Editor.CreateEditor(screenshotSubComponentScript);
            }
            else
                cutoutScreenshotSetScriptEditor = null;

            screenshotSubComponentScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.MultiCamera);
            if (screenshotSubComponentScript != null)
            {
                if (multiCameraScreenshotScriptEditor == null)
                    multiCameraScreenshotScriptEditor = (ScreenshotSubComponentScriptEditor)Editor.CreateEditor(screenshotSubComponentScript);
            }
            else
                multiCameraScreenshotScriptEditor = null;

            screenshotSubComponentScript = ((ScreenshotScript)target).GetSubComponent(ScreenshotScript.SubComponentType.MultiCameraGameObject);
            if (screenshotSubComponentScript != null)
            {
                if (multiCameraGameObjectScreenshotScriptEditor == null)
                    multiCameraGameObjectScreenshotScriptEditor = (ScreenshotSubComponentScriptEditor)Editor.CreateEditor(screenshotSubComponentScript);
            }
            else
                multiCameraGameObjectScreenshotScriptEditor = null;
        }

        // RequiresConstantRepaint doesn't avert getting paused within a Take All Screenshot Resolutions loop.
        void ForceUpdates()
        {
            if (((ScreenshotScript)target).screenshotsInProgress)
                serializedObject.FindProperty("screenshotsEditorRefreshHack").intValue += 1;
            else if (serializedObject.FindProperty("screenshotsEditorRefreshHack").intValue > 0)
                serializedObject.FindProperty("screenshotsEditorRefreshHack").intValue -= 1;
        }
#endregion
    }
}