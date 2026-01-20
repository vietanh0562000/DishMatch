using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(MultiCameraGameObjectScreenshotScript))]
    public class MultiCameraGameObjectScreenshotScriptEditor : ScreenshotSubComponentScriptEditor
    {

        public override void Buttons()
        {
            Button();
        }

        public override void Settings()
        {

        }

        public void Button()
        {
            if (target == null)
                return;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;

            if (GUILayout.Button(new GUIContent("Take Multi-Camera GameObject Screenshots", "Take screenshots of each gameObject from each of the listed cameras using render textures."), GUILayout.MinHeight(40)))
                ((MultiCameraGameObjectScreenshotScript)target).TakeMultiCameraGameObjectScreenshots();

            GUI.enabled = originalGUIEnabled;
        }
    }
}