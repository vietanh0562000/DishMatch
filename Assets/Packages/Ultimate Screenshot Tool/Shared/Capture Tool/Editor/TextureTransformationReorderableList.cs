using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class TextureTransformationReorderableList : ReorderableList
    {
        class TextureTransformationCreationParams
        {
            public string transformationTypeName;
            public TextureTransformation existingTextureTransformation;
        }

        public TextureTransformationReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty, string header, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(serializedObject, serializedProperty, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            List<string> headerTexts = new List<string>(new string[] { header });
            float[] widths = new float[] { 1f };
            this.AddHeader(headerTexts.ToArray(), widths, ReorderableListExtensions.SPACING_PERCENTAGE, -15f);

            this.elementHeightCallback += elementHeightHandler;

            this.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (this.serializedProperty.arraySize == 0) return;
                SerializedProperty element = this.serializedProperty.GetArrayElementAtIndex(index);
                if (element == null || element.objectReferenceValue == null)
                {
                    serializedProperty.DeleteArrayElementAtIndex(index);
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                    return;
                }

                SerializedObject textureTransformationObject = new SerializedObject(element.objectReferenceValue);

                float originalX = rect.x;
                float originalWidth = rect.width;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = originalWidth * 0.7f;

                System.Type transformationType = textureTransformationObject.targetObject.GetType();
                List<string> transformationTypeNames = new List<string>(typeof(TextureTransformation).GetSubclassNames());
                int oldSelectedIndex = transformationTypeNames.IndexOf(transformationType.Name);
                transformationTypeNames = transformationTypeNames.Select(x => x.Replace("TextureTransformation", "")).ToList<string>();
                int newSelectedIndex = EditorGUI.Popup(rect, oldSelectedIndex, transformationTypeNames.ToArray());
                if(newSelectedIndex != oldSelectedIndex)
                {
                    System.Type[] transformationTypes = typeof(TextureTransformation).GetSubclasses();
                    transformationType = transformationTypes[newSelectedIndex];
                    TextureTransformation textureTransformation = (TextureTransformation)ScriptableObject.CreateInstance(transformationType);
                    textureTransformationObject = new SerializedObject(textureTransformation);
                }

                // Trick to better align this field. The first property only displays the label and the second only displays the checkbox.
                float padding = 10.0f;
                rect.x += rect.width + padding;
                rect.width = originalWidth * 0.15f;
                SerializedProperty activeProperty = textureTransformationObject.FindProperty("active");
                CustomEditorGUI.PropertyField(rect, activeProperty);

                rect.x += rect.width + padding;
                rect.width = originalWidth * 0.1f;
                activeProperty = textureTransformationObject.FindProperty("active");
                CustomEditorGUI.PropertyField(rect, activeProperty, GUIContent.none);
                
                rect.x = originalX;
                rect.width = originalWidth;
                rect.y += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(activeProperty, activeProperty.isExpanded);

                SerializedProperty transformationProperty = textureTransformationObject.GetIterator();
                transformationProperty.Next(true);
                if (transformationProperty.NextVisible(true))
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
                element.objectReferenceValue = textureTransformationObject.targetObject;
                serializedProperty.serializedObject.ApplyModifiedProperties();
            };

            onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                GenericMenu menu = new GenericMenu();
                string[] transformationTypeNames = typeof(TextureTransformation).GetSubclassNames();
                foreach (string transformationTypeName in transformationTypeNames)
                {
                    // Skip the single transformations to clean up the UI.
                    if (transformationTypeName.Equals("ShaderTextureTransformation") || transformationTypeName.Equals("MaterialTextureTransformation")) continue;

                    string folder = "";
                    if (transformationTypeName.Contains("Layer")) folder = "Layer/";

                    TextureTransformationCreationParams creationParams = new TextureTransformationCreationParams() { transformationTypeName = transformationTypeName };
                    menu.AddItem(new GUIContent(folder + transformationTypeName.Replace("TextureTransformation", "")), false, clickHandler, creationParams);
                }

                string[] guids = AssetDatabase.FindAssets("t:" + typeof(TextureTransformation).Name); 
                TextureTransformation[] existingTextureTransformations = new TextureTransformation[guids.Length];
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    string textureTransformationName = System.IO.Path.GetFileNameWithoutExtension(path);
                    TextureTransformation textureTransformation = AssetDatabase.LoadAssetAtPath<TextureTransformation>(path);
                    TextureTransformationCreationParams creationParams = new TextureTransformationCreationParams() { existingTextureTransformation = textureTransformation };
                    menu.AddItem(new GUIContent("Shared/" + textureTransformationName), false, clickHandler, creationParams);
                    creationParams = new TextureTransformationCreationParams() { existingTextureTransformation = textureTransformation.Clone() };
                    menu.AddItem(new GUIContent("Copy/" + textureTransformationName), false, clickHandler, creationParams);
                }
                menu.ShowAsContext();
            };
        }

        float elementHeightHandler(int index)
        {
            if (serializedProperty.arraySize <= 0) return 0;

            SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);
            if (element == null || element.objectReferenceValue == null)
            {
                serializedProperty.serializedObject.ApplyModifiedProperties();
                return 0;
            }

            SerializedObject textureTransformationObject = new SerializedObject(element.objectReferenceValue);

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
            TextureTransformationCreationParams frameTransformationCreationParams = (TextureTransformationCreationParams)creationParams;

            int newIndex = serializedProperty.arraySize;
            serializedProperty.arraySize++;

            SerializedProperty element = serializedProperty.GetArrayElementAtIndex(newIndex);
            if(frameTransformationCreationParams.existingTextureTransformation != null)
                element.objectReferenceValue = frameTransformationCreationParams.existingTextureTransformation;
            else
            {
                element.objectReferenceValue = TextureTransformation.SolidifyTextureTransformation();
                SerializedObject textureTransformationObject = new SerializedObject(element.objectReferenceValue);
                System.Type defaultTransformationType = textureTransformationObject.targetObject.GetType();

                List<string> transformationTypeNames = new List<string>(typeof(TextureTransformation).GetSubclassNames());
                int typeIndex = transformationTypeNames.IndexOf(frameTransformationCreationParams.transformationTypeName);
                System.Type transformationType = typeof(TextureTransformation).GetSubclasses()[typeIndex];
                if (transformationType != defaultTransformationType)
                {
                    TextureTransformation textureTransformation = (TextureTransformation)ScriptableObject.CreateInstance(transformationType);
                    textureTransformationObject = new SerializedObject(textureTransformation);
                }

                textureTransformationObject.FindProperty("active").boolValue = true;

                textureTransformationObject.ApplyModifiedProperties();
                element.objectReferenceValue = textureTransformationObject.targetObject;
            }

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}