using UnityEngine;
using UnityEditor;

namespace TRS.CaptureTool.Extras
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(TimeActivateScript))]
    public class TimeActivateScriptEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }
    }
}