using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool.Share
{
    public class CustomUrlParameterReorderableList : ReorderableList
    {
#pragma warning disable 0649
        struct CustomUrlParameterCreationParams
        {
            //public string displayName;
            public string name;
            public string value;
        }
#pragma warning restore 0649

        public CustomUrlParameterReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton, SerializedProperty allResolutionsProperty) : base(serializedObject, serializedProperty, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            List<string> headerTexts = new List<string>(new string[] { "Parameter Name", "Parameter Value" });
            List<string> properties = new List<string>(new string[] { "name", "value" });
            float[] widths = { 0.5f, 0.5f };

            this.AddHeader(headerTexts.ToArray(), widths, ReorderableListExtensions.SPACING_PERCENTAGE);
            this.AddStandardElementCallback(properties.ToArray(), widths);
        }

        void clickHandler(object creationParams)
        {
            CustomUrlParameterCreationParams data = (CustomUrlParameterCreationParams)creationParams;

            int newIndex = serializedProperty.arraySize;
            serializedProperty.arraySize++;

            var element = serializedProperty.GetArrayElementAtIndex(newIndex);

            element.FindPropertyRelative("name").stringValue = data.name;
            element.FindPropertyRelative("value").stringValue = data.value;

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}