using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class ScreenshotResolutionLiveFrameReorderableList : ReorderableList
    {
        class ScreenshotResolutionLiveFrameCreationParams
        {
            public bool active;
            public string name;
        }

        public ScreenshotResolutionLiveFrameReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty) : base(serializedObject, serializedProperty)
        {
            List<string> headerTexts = new List<string>(new string[] { "Active", "Resolution Name" });
            List<string> properties = new List<string>(new string[] { "active", "screenshotResolutionName" });
            float[] widths = new float[] { 0.08f, 0.99f };

            this.AddHeader(headerTexts.ToArray(), widths, ReorderableListExtensions.SPACING_PERCENTAGE, -15f);
            this.elementHeightCallback += elementHeightHandler;
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);

                float originalX = rect.x;
                float originalFullWidth = rect.width;
                float firstLineWidth = rect.width - rect.x;
                float spacing = rect.width * ReorderableListExtensions.SPACING_PERCENTAGE;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 2;

                float lastWidth = 0f;
                for (int i = 0; i < properties.Count; ++i)
                {
                    rect.x += lastWidth + spacing;
                    rect.width = firstLineWidth * widths[i];
                    CustomEditorGUI.PropertyField(rect, element.FindPropertyRelative(properties[i]), GUIContent.none);
                    lastWidth = rect.width;
                }

                if (element.FindPropertyRelative("active").boolValue)
                {
                    rect.x = originalX;
                    rect.width = firstLineWidth;
                    rect.y += EditorGUIUtility.standardVerticalSpacing + rect.height;
                    EditorGUI.PropertyField(rect, element.FindPropertyRelative("liveFrame"), new GUIContent("Reorderable List Screenshot Resolution Live Frame"));
                } else
                {
                    rect.x = originalX;
                    rect.width = firstLineWidth;
                    rect.y += EditorGUIUtility.standardVerticalSpacing + rect.height;
                    CustomEditorGUI.PropertyField(rect, element.FindPropertyRelative("liveFrame").FindPropertyRelative("name"), new GUIContent("Live Frame Name"));
                }

                serializedProperty.serializedObject.ApplyModifiedProperties();
            };

            onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                ScreenshotScript screenshotScript = ((ScreenshotScript)serializedObject.targetObject);
                List<ScreenshotResolution> screenshotResolutions = screenshotScript.screenshotResolutions;

                GenericMenu menu = new GenericMenu();
                foreach (ScreenshotResolution screenshotResolution in screenshotResolutions)
                {
                    ScreenshotResolutionLiveFrameCreationParams creationParams = new ScreenshotResolutionLiveFrameCreationParams() { active = true, name = screenshotResolution.name };
                    menu.AddItem(new GUIContent(screenshotResolution.name), false, clickHandler, creationParams);
                }

                menu.ShowAsContext();
            };
        }

        float elementHeightHandler(int index)
        {
            if (serializedProperty.arraySize <= 0) return 0;

            SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);

            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if(element.FindPropertyRelative("active").boolValue)
            {
                SerializedProperty liveFrameProperty = element.FindPropertyRelative("liveFrame");
                height += EditorGUI.GetPropertyHeight(liveFrameProperty, new GUIContent("Reorderable List Screenshot Resolution Live Frame"), liveFrameProperty.isExpanded);
                height += EditorGUIUtility.standardVerticalSpacing;
            } else
            {
                SerializedProperty liveFrameProperty = element.FindPropertyRelative("liveFrame");
                height += EditorGUI.GetPropertyHeight(liveFrameProperty.FindPropertyRelative("name"), new GUIContent("Live Frame Name"));
                height += EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        void clickHandler(object creationParams)
        {
            ScreenshotResolutionLiveFrameCreationParams data = (ScreenshotResolutionLiveFrameCreationParams)creationParams;

            int newIndex = serializedProperty.arraySize;
            serializedProperty.arraySize++;

            var element = serializedProperty.GetArrayElementAtIndex(newIndex);
            element.FindPropertyRelative("active").boolValue = data.active;
            element.FindPropertyRelative("screenshotResolutionName").stringValue = data.name;
            element.FindPropertyRelative("liveFrame").FindPropertyRelative("active").boolValue = true;

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}