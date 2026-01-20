using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(GameObjectScreenshotScript))]
    public class GameObjectScreenshotScriptEditor : ScreenshotSubComponentScriptEditor
    {
        ReorderableList pairList;

        void OnEnable()
        {
            if (target == null)
                return;

            Refresh();
        }

        public override void Refresh()
        {
            bool adjustScale = false;
            bool adjustDelay = false;
            ScreenshotScript screenshotScript = ((GameObjectScreenshotScript)target).screenshotScript;
            if (screenshotScript != null)
            {
                adjustScale = screenshotScript.captureMode == CaptureMode.ScreenCapture && screenshotScript.screenCaptureConfig != null && screenshotScript.screenCaptureConfig.useAltScreenCapture;
                adjustDelay = screenshotScript.adjustScreenshotResolutionDelay;
            }
            ReloadPairList(adjustScale, adjustDelay);
        }

        public override void Buttons()
        {
            Button();
        }

        public override void Settings()
        {
            if (target == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            serializedObject.Update();

            string settingsName = ((GameObjectScreenshotScript)target).subWindowMode ? "Game Object Screenshot Settings" : "Settings";
            bool showSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSettings", settingsName);
            if (showSettings)
            {
                if (!(((GameObjectScreenshotScript)target).editorWindowMode || ((GameObjectScreenshotScript)target).subWindowMode))
                {
                    ScreenshotScript currentScreenshotScript = (ScreenshotScript)serializedObject.FindProperty("screenshotScript").objectReferenceValue;
                    serializedObject.FindProperty("screenshotScript").objectReferenceValue = EditorGUILayout.ObjectField("Screenshot Script", currentScreenshotScript, typeof(ScreenshotScript), true);
                }

                pairList.DoLayoutList();
            }

            serializedObject.ApplyModifiedProperties();

            GUI.enabled = originalGUIEnabled;
        }

        public void ReloadPairList(bool adjustScale, bool adjustDelay)
        {
            pairList = new GameObjectResolutionReorderableList(serializedObject,
                                 serializedObject.FindProperty("pairs"), adjustScale, adjustDelay, true, true, true, true, null);
        }

        public void Button()
        {
            if (target == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;

            if (GUILayout.Button(new GUIContent("Take GameObject Screenshots", "Take screenshots of game objects at different resolutions."), GUILayout.MinHeight(40)))
                ((GameObjectScreenshotScript)target).TakeGameObjectScreenshots();

            GUI.enabled = originalGUIEnabled;
        }
    }
}