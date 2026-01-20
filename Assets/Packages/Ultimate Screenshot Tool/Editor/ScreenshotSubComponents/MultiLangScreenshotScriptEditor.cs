using UnityEngine;
using UnityEditor;
#if UNITY_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(MultiLangScreenshotScript))]
    public class MultiLangScreenshotScriptEditor : ScreenshotSubComponentScriptEditor
    {
        public override void Buttons()
        {
            TakeButton();
            TakeLiveFrameButton();
            TakeAllButton();
            TakeAllLiveFramesButton();
            TakeSeriesButton();
            TakeLiveFrameSeriesButton();
        }

        public override void Settings()
        {
            if (target == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            serializedObject.Update();

            string settingsName = ((MultiLangScreenshotScript)target).subWindowMode ? "Multi-Language Settings" : "Settings";
            bool showSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSettings", settingsName);
            if (showSettings)
            {
                if (!(((MultiLangScreenshotScript)target).editorWindowMode || ((MultiLangScreenshotScript)target).subWindowMode))
                {
                    ScreenshotScript currentScreenshotScript = (ScreenshotScript)serializedObject.FindProperty("screenshotScript").objectReferenceValue;
                    serializedObject.FindProperty("screenshotScript").objectReferenceValue = EditorGUILayout.ObjectField("Screenshot Script", currentScreenshotScript, typeof(ScreenshotScript), true);

                    ScreenshotSeriesScript currentScreenshotSeriesScript = (ScreenshotSeriesScript)serializedObject.FindProperty("screenshotSeriesScript").objectReferenceValue;
                    serializedObject.FindProperty("screenshotSeriesScript").objectReferenceValue = EditorGUILayout.ObjectField("Screenshot Series Script", currentScreenshotSeriesScript, typeof(ScreenshotSeriesScript), true);
                }

#if UNITY_LOCALIZATION
                string allLocalesTooltip = "Whether to run through all available locales during screenshots or a selected subset.";
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("allLocales"), new GUIContent("All Locales", allLocalesTooltip));
                if(!serializedObject.FindProperty("allLocales").boolValue)
                {
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("locales"), true);
                }
#else
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("languages"), true);
#endif

            }

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = originalGUIEnabled;
        }

        public override void ButtonSettings()
        {
            bool originalGUIEnabled = GUI.enabled;
            bool hasScreenshotResolutions = ((MultiLangScreenshotScript)target).screenshotScript != null && ((MultiLangScreenshotScript)target).screenshotScript.hasScreenshotResolutions;
            bool hasLiveFrames = ((MultiLangScreenshotScript)target).screenshotScript != null && ((MultiLangScreenshotScript)target).screenshotScript.hasLiveFrames;
            bool hasScreenshotSeriesScript = ((MultiLangScreenshotScript)target).screenshotSeriesScript != null;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Show Multi-Language Button"));
            serializedObject.FindProperty("showTakeMultiLangCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showTakeMultiLangCaptureButton").boolValue);
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            GUI.enabled &= hasLiveFrames;
            string mustHaveLiveFramesTooltip = "Button only shown if a live frame exists on the screenshot script.";
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Show Multi-Language Live Frame Button", mustHaveLiveFramesTooltip));
            if (hasLiveFrames)
                serializedObject.FindProperty("showTakeMultiLangLiveFrameCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showTakeMultiLangLiveFrameCaptureButton").boolValue);
            else
                EditorGUILayout.Toggle(false);
            EditorGUILayout.EndHorizontal();

            GUI.enabled = originalGUIEnabled && hasScreenshotResolutions;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Show All Resolution Multi-Language Button"));
            if (hasScreenshotResolutions)
                serializedObject.FindProperty("showTakeAllResolutionMultiLangCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showTakeAllResolutionMultiLangCaptureButton").boolValue);
            else
                EditorGUILayout.Toggle(false);
            EditorGUILayout.EndHorizontal();

            GUI.enabled &= hasLiveFrames;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Show All Resolution Multi-Language Live Frame Button", mustHaveLiveFramesTooltip));
            if (hasLiveFrames)
                serializedObject.FindProperty("showTakeAllResolutionMultiLangLiveFrameCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showTakeAllResolutionMultiLangLiveFrameCaptureButton").boolValue);
            else
                EditorGUILayout.Toggle(false);
            EditorGUILayout.EndHorizontal();

            GUI.enabled = originalGUIEnabled && hasScreenshotSeriesScript;
            string mustHaveScreenshotSeriesScriptTooltip = "Button only shown if a screenshot series script component is added to the same object as the screenshot script.";
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Show Multi-Language Series Button", mustHaveScreenshotSeriesScriptTooltip));
            if(hasScreenshotSeriesScript)
                serializedObject.FindProperty("showTakeMultiLangSeriesCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showTakeMultiLangSeriesCaptureButton").boolValue);
            else
                EditorGUILayout.Toggle(false);
            EditorGUILayout.EndHorizontal();

            GUI.enabled &= hasLiveFrames;
            string mustHaveLiveFramesAndScreenshotSeriesScriptTooltip = "Button only shown if a live frame exists on the screenshot script and a screenshot series script component is added to the same object as the screenshot script.";
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Show Multi-Language Live Frame Series Button", mustHaveLiveFramesAndScreenshotSeriesScriptTooltip));
            if (hasLiveFrames && hasScreenshotSeriesScript)
                serializedObject.FindProperty("showTakeMultiLangSeriesLiveFrameCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showTakeMultiLangSeriesLiveFrameCaptureButton").boolValue);
            else
                EditorGUILayout.Toggle(false);
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = originalGUIEnabled;
        }

        public void TakeButton()
        {
            if (!serializedObject.FindProperty("showTakeMultiLangCaptureButton").boolValue)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            if (GUILayout.Button(new GUIContent("Take Multi-Language Screenshots", "Take a screenshot for each of the languages."), GUILayout.MinHeight(40)))
                ((MultiLangScreenshotScript)target).TakeMultiLangScreenshots();
            GUI.enabled = originalGUIEnabled;
        }

        public void TakeLiveFrameButton()
        {
            if (!serializedObject.FindProperty("showTakeMultiLangLiveFrameCaptureButton").boolValue)
                return;

            bool hasLiveFrames = ((MultiLangScreenshotScript)target).screenshotScript != null && ((MultiLangScreenshotScript)target).screenshotScript.hasLiveFrames;
            if (!hasLiveFrames) return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            if (GUILayout.Button(new GUIContent("Take Multi-Language Screenshots with Live Frames", "Take a screenshot for each of the languages with live frame(s)."), GUILayout.MinHeight(40)))
                ((MultiLangScreenshotScript)target).TakeMultiLangScreenshotsWithLiveFrames();
            GUI.enabled = originalGUIEnabled;
        }

        public void TakeAllButton()
        {
            if (!serializedObject.FindProperty("showTakeAllResolutionMultiLangCaptureButton").boolValue)
                return;

            bool hasScreenshotResolutions = ((MultiLangScreenshotScript)target).screenshotScript != null && ((MultiLangScreenshotScript)target).screenshotScript.hasScreenshotResolutions;
            if (!hasScreenshotResolutions) return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            if (GUILayout.Button(new GUIContent("Take Multi-Language All Screenshot Resolutions", "Take a screenshot at each of the resolutions in the resolutions list for each of the languages."), GUILayout.MinHeight(40)))
                ((MultiLangScreenshotScript)target).TakeAllResolutionMultiLangScreenshots();
            GUI.enabled = originalGUIEnabled;
        }


        public void TakeAllLiveFramesButton()
        {
            if (!serializedObject.FindProperty("showTakeAllResolutionMultiLangLiveFrameCaptureButton").boolValue)
                return;

            bool hasLiveFrames = ((MultiLangScreenshotScript)target).screenshotScript != null && ((MultiLangScreenshotScript)target).screenshotScript.hasLiveFrames;
            if (!hasLiveFrames) return;

            bool hasScreenshotResolutions = ((MultiLangScreenshotScript)target).screenshotScript != null && ((MultiLangScreenshotScript)target).screenshotScript.hasScreenshotResolutions;
            if (!hasScreenshotResolutions) return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            if (GUILayout.Button(new GUIContent("Take Multi-Language All Screenshot Resolutions with Live Frames", "Take a screenshot at each of the resolutions in the resolutions list for each of the languages with live frame(s)."), GUILayout.MinHeight(40)))
                ((MultiLangScreenshotScript)target).TakeAllResolutionMultiLangScreenshotsWithLiveFrames();
            GUI.enabled = originalGUIEnabled;
        }

        public void TakeSeriesButton()
        {
            if (!serializedObject.FindProperty("showTakeMultiLangSeriesCaptureButton").boolValue)
                return;
            
            ScreenshotSeriesScript currentScreenshotSeriesScript = (ScreenshotSeriesScript)serializedObject.FindProperty("screenshotSeriesScript").objectReferenceValue;
            if (currentScreenshotSeriesScript == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating && Application.isPlaying;
            string tooltipString = "Capture multiple screens in multiple different languages with a single button press.";
            if (!GUI.enabled) tooltipString += " (Only works while the game is playing.)";
            if (GUILayout.Button(new GUIContent("Take Multi-Language Screenshot Series", tooltipString), GUILayout.MinHeight(40)))
                ((MultiLangScreenshotScript)target).TakeMultiLangScreenshotSeries();
            GUI.enabled = originalGUIEnabled;
        }

        public void TakeLiveFrameSeriesButton()
        {
            if (!serializedObject.FindProperty("showTakeMultiLangSeriesLiveFrameCaptureButton").boolValue)
                return;

            bool hasLiveFrames = ((MultiLangScreenshotScript)target).screenshotScript != null && ((MultiLangScreenshotScript)target).screenshotScript.hasLiveFrames;
            if (!hasLiveFrames) return;

            ScreenshotSeriesScript currentScreenshotSeriesScript = (ScreenshotSeriesScript)serializedObject.FindProperty("screenshotSeriesScript").objectReferenceValue;
            if (currentScreenshotSeriesScript == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating && Application.isPlaying;
            string tooltipString = "Capture multiple screens in multiple different languages with live frames with a single button press.";
            if (!GUI.enabled) tooltipString += " (Only works while the game is playing.)";
            if (GUILayout.Button(new GUIContent("Take Multi-Language Screenshot Series with Live Frames", tooltipString), GUILayout.MinHeight(40)))
                ((MultiLangScreenshotScript)target).TakeMultiLangScreenshotSeriesWithLiveFrames();
            GUI.enabled = originalGUIEnabled;
        }
    }
}