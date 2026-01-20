using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class ScreenshotResolutionTransformationReorderableList : ReorderableList
    {
        class ScreenshotResolutionTransformationCreationParams
        {
            public bool active;
            public string name;
        }

        public ScreenshotResolutionTransformationReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty) : base(serializedObject, serializedProperty)
        {
            List<string> headerTexts = new List<string>(new string[] { "Active", "Resolution Name", "Transformation" });
            List<string> properties = new List<string>(new string[] { "active", "screenshotResolutionName", "textureTransformation" });
            float[] widths = new float[] { 0.08f, 0.55f, 0.44f, };

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
                SerializedProperty textureTransformationProperty = element.FindPropertyRelative("textureTransformation");
                if (textureTransformationProperty.objectReferenceValue == null)
                    textureTransformationProperty.objectReferenceValue = TextureTransformation.SolidifyTextureTransformation();

                SerializedObject textureTransformationObject = new SerializedObject(textureTransformationProperty.objectReferenceValue);
                SerializedProperty typeProperty = textureTransformationObject.FindProperty("type");
                for (int i = 0; i < properties.Count; ++i)
                {
                    rect.x += lastWidth + spacing;
                    rect.width = firstLineWidth * widths[i];

                    // Custom handle the enum selection field to handle value changes.
                    if (i == properties.Count - 1)
                    {
                        System.Type transformationType = textureTransformationObject.targetObject.GetType();
                        List<string> transformationTypeNames = new List<string>(typeof(TextureTransformation).GetSubclassNames());
                        int oldSelectedIndex = transformationTypeNames.IndexOf(transformationType.Name);
                        transformationTypeNames = transformationTypeNames.Select(x => x.Replace("TextureTransformation", "")).ToList<string>();
                        int newSelectedIndex = EditorGUI.Popup(rect, oldSelectedIndex, transformationTypeNames.ToArray());
                        if (newSelectedIndex != oldSelectedIndex)
                        {
                            System.Type[] transformationTypes = typeof(TextureTransformation).GetSubclasses();
                            transformationType = transformationTypes[newSelectedIndex];
                            TextureTransformation textureTransformation = (TextureTransformation)ScriptableObject.CreateInstance(transformationType);
                            textureTransformationObject = new SerializedObject(textureTransformation);
                        }
                    }
                    else
                        CustomEditorGUI.PropertyField(rect, element.FindPropertyRelative(properties[i]), GUIContent.none);
                    lastWidth = rect.width;
                }
                rect.x = originalX;
                rect.width = firstLineWidth;
                rect.y += EditorGUIUtility.standardVerticalSpacing + rect.height;

                SerializedProperty transformationProperty = textureTransformationObject.GetIterator();
                transformationProperty.Next(true);
                if (transformationProperty.NextVisible(transformationProperty.hasChildren))
                {
                    do
                    {
                        if (transformationProperty.name == "m_Script" || transformationProperty.name == "active") continue;
                        rect.height = EditorGUI.GetPropertyHeight(transformationProperty, transformationProperty.isExpanded);
                        TextureTransformation textureTransformation = (TextureTransformation)textureTransformationObject.targetObject;
                        if (!textureTransformation.PropertyNames().Contains(transformationProperty.name)) continue;
                        CustomEditorGUI.PropertyField(rect, transformationProperty, new GUIContent(textureTransformation.LabelForPropertyName(transformationProperty.name)), transformationProperty.hasVisibleChildren && transformationProperty.isExpanded);
                        rect.y += EditorGUIUtility.standardVerticalSpacing + rect.height;
                    }
                    while (transformationProperty.NextVisible(false));
                }

                textureTransformationObject.ApplyModifiedProperties();
                textureTransformationProperty.objectReferenceValue = textureTransformationObject.targetObject;
                serializedProperty.serializedObject.ApplyModifiedProperties();
            };

            onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                ScreenshotScript screenshotScript = ((ScreenshotScript)serializedObject.targetObject);
                List<ScreenshotResolution> screenshotResolutions = screenshotScript.screenshotResolutions;

                GenericMenu menu = new GenericMenu();
                foreach (ScreenshotResolution screenshotResolution in screenshotResolutions)
                {
                    ScreenshotResolutionTransformationCreationParams creationParams = new ScreenshotResolutionTransformationCreationParams() { active = true, name = screenshotResolution.name };
                    menu.AddItem(new GUIContent(screenshotResolution.name), false, clickHandler, creationParams);
                }

                menu.ShowAsContext();
            };
        }

        float elementHeightHandler(int index)
        {
            if (serializedProperty.arraySize <= 0) return 0;

            SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty textureTransformationProperty = element.FindPropertyRelative("textureTransformation");
            if (textureTransformationProperty.objectReferenceValue == null)
                textureTransformationProperty.objectReferenceValue = TextureTransformation.SolidifyTextureTransformation();
            SerializedObject textureTransformationObject = new SerializedObject(textureTransformationProperty.objectReferenceValue);

            // Type selection field + properties
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            SerializedProperty transformationProperty = textureTransformationObject.GetIterator();
            transformationProperty.Next(true);
            if (transformationProperty.NextVisible(true))
            {
                do
                {
                    if (transformationProperty.name == "m_Script" || transformationProperty.name == "active") continue;
                    height += EditorGUI.GetPropertyHeight(transformationProperty, transformationProperty.isExpanded);
                    height += EditorGUIUtility.standardVerticalSpacing;
                }
                while (transformationProperty.NextVisible(false));
            }
            return height;
        }

        void clickHandler(object creationParams)
        {
            ScreenshotResolutionTransformationCreationParams data = (ScreenshotResolutionTransformationCreationParams)creationParams;

            int newIndex = serializedProperty.arraySize;
            serializedProperty.arraySize++;

            var element = serializedProperty.GetArrayElementAtIndex(newIndex);
            element.FindPropertyRelative("active").boolValue = data.active;
            element.FindPropertyRelative("screenshotResolutionName").stringValue = data.name;
            element.FindPropertyRelative("textureTransformation").objectReferenceValue = TextureTransformation.SolidifyTextureTransformation() as System.Object as UnityEngine.Object;

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}