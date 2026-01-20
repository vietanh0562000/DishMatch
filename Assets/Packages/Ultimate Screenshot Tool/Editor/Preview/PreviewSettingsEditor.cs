using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(PreviewSettings))]
    public class PreviewSettingsEditor : Editor
    {
        ReorderableList previewDeviceList;

        void OnEnable()
        {
            if (target == null)
                return;

            previewDeviceList = new PreviewDeviceReorderableList(serializedObject, serializedObject.FindProperty("previewDevices"));
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Devices");
            previewDeviceList.DoLayoutList();

            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("excludeFolderPathFromDeviceDisplayName"));

            string screenshotScriptTooltip = "Whether to use a screenshot script with additional settings like capture list, transformations, etc.";
            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("useScreenshotScript"), new GUIContent("Use Screenshot Script", screenshotScriptTooltip));

            SavePathHelpers.SaveFolderPropertyWithBrowseButton(serializedObject.FindProperty("saveDirectory"));

            if(Application.isPlaying)
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("saveAsynchronously"), new GUIContent("Save Asynchronously", "Only available while application is playing."));

            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("autoUpdate"));
            if (serializedObject.FindProperty("autoUpdate").boolValue)
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("autoUpdateDelay"));

            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("safeAreaEnabled"));
            if(serializedObject.FindProperty("safeAreaEnabled").boolValue)
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("safeAreaColor"));

            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("displayInGrid"));
            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("rotation"));

            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("scale"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
