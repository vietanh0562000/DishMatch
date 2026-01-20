using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(ScreenshotSeriesScript))]
    public class ScreenshotSeriesScriptEditor : ScreenshotSubComponentScriptEditor
    {
        ReorderableList buttonInteractionList;

        void OnEnable()
        {
            if (target == null)
                return;

            buttonInteractionList = new ReorderableList(serializedObject,
                                                 serializedObject.FindProperty("buttonInteractions"),
                                 true, true, true, true);

            string[] headerTexts = { "Button to Press", "Animation Delay", "Take Photo" };
            string[] properties = { "button", "animationDelay", "takePhoto" };
            float[] widths = { 0.4f, 0.4f, 0.2f };

            buttonInteractionList.AddHeader(headerTexts, widths);
            buttonInteractionList.AddStandardElementCallback(properties, widths);
        }

        public override void Buttons()
        {
            Button();
            LiveFrameButton();
        }

        public override void Settings()
        {
            if (target == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            serializedObject.Update();

            string settingsName = ((ScreenshotSeriesScript)target).subWindowMode ? "Screenshot Series Settings" : "Settings";
            bool showSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSettings", settingsName);
            if (showSettings)
            {
                if (!(((ScreenshotSeriesScript)target).editorWindowMode || ((ScreenshotSeriesScript)target).subWindowMode))
                {
                    ScreenshotScript currentScreenshotScript = (ScreenshotScript)serializedObject.FindProperty("screenshotScript").objectReferenceValue;
                    serializedObject.FindProperty("screenshotScript").objectReferenceValue = EditorGUILayout.ObjectField("Screenshot Script", currentScreenshotScript, typeof(ScreenshotScript), true);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Capture Initial Screen");
                serializedObject.FindProperty("captureInitialScreen").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("captureInitialScreen").boolValue);
                EditorGUILayout.EndHorizontal();

                buttonInteractionList.DoLayoutList();

                if (serializedObject.FindProperty("buttonInteractions").arraySize == 0)
                    EditorGUILayout.HelpBox("Entering rows without a a button press to create a delay is valid too. (You could even use this as a macro to jump around your app.)", MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = originalGUIEnabled;
        }

        public override void ButtonSettings()
        {
            bool originalGUIEnabled = GUI.enabled;
            bool hasLiveFrames = ((ScreenshotSeriesScript)target).screenshotScript.liveFrames.Length > 0 || ((ScreenshotSeriesScript)target).screenshotScript.screenshotResolutionLiveFrames.Length > 0;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Show Series Button"));
            serializedObject.FindProperty("showTakeSeriesCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showTakeSeriesCaptureButton").boolValue);
            EditorGUILayout.EndHorizontal();

            GUI.enabled &= hasLiveFrames;
            string mustHaveLiveFramesTooltip = "Button only shown if a live frame exists on the screenshot script.";
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Show Live Frame Series Button", mustHaveLiveFramesTooltip));
            if (hasLiveFrames)
                serializedObject.FindProperty("showTakeSeriesLiveFrameCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showTakeSeriesLiveFrameCaptureButton").boolValue);
            else
                EditorGUILayout.Toggle(false);
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = originalGUIEnabled;
        }

        public void Button()
        {
            if (!serializedObject.FindProperty("showTakeSeriesCaptureButton").boolValue)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating && Application.isPlaying;
            string tooltipString = "Capture multiple screens with a single button press.";
            if (!GUI.enabled) tooltipString += " (Only works while the game is playing.)";
            if (GUILayout.Button(new GUIContent("Take Screenshot Series", tooltipString), GUILayout.MinHeight(40)))
                ((ScreenshotSeriesScript)target).TakeScreenshotSeries();
            GUI.enabled = originalGUIEnabled;
        }

        public void LiveFrameButton()
        {
            if (!serializedObject.FindProperty("showTakeSeriesLiveFrameCaptureButton").boolValue)
                return;

            bool hasLiveFrames = ((ScreenshotSeriesScript)target).screenshotScript.liveFrames.Length > 0 || ((ScreenshotSeriesScript)target).screenshotScript.screenshotResolutionLiveFrames.Length > 0;
            if (!hasLiveFrames) return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating && Application.isPlaying;
            string tooltipString = "Capture multiple screens with live frames with a single button press.";
            if (!GUI.enabled) tooltipString += " (Only works while the game is playing.)";
            if (GUILayout.Button(new GUIContent("Take Screenshot Series with Live Frames", tooltipString), GUILayout.MinHeight(40)))
                ((ScreenshotSeriesScript)target).TakeScreenshotSeriesWithLiveFrames();
            GUI.enabled = originalGUIEnabled;
        }
    }
}