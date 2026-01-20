using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(CutoutScreenshotSetScript))]
    public class CutoutScreenshotSetScriptEditor : ScreenshotSubComponentScriptEditor
    {
        ReorderableList cutoutScriptsList;
        ReorderableList cutoutAdjustedRectTransformsList;

        void OnEnable()
        {
            if (target == null)
                return;

            cutoutScriptsList = new ReorderableList(serializedObject,
                                     serializedObject.FindProperty("cutoutScripts"),
                     true, true, true, true);

            cutoutScriptsList.AddHeader("Cutout Scripts");
            cutoutScriptsList.AddStandardElementCallback();

            cutoutAdjustedRectTransformsList = new ReorderableList(serializedObject,
                                                 serializedObject.FindProperty("cutoutAdjustedRectTransforms"),
                                 true, true, true, true);

            cutoutAdjustedRectTransformsList.AddHeader("Adjusted Rect Transforms");
            cutoutAdjustedRectTransformsList.AddStandardElementCallback();
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

            string settingsName = ((ScreenshotSubComponentScript)target).subWindowMode ? "Cutout Set Settings" : "Settings";
            bool showSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSettings", settingsName);
            if (showSettings)
            {
                if (!(((ScreenshotSubComponentScript)target).editorWindowMode || ((ScreenshotSubComponentScript)target).subWindowMode))
                {
                    ScreenshotScript currentScreenshotScript = (ScreenshotScript)serializedObject.FindProperty("screenshotScript").objectReferenceValue;
                    serializedObject.FindProperty("screenshotScript").objectReferenceValue = EditorGUILayout.ObjectField("Screenshot Script", currentScreenshotScript, typeof(ScreenshotScript), true);
                }

                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("takeCutoutSetKeySet"), true);
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("includeOriginalCutout"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("overrideResolution"));
                if (serializedObject.FindProperty("overrideResolution").boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Width", GUILayout.MaxWidth(40));
                    serializedObject.FindProperty("resolutionWidth").intValue = EditorGUILayout.IntField(serializedObject.FindProperty("resolutionWidth").intValue);
                    if (serializedObject.FindProperty("resolutionWidth").intValue < 5)
                        serializedObject.FindProperty("resolutionWidth").intValue = 5;
                    EditorGUILayout.LabelField("Height", GUILayout.MaxWidth(40));
                    serializedObject.FindProperty("resolutionHeight").intValue = EditorGUILayout.IntField(serializedObject.FindProperty("resolutionHeight").intValue);
                    if (serializedObject.FindProperty("resolutionHeight").intValue < 5)
                        serializedObject.FindProperty("resolutionHeight").intValue = 5;

                    // CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("resolutionWidth"), new GUIContent("Width"));
                    // CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("resolutionHeight"), new GUIContent("Height"));
                    EditorGUILayout.EndHorizontal();
                }

                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("overrideCutoutAdjustedRectTransforms"), new GUIContent("Override Adjusted Rect Transforms"));
                if (serializedObject.FindProperty("overrideCutoutAdjustedRectTransforms").boolValue)
                {
                    cutoutAdjustedRectTransformsList.DoLayoutList();
                    EditorGUILayout.HelpBox("(Overrides default cutout setting.) Select transforms that should be adjusted to fit within the cutout area (for example logos, title text, or other overlays). This effect is done by temporarily parenting the rect transform within the cutout, so only the top level rect transform is needed.", MessageType.Info);
                }

                cutoutScriptsList.DoLayoutList();
            }

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = originalGUIEnabled;
        }

        public void Button()
        {
            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            if (GUILayout.Button(new GUIContent("Take Cutout Set", "Takes a set of cutouts at the selected resolution."), GUILayout.MinHeight(40)))
                ((CutoutScreenshotSetScript)target).TakeCutoutSet();
            GUI.enabled = originalGUIEnabled;
        }
    }
}