using UnityEngine;
using UnityEditor;

namespace TRS.CaptureTool.Extras
{
    public static class SavePathHelpers
    {
        public static void SaveFolderPropertyWithBrowseButton(SerializedProperty serializedProperty, GUIContent label = null, string panelTitle = "", string defaultPath = "")
        {
            if (label == null)
                label = new GUIContent(serializedProperty.displayName);

            EditorGUILayout.BeginHorizontal();
            string currentDirectory = serializedProperty.stringValue;
            serializedProperty.stringValue = EditorGUILayout.TextField(label, currentDirectory, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
                SaveFolderPanelForProperty(serializedProperty, panelTitle, defaultPath);
            EditorGUILayout.EndHorizontal();
        }

        public static void SaveFolderPropertyWithBrowseButton(Rect rect, SerializedProperty serializedProperty, GUIContent label = null, string panelTitle = "", string defaultPath = "", float buttonWidth = 60.0f)
        {
            if (label == null)
                label = new GUIContent(serializedProperty.displayName);

            rect.width -= buttonWidth;

            EditorGUILayout.BeginHorizontal();
            string currentDirectory = serializedProperty.stringValue;
            serializedProperty.stringValue = EditorGUI.TextField(rect, label, currentDirectory);

            rect.x += rect.width;
            rect.width = buttonWidth;
            if (GUI.Button(rect, "Browse"))
                SavePathHelpers.SaveFolderPanelForProperty(serializedProperty);
            EditorGUILayout.EndHorizontal();
        }

        public static void SaveFilePropertyWithBrowseButton(SerializedProperty serializedProperty, GUIContent label = null, string panelTitle = "", string directory = "", string defaultName = "", string extension = "")
        {
            if (label == null)
                label = new GUIContent(serializedProperty.displayName);

            EditorGUILayout.BeginHorizontal();
            string currentFile = serializedProperty.stringValue;
            serializedProperty.stringValue = EditorGUILayout.TextField(label, currentFile, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
                SaveFilePanelForProperty(serializedProperty, panelTitle, directory, defaultName, extension);
            EditorGUILayout.EndHorizontal();
        }

        public static void SaveFilePropertyWithBrowseButton(Rect rect, SerializedProperty serializedProperty, GUIContent label = null, string panelTitle = "", string directory = "", string defaultName = "", string extension = "", float buttonWidth = 60.0f)
        {
            if (label == null)
                label = new GUIContent(serializedProperty.displayName);

            rect.width -= buttonWidth;

            EditorGUILayout.BeginHorizontal();
            string currentFile = serializedProperty.stringValue;
            serializedProperty.stringValue = EditorGUI.TextField(rect, label, currentFile);

            rect.x += rect.width;
            rect.width = buttonWidth;
            if (GUI.Button(rect, "Browse"))
                SaveFilePanelForProperty(serializedProperty, panelTitle, directory, defaultName, extension);
            EditorGUILayout.EndHorizontal();
        }

        public static void OpenFilePropertyWithBrowseButton(SerializedProperty serializedProperty, GUIContent label = null, string panelTitle = "", string directory = "", string extension = "")
        {
            if (label == null)
                label = new GUIContent(serializedProperty.displayName);

            EditorGUILayout.BeginHorizontal();
            string currentFile = serializedProperty.stringValue;
            serializedProperty.stringValue = EditorGUILayout.TextField(label, currentFile, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
                OpenFilePanelForProperty(serializedProperty, panelTitle, directory, extension);
            EditorGUILayout.EndHorizontal();
        }

        public static void OpenFilePropertyWithBrowseButton(Rect rect, SerializedProperty serializedProperty, GUIContent label = null, string panelTitle = "", string directory = "", string extension = "", float buttonWidth = 60.0f)
        {
            if (label == null)
                label = new GUIContent(serializedProperty.displayName);

            rect.width -= buttonWidth;

            EditorGUILayout.BeginHorizontal();
            string currentFile = serializedProperty.stringValue;
            serializedProperty.stringValue = EditorGUI.TextField(rect, label, currentFile);

            rect.x += rect.width;
            rect.width = buttonWidth;
            if (GUI.Button(rect, "Browse"))
                OpenFilePanelForProperty(serializedProperty, panelTitle, directory, extension);
            EditorGUILayout.EndHorizontal();
        }

        public static void RequestSavePath(SerializedProperty serializedProperty, string panelTitle = "", string defaultPath = "")
        {
            string currentDirectory = serializedProperty.stringValue;
            if (currentDirectory == "")
                SaveFolderPanelForProperty(serializedProperty, panelTitle, defaultPath);
        }

        public static void SaveFolderPanelForProperty(SerializedProperty serializedProperty, string panelTitle = "", string defaultPath = "")
        {
            panelTitle = string.IsNullOrEmpty(panelTitle) ? "Folder for " + serializedProperty.name : panelTitle;
            defaultPath = string.IsNullOrEmpty(defaultPath) ? Application.dataPath : defaultPath;

            string currentDirectory = serializedProperty.stringValue;
            string newDirectory = EditorUtility.SaveFolderPanel(panelTitle, currentDirectory, defaultPath);
            if (newDirectory.Length > 0)
            {
                serializedProperty.stringValue = newDirectory;
                serializedProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        public static void OpenFilePanelForProperty(SerializedProperty serializedProperty, string panelTitle = "", string directory = "", string extension = "")
        {
            string extensionTitleAddOn = !string.IsNullOrEmpty(extension) ? " with extension: ." + extension : "";
            panelTitle = string.IsNullOrEmpty(panelTitle) ? "File path for " + serializedProperty.name + extensionTitleAddOn : panelTitle;
            directory = string.IsNullOrEmpty(directory) ? Application.dataPath : directory;
            directory = !string.IsNullOrEmpty(serializedProperty.stringValue) ? System.IO.Path.GetDirectoryName(serializedProperty.stringValue) : directory;

            string newFile = EditorUtility.OpenFilePanel(panelTitle, directory, extension);
            if (newFile.Length > 0)
            {
                serializedProperty.stringValue = newFile;
                serializedProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        public static void SaveFilePanelForProperty(SerializedProperty serializedProperty, string panelTitle = "", string directory = "", string defaultName = "", string extension = "")
        {
            string extensionTitleAddOn = !string.IsNullOrEmpty(extension) ? " with extension: ." + extension : "";
            panelTitle = string.IsNullOrEmpty(panelTitle) ? "File path for " + serializedProperty.name + extensionTitleAddOn : panelTitle;
            directory = string.IsNullOrEmpty(directory) ? Application.dataPath : directory;
            directory = !string.IsNullOrEmpty(serializedProperty.stringValue) ? System.IO.Path.GetDirectoryName(serializedProperty.stringValue) : directory;

            string newFile = EditorUtility.SaveFilePanel(panelTitle, directory, defaultName, extension);
            if (newFile.Length > 0)
            {
                serializedProperty.stringValue = newFile;
                serializedProperty.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}