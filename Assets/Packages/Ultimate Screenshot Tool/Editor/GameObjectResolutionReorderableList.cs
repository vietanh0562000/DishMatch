using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class GameObjectResolutionReorderableList : ReorderableList
    {

        class ScreenshotResolutionCreationParams
        {
            public string[] names;
            public Resolution[] resolutions;
        }

        int prevIndex = -1;

        public GameObjectResolutionReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty, bool adjustScale, bool adjustDelay, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton, SerializedProperty allResolutionsProperty) : base(serializedObject, serializedProperty, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            List<string> headerTexts = new List<string>(new string[] { "GameObject", "Name", "Width", "Height" });
            List<string> properties = new List<string>(new string[] { "gameObject", "resolution.name", "resolution.width", "resolution.height" });
            float[] widths = new float[] { 0.35f, 0.4f, 0.11f, 0.11f };

            if (adjustScale)
            {
                headerTexts.Add("Scale");
                properties.Add("resolution.scale");
            }
            if (adjustDelay)
            {
                headerTexts.Add("Delay");
                properties.Add("resolution.delay");
            }

            if (adjustScale && adjustDelay)
                widths = new float[] { 0.25f, 0.3f, 0.11f, 0.11f, 0.09f, 0.09f };
            else if (adjustScale || adjustDelay)
                widths = new float[] { 0.3f, 0.35f, 0.11f, 0.11f, 0.09f };

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
                        string[] names = AdditionalResolutions.resolutionGroup[resolutionName];
                        Resolution[] resolutions = new Resolution[names.Length];
                        for (int i = 0; i < names.Length; ++i)
                            resolutions[i] = resolutionDictionary[names[i]];
                        creationParams = new ScreenshotResolutionCreationParams() { names = names, resolutions = resolutions };
                    }

                    menu.AddItem(new GUIContent(resolutionName), false, addItemMenuClickHandler, creationParams);
                }

                menu.ShowAsContext();
            };
        }

        void selectIndexHandler(ReorderableList reorderableList)
        {
            if (reorderableList.index == prevIndex)
            {
                SerializedProperty pairProperty = serializedProperty.GetArrayElementAtIndex(reorderableList.index);
                if (pairProperty == null) return;
                SerializedProperty screenshotResolutionProperty = pairProperty.FindPropertyRelative("resolution");
                if (screenshotResolutionProperty == null) return;
                int width = screenshotResolutionProperty.FindPropertyRelative("width").intValue;
                int height = screenshotResolutionProperty.FindPropertyRelative("height").intValue;
                GameView.SetSize(width, height);
            }
            else
                prevIndex = reorderableList.index;
        }

        void addItemMenuClickHandler(object creationParams)
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
                var screenshotResolutionProperty = element.FindPropertyRelative("resolution");

                screenshotResolutionProperty.FindPropertyRelative("active").boolValue = true;
                screenshotResolutionProperty.FindPropertyRelative("name").stringValue = AdditionalResolutions.ConvertToStructuredFolderName(name);
                screenshotResolutionProperty.FindPropertyRelative("width").intValue = resolution.width;
                screenshotResolutionProperty.FindPropertyRelative("height").intValue = resolution.height;
                screenshotResolutionProperty.FindPropertyRelative("scale").intValue = 1;
                screenshotResolutionProperty.FindPropertyRelative("waitForUpdates").boolValue = true;
            }

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}