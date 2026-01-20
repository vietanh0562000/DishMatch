using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(MultiCameraScreenshotScript))]
    public class MultiCameraScreenshotScriptEditor : ScreenshotSubComponentScriptEditor
    {
        ReorderableList cameraList;

        void OnEnable()
        {
            if (target == null)
                return;
            ReloadCameraList();
        }

        public void ReloadCameraList()
        {
            cameraList = new ReorderableList(serializedObject,
                                 serializedObject.FindProperty("cameras"),
                                 true, true, true, true);
            cameraList.AddHeader("Camera");
            cameraList.AddStandardElementCallback();
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

            string settingsName = ((MultiCameraScreenshotScript)target).subWindowMode ? "Multi Camera Screenshot Settings" : "Settings";
            bool showSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSettings", settingsName);
            if (showSettings)
            {
                if (!(((MultiCameraScreenshotScript)target).editorWindowMode || ((MultiCameraScreenshotScript)target).subWindowMode))
                {
                    ScreenshotScript currentScreenshotScript = (ScreenshotScript)serializedObject.FindProperty("screenshotScript").objectReferenceValue;
                    serializedObject.FindProperty("screenshotScript").objectReferenceValue = EditorGUILayout.ObjectField("Screenshot Script", currentScreenshotScript, typeof(ScreenshotScript), true);
                }

                cameraList.DoLayoutList();
            }

            serializedObject.ApplyModifiedProperties();

            GUI.enabled = originalGUIEnabled;
        }

        public void Button()
        {
            if (target == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;

            if (GUILayout.Button(new GUIContent("Take Multi-Camera Screenshots", "Take screenshots from each of the listed cameras using render textures."), GUILayout.MinHeight(40)))
                ((MultiCameraScreenshotScript)target).TakeMultiCameraScreenshots();

            GUI.enabled = originalGUIEnabled;
        }
    }
}