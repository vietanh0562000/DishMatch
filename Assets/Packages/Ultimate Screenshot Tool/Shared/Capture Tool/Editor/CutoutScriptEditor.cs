using UnityEngine;
using UnityEditor;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(CutoutScript))]
    public class CutoutScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            ((CutoutScript)target).preview = EditorGUILayout.Toggle("Preview", ((CutoutScript)target).preview);
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(target);

            if(((CutoutScript)target).preview)
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("clickToSelectPivot"));

            EditorGUILayout.HelpBox("Adjust the rect transform to define the cutout.\n\nTip: Use a relative size rect transform by setting the min and max anchors to separate values to keep a constant cutout regardless of recording size. Example: anchorMin.x = 0.1, anchorMin.y = 0.1, anchorMax.x = 0.9, anchorMax.y = 0.9. That would cut off 10% from each screen edge.", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}