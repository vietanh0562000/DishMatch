using UnityEngine;
using UnityEditor;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(ScreenshotBurstScript))]
    public class ScreenshotBurstScriptEditor : ScreenshotSubComponentScriptEditor
    {
        public override void Buttons()
        {
            TakeBurstButton();
            TakeAllBurstButton();
        }

        public override void Settings()
        {
            if (target == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            serializedObject.Update();

            string settingsName = ((ScreenshotBurstScript)target).subWindowMode ? "Burst Settings" : "Settings";
            bool showSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSettings", settingsName);
            if (showSettings)
            {
                if (!(((ScreenshotBurstScript)target).editorWindowMode || ((ScreenshotBurstScript)target).subWindowMode))
                {
                    ScreenshotScript currentScreenshotScript = (ScreenshotScript)serializedObject.FindProperty("screenshotScript").objectReferenceValue;
                    serializedObject.FindProperty("screenshotScript").objectReferenceValue = EditorGUILayout.ObjectField("Screenshot Script", currentScreenshotScript, typeof(ScreenshotScript), true);
                }

                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("takeScreenshotBurstKeySet"), true);
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("burstSize"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("useTimeDelay"));
                if(serializedObject.FindProperty("useTimeDelay").boolValue)
                {
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("realtimeDelay"));
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("delay"));
                }
                else
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("skipFrames"));

                int burstSize = serializedObject.FindProperty("burstSize").intValue;
                if (burstSize <= 0)
                {
                    burstSize = 1;
                    serializedObject.FindProperty("burstSize").intValue = burstSize;
                }

                int skipFrames = serializedObject.FindProperty("skipFrames").intValue;
                if (skipFrames < 0)
                {
                    skipFrames = 0;
                    serializedObject.FindProperty("skipFrames").intValue = skipFrames;
                }

                float delay = serializedObject.FindProperty("delay").floatValue;
                if (delay < 0)
                {
                    delay = 0;
                    serializedObject.FindProperty("delay").floatValue = delay;
                }

                string helpString = "Take burst of " + burstSize + " screenshots.";
                if (serializedObject.FindProperty("useTimeDelay").boolValue)
                {
                    helpString += " Capturing every " + delay + " seconds";
                    if (serializedObject.FindProperty("realtimeDelay").boolValue)
                        helpString += " in realtime.";
                    else
                        helpString += " in game time.";
                }
                else
                {
                    if (skipFrames > 0)
                    {
                        if (skipFrames == 1)
                            helpString += " Capturing every other frame.";
                        else
                            helpString += " Capturing every " + skipFrames + " frames.";
                    }
                }


                EditorGUILayout.HelpBox(helpString, MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = originalGUIEnabled;
        }

        public override void ButtonSettings()
        {
            bool originalGUIEnabled = GUI.enabled;

            bool hasScreenshotResolutions = ((ScreenshotBurstScript)target).screenshotScript != null && ((ScreenshotBurstScript)target).screenshotScript.hasScreenshotResolutions;
            GUI.enabled &= hasScreenshotResolutions;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Show All Resolution Series Button", ""));
            if (hasScreenshotResolutions)
                serializedObject.FindProperty("showTakeAllResolutionBurstCaptureButton").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty("showTakeAllResolutionBurstCaptureButton").boolValue);
            else
                EditorGUILayout.Toggle(false);
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = originalGUIEnabled;
        }

        public void TakeBurstButton()
        {
            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating && Application.isPlaying;
            if (GUILayout.Button(new GUIContent("Take Screenshot Burst", "Take a burst of screenshots in successive frames."), GUILayout.MinHeight(40)))
                ((ScreenshotBurstScript)target).TakeScreenshotBurst();
            GUI.enabled = originalGUIEnabled;
        }

        public void TakeAllBurstButton()
        {
            if (!serializedObject.FindProperty("showTakeAllResolutionBurstCaptureButton").boolValue)
                return;

            bool hasScreenshotResolutions = ((ScreenshotBurstScript)target).screenshotScript != null && ((ScreenshotBurstScript)target).screenshotScript.hasScreenshotResolutions;
            if (!hasScreenshotResolutions) return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating && Application.isPlaying;
            if (GUILayout.Button(new GUIContent("Take Burst All Screenshot Resolutions", "Take a burst of screenshots at each of the resolutions in the resolutions list."), GUILayout.MinHeight(40)))
                ((ScreenshotBurstScript)target).TakeAllScreenshotBurst();
            GUI.enabled = originalGUIEnabled;
        }
    }
}