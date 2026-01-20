using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [CustomPropertyDrawer(typeof(LiveFrame))]
    public class LiveFrameDrawer : PropertyDrawer
    {
        Dictionary<string, ReorderableList> reorderableListForPropertyPath = new Dictionary<string, ReorderableList>();

        // This is hacky, but necessary
        const float approximateSingleRowHeight = 59f;
        const float approximateRowHeight = 21f;
        float ReorderablistListHeightEstimateForProperty(SerializedProperty property, GUIContent label)
        {
            int count = property.arraySize;
            if (count <= 0)
                return EditorGUI.GetPropertyHeight(property, label, true) + 7f;

            return approximateSingleRowHeight + approximateRowHeight * (count - 1);
        }

        List<string> reorderableListBackedProperties = new List<string> { "tempEnabledObjects", "tempDisabledObjects", "transformations" };
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.FindPropertyRelative("active").boolValue)
            {
                return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("active"), label, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            float totalHeight = 0;
            foreach (SerializedProperty childProperty in property.GetChildren())
            {
                if (childProperty.name == "resolutionWidth" && !property.FindPropertyRelative("overrideResolution").boolValue)
                    continue;

                if (childProperty.name == "gameObjectRequiredToBeActive" && !property.FindPropertyRelative("activeOnlyIfGameObjectActive").boolValue)
                    continue;

                // Always skip as active and name are on the same line.
                if (childProperty.name == "active") continue;
                // Always skip as width and height are on the same line.
                if (childProperty.name == "resolutionHeight") continue;
                // Skip variable.
                if (childProperty.name == "originalResolution") continue;

                if (reorderableListBackedProperties.Contains(childProperty.name))
                {
                    if (reorderableListForPropertyPath.ContainsKey(childProperty.propertyPath))
                        totalHeight += reorderableListForPropertyPath[childProperty.propertyPath].GetHeight();
                    else
                        totalHeight += ReorderablistListHeightEstimateForProperty(childProperty, label);
                }
                else
                    totalHeight += EditorGUI.GetPropertyHeight(childProperty, label, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float height = 0;
            EditorGUI.BeginProperty(position, label, property);
            Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            if(label.text.Equals("Reorderable List Screenshot Resolution Live Frame"))
            {
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("name"), new GUIContent("Live Frame Name"));
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            } else if (label.text.Equals("Reorderable List Live Frame"))
            {
                float activeWidth = position.width * 0.08f;
                rect.width = activeWidth;
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("active"), new GUIContent(""));
                rect.x += activeWidth;
                rect.width = position.width - activeWidth;

                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("name"), new GUIContent(""));
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            } else
            {
                float activeWidth = position.width * 0.30f;
                float labelWidth = position.width * 0.2f;
                float padding = position.width * 0.02f;
                float positionX = position.x;

                rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rect, new GUIContent("Active"));
                positionX += rect.width;

                rect = new Rect(positionX, position.y + height, activeWidth - labelWidth, EditorGUIUtility.singleLineHeight);
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("active"), new GUIContent(""));
                positionX += rect.width;

                positionX += padding;
                rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rect, new GUIContent("Name"));
                positionX += rect.width;

                float remainingWidth = position.width - positionX + position.x;
                rect = new Rect(positionX, position.y + height, remainingWidth, EditorGUIUtility.singleLineHeight);
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("name"), new GUIContent(""));
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (!property.FindPropertyRelative("active").boolValue)
            {
                EditorGUI.EndProperty();
                return;
            }

            rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("textureDestination"));
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            string activeOnlyIfGameObjectActiveTooltip = "Whether to only use this live frame if a certain game object is active. Useful to tie a Live Frame to a particular screen.";
            rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("activeOnlyIfGameObjectActive"), new GUIContent("Active Only If GameObject Active", activeOnlyIfGameObjectActiveTooltip));
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if(property.FindPropertyRelative("activeOnlyIfGameObjectActive").boolValue)
            {
                rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("gameObjectRequiredToBeActive"));
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("overrideResolution"));
            height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (property.FindPropertyRelative("overrideResolution").boolValue)
            {
                float labelWidth = position.width * 0.15f;
                float fieldWidth = position.width * 0.34f;
                float padding = position.width * 0.02f;
                float positionX = position.x;

                rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rect, new GUIContent("Width"));
                positionX += rect.width;
                rect = new Rect(positionX, position.y + height, fieldWidth, EditorGUIUtility.singleLineHeight);
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("resolutionWidth"), new GUIContent(""));
                positionX += rect.width;

                positionX += padding;
                rect = new Rect(positionX, position.y + height, labelWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rect, new GUIContent("Height"));
                positionX += rect.width;

                rect = new Rect(positionX, position.y + height, fieldWidth, EditorGUIUtility.singleLineHeight);
                CustomEditorGUI.PropertyField(rect, property.FindPropertyRelative("resolutionHeight"), new GUIContent(""));
                positionX += rect.width;

                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            Rect tempEnabledObjectsRect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
            SerializedProperty tempEnabledObjectsProperty = property.FindPropertyRelative("tempEnabledObjects");
            if (!reorderableListForPropertyPath.ContainsKey(tempEnabledObjectsProperty.propertyPath))
            {
                reorderableListForPropertyPath[tempEnabledObjectsProperty.propertyPath] = new ReorderableList(property.serializedObject, tempEnabledObjectsProperty, true, true, true, true);
                reorderableListForPropertyPath[tempEnabledObjectsProperty.propertyPath].AddHeader("Temporarily Enabled Objects (Frame, logo, etc.)");
                reorderableListForPropertyPath[tempEnabledObjectsProperty.propertyPath].AddStandardElementCallback();
            }
            reorderableListForPropertyPath[tempEnabledObjectsProperty.propertyPath].DoList(tempEnabledObjectsRect);
            height += reorderableListForPropertyPath[tempEnabledObjectsProperty.propertyPath].GetHeight();

            Rect tempDisabledObjectsRect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
            SerializedProperty tempDisabledObjectsProperty = property.FindPropertyRelative("tempDisabledObjects");
            if (!reorderableListForPropertyPath.ContainsKey(tempDisabledObjectsProperty.propertyPath))
            {
                reorderableListForPropertyPath[tempDisabledObjectsProperty.propertyPath] = new ReorderableList(property.serializedObject, tempDisabledObjectsProperty, true, true, true, true);
                reorderableListForPropertyPath[tempDisabledObjectsProperty.propertyPath].AddHeader("Temporarily Disabled Objects");
                reorderableListForPropertyPath[tempDisabledObjectsProperty.propertyPath].AddStandardElementCallback();
            }
            reorderableListForPropertyPath[tempDisabledObjectsProperty.propertyPath].DoList(tempDisabledObjectsRect);
            height += reorderableListForPropertyPath[tempDisabledObjectsProperty.propertyPath].GetHeight();

            Rect transformationsRect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
            SerializedProperty transformationsProperty = property.FindPropertyRelative("transformations");
            if (!reorderableListForPropertyPath.ContainsKey(transformationsProperty.propertyPath))
                reorderableListForPropertyPath[transformationsProperty.propertyPath] = new TextureTransformationReorderableList(property.serializedObject, transformationsProperty, "Transformations", true, true, true, true);
            reorderableListForPropertyPath[transformationsProperty.propertyPath].DoList(transformationsRect);
            height += reorderableListForPropertyPath[transformationsProperty.propertyPath].GetHeight();

            EditorGUI.EndProperty();
        }
    }
}