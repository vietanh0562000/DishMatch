using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace TRS.CaptureTool.Extras
{
    public static class ReorderableListExtensions
    {
        const float DIFF_IN_HEADER_SIZE_AND_ELEMENT_SIZE = 14f;
        public const float SPACING_PERCENTAGE = 0.01f;

        public static void AddHeader(this ReorderableList reorderableList, string headerText)
        {
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, headerText);
            };
        }

        public static void AddStandardElementCallback(this ReorderableList reorderableList)
        {
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                CustomEditorGUI.PropertyField(rect, element, GUIContent.none);
            };
        }

        public static void AddHeader(this ReorderableList reorderableList, string[] headerTexts, float[] widths, float spacingPercentage = SPACING_PERCENTAGE, float firstColumnOffset = 0, GUIStyle style = null)
        {
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                float initialOffset = DIFF_IN_HEADER_SIZE_AND_ELEMENT_SIZE;
                float originalWidth = rect.width - initialOffset;
                float spacing = originalWidth * spacingPercentage;

                float lastWidth = initialOffset - spacing + firstColumnOffset;
                for (int i = 0; i < headerTexts.Length; ++i)
                {
                    rect.x += lastWidth + spacing;
                    rect.width = originalWidth * widths[i];
                    if (i == 0)
                        rect.width -= firstColumnOffset;

                    if (style == null)
                        style = EditorStyles.label;
                    EditorGUI.LabelField(rect, headerTexts[i], style);
                    lastWidth = rect.width;
                }
            };
        }

        public static void AddStandardElementCallback(this ReorderableList reorderableList, string[] properties, float[] widths, float spacingPercentage = SPACING_PERCENTAGE)
        {
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

                float originalWidth = rect.width;
                float spacing = rect.width * spacingPercentage;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 2;

                float lastWidth = 0f;
                for (int i = 0; i < properties.Length; ++i)
                {
                    rect.x += lastWidth + spacing;
                    rect.width = originalWidth * widths[i];

                    CustomEditorGUI.PropertyField(rect, element.FindPropertyRelative(properties[i]), GUIContent.none);
                    lastWidth = rect.width;
                }
            };
        }
    }
}
