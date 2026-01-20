using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool.Share
{
    [CustomPropertyDrawer(typeof(CustomUrl))]
    public class CustomUrlDrawer : PropertyDrawer
    {
        Dictionary<string, ReorderableList> parameterListForPropertyPath = new Dictionary<string, ReorderableList>();

        // This is hacky, but necessary
        const float approximateContainerHeight = 75f;
        const float approximateRowHeight = 22f;
        float ReorderablistListHeightForProperty(SerializedProperty property, GUIContent label)
        {
            int count = property.arraySize;
            if (count <= 0)
                return approximateContainerHeight;

            return approximateContainerHeight + approximateRowHeight * (count - 1);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = ReorderablistListHeightForProperty(property.FindPropertyRelative("parameters"), label);

            foreach (SerializedProperty childProperty in property.GetChildren())
            {
                if (childProperty.name != "parameters")
                    totalHeight += EditorGUI.GetPropertyHeight(childProperty, label, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float height = 0;
            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("displayName"));
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("baseUrl"));
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            Rect parametersRect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
            SerializedProperty parametersProperty = property.FindPropertyRelative("parameters");
            if (!parameterListForPropertyPath.ContainsKey(parametersProperty.propertyPath))
                parameterListForPropertyPath[parametersProperty.propertyPath] = new CustomUrlParameterReorderableList(property.serializedObject, parametersProperty, true, true, true, true, null);
            parameterListForPropertyPath[parametersProperty.propertyPath].DoList(parametersRect);

            EditorGUI.EndProperty();
        }
    }
}