using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool.Extras
{
    [CustomEditor(typeof(DebugInfoScript))]
    public class DebugInfoScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InfoSettings();

            serializedObject.ApplyModifiedProperties();
        }

        void InfoSettings()
        {
            Text currentDebugInfoText = (Text)serializedObject.FindProperty("debugInfoText").objectReferenceValue;
            if (currentDebugInfoText == null)
                currentDebugInfoText = ((DebugInfoScript)target).gameObject.GetComponent<Text>();

            serializedObject.FindProperty("debugInfoText").objectReferenceValue = EditorGUILayout.ObjectField("Text", currentDebugInfoText, typeof(Text), true);

            EditorGUILayout.LabelField("Example: " + ((DebugInfoScript)target).debugText);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            Color lightGrey = (Color.grey + Color.white) / 2f;
            const string buttonGroup = "toolbarButton";

            bool includeCustomText = serializedObject.FindProperty("includeCustomText").boolValue;
            GUI.color = includeCustomText ? lightGrey : Color.white;
            if (GUILayout.Button("Custom Text", buttonGroup))
                serializedObject.FindProperty("includeCustomText").boolValue = !includeCustomText;

            bool includeProjectName = serializedObject.FindProperty("includeProject").boolValue;
            GUI.color = includeProjectName ? lightGrey : Color.white;
            if (GUILayout.Button("Project", buttonGroup))
                serializedObject.FindProperty("includeProject").boolValue = !includeProjectName;

            bool includeScene = serializedObject.FindProperty("includeScene").boolValue;
            GUI.color = includeScene ? lightGrey : Color.white;
            if (GUILayout.Button("Scene", buttonGroup))
                serializedObject.FindProperty("includeScene").boolValue = !includeScene;

            bool includeVersion = serializedObject.FindProperty("includeVersion").boolValue;
            GUI.color = includeVersion ? lightGrey : Color.white;
            if (GUILayout.Button("Version", buttonGroup))
                serializedObject.FindProperty("includeVersion").boolValue = !includeVersion;

            bool includeBuild = serializedObject.FindProperty("includeBuild").boolValue;
            GUI.color = includeBuild ? lightGrey : Color.white;
            if (GUILayout.Button("Build", buttonGroup))
                serializedObject.FindProperty("includeBuild").boolValue = !includeBuild;

            bool includeDate = serializedObject.FindProperty("includeDate").boolValue;
            GUI.color = includeDate ? lightGrey : Color.white;
            if (GUILayout.Button("Date", buttonGroup))
                serializedObject.FindProperty("includeDate").boolValue = !includeDate;

            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
                serializedObject.FindProperty("debugInfoSettingsChanged").boolValue = true;

            if (!serializedObject.FindProperty("debugInfoSettingsChanged").boolValue)
                EditorGUILayout.HelpBox("Toggle the above tabs to add or remove that component from the debug info. (And to dismiss this message.)", MessageType.Info);


            if (serializedObject.FindProperty("includeCustomText").boolValue)
            {
                string currentCustomText = serializedObject.FindProperty("customText").stringValue;
                serializedObject.FindProperty("customText").stringValue = EditorGUILayout.TextField("Custom Text", currentCustomText);
            }

            if (serializedObject.FindProperty("includeDate").boolValue)
            {
                string currentDateFormat = serializedObject.FindProperty("dateFormat").stringValue;
                serializedObject.FindProperty("dateFormat").stringValue = EditorGUILayout.TextField("Date Format", currentDateFormat);
            }
        }
    }
}