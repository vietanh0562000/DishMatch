using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class ScreenshotResolutionReorderableList : ReorderableList
    {
        class ScreenshotResolutionCreationParams
        {
            public string[] names;
            public Resolution[] resolutions;
        }

        int prevIndex = -1;

        public ScreenshotResolutionReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty, bool adjustScale, bool adjustDelay, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton, SerializedProperty allResolutionsProperty) : base(serializedObject, serializedProperty, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            /*
            string[] headerTexts = { "Active", "Name", "Width", "Height", "Scale", "Wait" };
            string[] properties = { "active", "name", "width", "height", "scale", "waitForUpdates" };
            float[] widths = { 0.12f, 0.38f, 0.14f, 0.14f, 0.1f, 0.08f };

            string[] headerTexts = { "Active", "Name", "Width", "Height", "Wait" };
            string[] properties = { "active", "name", "width", "height", "waitForUpdates" };
            float[] widths = { 0.1f, 0.5f, 0.14f, 0.14f, 0.08f };
            */

            List<string> headerTexts = new List<string>(new string[] { "Active", "Name", "Width", "Height" });
            List<string> properties = new List<string>(new string[] { "active", "name", "width", "height" });
            float[] widths = new float[] { 0.07f, 0.6f, 0.15f, 0.15f };

            if (adjustScale)
            {
                headerTexts.Add("Scale");
                properties.Add("scale");
            }
            if (adjustDelay)
            {
                headerTexts.Add("Delay");
                properties.Add("delay");
            }

            if (adjustScale && adjustDelay)
                widths = new float[] { 0.07f, 0.44f, 0.12f, 0.12f, 0.1f, 0.1f };
            else if (adjustScale || adjustDelay)
                widths = new float[] { 0.07f, 0.5f, 0.14f, 0.14f, 0.11f };

            this.AddHeader(headerTexts.ToArray(), widths, ReorderableListExtensions.SPACING_PERCENTAGE, -15f);
            this.AddStandardElementCallback(properties.ToArray(), widths);
            onSelectCallback += selectIndexHandler;
            onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                bool includeAllTypes = allResolutionsProperty == null || allResolutionsProperty.boolValue;
                Dictionary<string, Resolution> resolutionDictionary = AdditionalResolutions.All(includeAllTypes, true);

                GenericMenu menu = new GenericMenu();
                foreach (string resolutionName in resolutionDictionary.Keys)
                {
                    ScreenshotResolutionCreationParams creationParams = null;
                    if (resolutionDictionary[resolutionName].height != -1)
                        creationParams = new ScreenshotResolutionCreationParams() { names = new string[] { resolutionName }, resolutions = new Resolution[] { resolutionDictionary[resolutionName] } };
                    else
                    {
                        // This is a resolution group.
                        string[] names = AdditionalResolutions.resolutionGroup[resolutionName];
                        Resolution[] resolutions = new Resolution[names.Length];
                        for (int i = 0; i < names.Length; ++i)
                            resolutions[i] = resolutionDictionary[names[i]];
                        creationParams = new ScreenshotResolutionCreationParams() { names = names, resolutions = resolutions };
                    }

                    menu.AddItem(new GUIContent(resolutionName), false, clickHandler, creationParams);
                }

                menu.ShowAsContext();
            };
        }

        void selectIndexHandler(ReorderableList reorderableList)
        {
            if (reorderableList.index == prevIndex)
            {
                SerializedProperty screenshotResolutionProperty = serializedProperty.GetArrayElementAtIndex(reorderableList.index);
                int width = screenshotResolutionProperty.FindPropertyRelative("width").intValue;
                int height = screenshotResolutionProperty.FindPropertyRelative("height").intValue;
                GameView.SetSize(width, height);
            }
            else
                prevIndex = reorderableList.index;
        }

        void clickHandler(object creationParams)
        {
            var data = (ScreenshotResolutionCreationParams)creationParams;
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
                element.FindPropertyRelative("name").stringValue = AdditionalResolutions.ConvertToStructuredFolderName(name);
                element.FindPropertyRelative("width").intValue = resolution.width;
                element.FindPropertyRelative("height").intValue = resolution.height;
                element.FindPropertyRelative("scale").intValue = 1;
                element.FindPropertyRelative("waitForUpdates").boolValue = true;
            }

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}