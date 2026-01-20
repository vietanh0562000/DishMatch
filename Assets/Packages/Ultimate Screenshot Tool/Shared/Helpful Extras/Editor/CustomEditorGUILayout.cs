using UnityEngine;
using UnityEditor;

namespace TRS.CaptureTool.Extras
{
    public static class CustomEditorGUILayout
    {
        public static bool PropertyField(SerializedProperty property, params GUILayoutOption[] options)
        {
            return PropertyField(property, new GUIContent(property.displayName), false, options);
        }

        public static bool PropertyField(SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            return PropertyField(property, label, false, options);
        }

        public static bool PropertyField(SerializedProperty property, bool includeChildren, params GUILayoutOption[] options)
        {
            return PropertyField(property, new GUIContent(property.displayName), includeChildren, options);
        }

        public static bool PropertyField(SerializedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options)
        {
            return PropertyField(property, label, EditorGUI.indentLevel, includeChildren, options);
        }

        public static bool PropertyField(SerializedProperty property, GUIContent label, int indentLevel, bool includeChildren, params GUILayoutOption[] options)
        {
            int originalIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indentLevel;

            Rect position = EditorGUILayout.GetControlRect(label.text.Length > 0, EditorGUI.GetPropertyHeight(property, label, includeChildren), options);
            position = EditorGUI.IndentedRect(position);
            CustomEditorGUI.PropertyField(position, property, label, includeChildren);

            EditorGUI.indentLevel = originalIndentLevel;

            return property.hasChildren && property.isExpanded && !includeChildren;
        }

        public static bool BoldFoldout(bool shown, string label)
        {
            GUIStyle style = EditorStyles.foldout;
            FontStyle prevFontStyle = style.fontStyle;

            style.fontStyle = FontStyle.Bold;
            bool newShown = GUILayout.Toggle(shown, label, "foldout"); // EditorGUILayout.Foldout(shown, label);
            style.fontStyle = prevFontStyle;
            return newShown;
        }

        public static bool BoldFoldoutForProperty(SerializedObject serializedObject, string propertyName, string label)
        {
            return BoldFoldoutForProperty(serializedObject.FindProperty(propertyName), label);
        }

        public static bool BoldFoldoutForProperty(SerializedProperty serializedProperty, string label)
        {
            bool currentValue = serializedProperty.boolValue;
            bool newValue = BoldFoldout(currentValue, label);
            serializedProperty.boolValue = newValue;
            return newValue;
        }
    }
}
