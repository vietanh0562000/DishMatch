using UnityEngine;
using UnityEditor;

namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    public class ScreenshotSubComponentScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (target == null || ((ScreenshotSubComponentScript)target).hiddenMode)
                return;

            if (!(((ScreenshotSubComponentScript)target).editorWindowMode || ((ScreenshotSubComponentScript)target).subWindowMode))
                EditorGUILayout.Space();

            Display();
        }

        public void Display()
        {
            Settings();

            Buttons();
        }

        public virtual void Refresh()
        {

        }

        public virtual void Buttons()
        {

        }

        public virtual void Settings()
        {

        }

        public virtual void ButtonSettings()
        {

        }
    }
}