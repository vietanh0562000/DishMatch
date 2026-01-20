using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class PreviewDeviceReorderableList : ReorderableList
    {
        class PreviewDeviceCreationParams
        {
            public string[] names;
            public Resolution[] resolutions;
        }

        public PreviewDeviceReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty) : base(serializedObject, serializedProperty)
        {
            List<string> headerTexts = new List<string>(new string[] { "Active", "Name" });
            List<string> properties = new List<string>(new string[] { "active", "name" });
            float[] widths = new float[] { 0.08f, 0.99f };

            this.AddHeader(headerTexts.ToArray(), widths, ReorderableListExtensions.SPACING_PERCENTAGE, -15f);
            this.elementHeightCallback += elementHeightHandler;
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, new GUIContent("Reorderable List Preview Device"));

                serializedObject.ApplyModifiedProperties();
            };

            onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                Dictionary<string, Resolution> resolutionDictionary = AdditionalResolutions.All(true, true);

                GenericMenu menu = new GenericMenu();
                foreach (string resolutionName in resolutionDictionary.Keys)
                {
                    PreviewDeviceCreationParams creationParams = null;
                    if (resolutionDictionary[resolutionName].height != -1)
                        creationParams = new PreviewDeviceCreationParams() { names = new string[] { resolutionName }, resolutions = new Resolution[] { resolutionDictionary[resolutionName] } };
                    else
                    {
                        // This is a resolution group.
                        string[] names = AdditionalResolutions.resolutionGroup[resolutionName];
                        Resolution[] resolutions = new Resolution[names.Length];
                        for (int i = 0; i < names.Length; ++i)
                            resolutions[i] = resolutionDictionary[names[i]];
                        creationParams = new PreviewDeviceCreationParams() { names = names, resolutions = resolutions };
                    }

                    menu.AddItem(new GUIContent(resolutionName), false, clickHandler, creationParams);
                }

                menu.ShowAsContext();
            };
        }

        void clickHandler(object creationParams)
        {
            var data = (PreviewDeviceCreationParams)creationParams;
            for (int i = 0; i < data.names.Length; ++i)
            {
                string name = data.names[i];
                Resolution resolution = data.resolutions[i];
                if (AdditionalResolutions.resolutionForAspectRatio.ContainsKey(resolution))
                    resolution = AdditionalResolutions.resolutionForAspectRatio[resolution];

                int newIndex = serializedProperty.arraySize;
                serializedProperty.arraySize++;

                var element = serializedProperty.GetArrayElementAtIndex(newIndex);

                element.FindPropertyRelative("active").boolValue = true;
                element.FindPropertyRelative("deviceName").stringValue = AdditionalResolutions.ConvertToStructuredFolderName(name);
                element.FindPropertyRelative("type").intValue = (int)PreviewDevice.Type.Screenshot;
                element.FindPropertyRelative("sizingType").intValue = (int)PreviewDevice.SizingType.SetScreenshotToSize;
                element.FindPropertyRelative("width").intValue = resolution.width;
                element.FindPropertyRelative("height").intValue = resolution.height;

                if (resolution.height > resolution.width)
                    element.FindPropertyRelative("orientation").intValue = (int)ScreenOrientation.Portrait;
                else
                    element.FindPropertyRelative("orientation").intValue = (int)ScreenOrientation.LandscapeLeft;
            }

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }

        float elementHeightHandler(int index)
        {
            if (serializedProperty.arraySize <= 0) return 0;

            SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, new GUIContent("Reorderable List Preview Device"));
        }
    }
}