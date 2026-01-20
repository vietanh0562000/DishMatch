using UnityEditor;
using UnityEditor.UI;

namespace BasePuzzle.PuzzlePackages.Core
{
    [CustomEditor(typeof(ScaleOnPressButton), true)] 
    [CanEditMultipleObjects]
    public class ScaleOnPressButtonEditor : ButtonEditor
    {
        SerializedProperty _downScaleProp;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _downScaleProp = serializedObject.FindProperty("_scaleFactor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_downScaleProp);
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}