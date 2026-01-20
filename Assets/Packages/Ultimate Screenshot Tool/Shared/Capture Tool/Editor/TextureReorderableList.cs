using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class TextureReorderableList : ReorderableList
    {
        float maxHeight;

        public TextureReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty, string header, float maxHeight, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(serializedObject, serializedProperty, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            this.maxHeight = maxHeight;

            List<string> headerTexts = new List<string>(new string[] { header });
            float[] widths = new float[] { 1f };
            this.AddHeader(headerTexts.ToArray(), widths, ReorderableListExtensions.SPACING_PERCENTAGE, -15f);
            this.elementHeightCallback += elementHeightHandler;

            this.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);

                Texture2D texture = element.objectReferenceValue as Texture2D;

                if (texture != null)
                {
                    float previewScale = EditorGUIUtility.currentViewWidth / (float)texture.width;
                    rect.height = texture.height * previewScale;
                    if (maxHeight > 0)
                        rect.height = Mathf.Min(rect.height, maxHeight);
                    if (Event.current.type.Equals(EventType.Repaint))
                        GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
                    rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
                }

                rect.height = EditorGUIUtility.singleLineHeight;
                CustomEditorGUI.PropertyField(rect, element);
                rect.y += rect.height;

                serializedProperty.serializedObject.ApplyModifiedProperties();
            };
        }

        float elementHeightHandler(int index)
        {
            if (serializedProperty.arraySize <= 0) return 0;

            SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);
            Texture2D texture = element.objectReferenceValue as Texture2D;
            float height = 0;
            if (texture != null)
            {
                float previewScale = EditorGUIUtility.currentViewWidth / (float)texture.width;
                float textureHeight = texture.height * previewScale;
                if (maxHeight > 0)
                    textureHeight = Mathf.Min(textureHeight, maxHeight);
                height += textureHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
