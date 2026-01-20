using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class LiveFrameReorderableList : ReorderableList
    {
        public LiveFrameReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty) : base(serializedObject, serializedProperty)
        {
            List<string> headerTexts = new List<string>(new string[] { "Active", "Name" });
            List<string> properties = new List<string>(new string[] { "active", "name" });
            float[] widths = new float[] { 0.08f, 0.99f };

            this.AddHeader(headerTexts.ToArray(), widths, ReorderableListExtensions.SPACING_PERCENTAGE, -15f);
            this.elementHeightCallback += elementHeightHandler;
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, new GUIContent("Reorderable List Live Frame"));

                serializedObject.ApplyModifiedProperties();
            };

            onAddCallback = (ReorderableList list) =>
            {
                int newIndex = serializedProperty.arraySize;
                serializedProperty.arraySize++;

                var element = serializedProperty.GetArrayElementAtIndex(newIndex);
                element.FindPropertyRelative("active").boolValue = true;

                serializedProperty.serializedObject.ApplyModifiedProperties();
            };
        }

        float elementHeightHandler(int index)
        {
            if (serializedProperty.arraySize <= 0) return 0;

            SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, new GUIContent("Reorderable List Live Frame"));
        }
    }
}