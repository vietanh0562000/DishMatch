using UnityEngine;
using UnityEditor;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(TextureCombinator))]
    public class TextureCombinatorEditor : Editor
    {
        const float MAX_HEIGHT = 250.0F;

        Texture2D preview;

        public override void OnInspectorGUI()
        {
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            serializedObject.Update();

            if (preview != null)
            {
                float previewScale = EditorGUIUtility.currentViewWidth / (float)preview.width;
                Rect position = EditorGUILayout.GetControlRect(false, Mathf.Min(preview.height * previewScale, MAX_HEIGHT));
                if (Event.current.type.Equals(EventType.Repaint))
                    GUI.DrawTexture(position, preview, ScaleMode.ScaleToFit);
            }

            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundTexture"));
            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("foregroundTexture"));

            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("foregroundPosition"));
            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("foregroundPositionPoint"));
            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("positionForegroundRelative"));

            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("overlapOnly"));
            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("emptySpaceFillColor"));
            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("useAlphaBlend"));
            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("solidify"));

            bool showSaveSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSaveSettings", "Save Settings");
            if (showSaveSettings)
            {
                SerializedProperty fileSettingsProperty = serializedObject.FindProperty("fileSettings");
                FileSettingsEditorHelper.ScreenshotFileSettingsFields(((TextureCombinator)target).fileSettings, fileSettingsProperty, true, false, false);
            }
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Update Preview", GUILayout.MinHeight(40)))
            {
                preview = ((TextureCombinator)target).Texture();
                preview.Apply();
            }

            if (GUILayout.Button("Save", GUILayout.MinHeight(40)))
                ((TextureCombinator)target).Save();

            EditorGUILayout.Space();

            OpenFileOrFolderButtons();
        }

        protected void OpenFileOrFolderButtons()
        {
            string filePath = serializedObject.FindProperty("lastSaveFilePath").stringValue;
            SerializedProperty captureFileSettingsProperty = serializedObject.FindProperty("fileSettings");
            string folderPath = captureFileSettingsProperty.FindPropertyRelative("cachedEditorDirectory").stringValue;

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !string.IsNullOrEmpty(filePath);
            if (GUILayout.Button("View Last Combined Texture", GUILayout.MinHeight(40)))
            {
                Application.OpenURL("file:///" + System.Uri.EscapeUriString(filePath));
                Debug.Log("Opening File " + filePath);
            }

            GUI.enabled = originalGUIEnabled && !string.IsNullOrEmpty(folderPath);
            if (GUILayout.Button("View Combined Textures Folder", GUILayout.MinHeight(60)))
            {
                Application.OpenURL("file:///" + System.Uri.EscapeUriString(folderPath));
                Debug.Log("Opening Directory " + folderPath);
            }
            GUI.enabled = originalGUIEnabled;
        }
    }
}