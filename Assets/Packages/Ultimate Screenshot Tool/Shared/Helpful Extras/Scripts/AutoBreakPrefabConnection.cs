// Inspired by:
// https://answers.unity.com/questions/323866/how-to-programmatically-upon-adding-to-hierachy.html

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TRS.CaptureTool.Extras
{
    [ExecuteInEditMode]
    public class AutoBreakPrefabConnection : MonoBehaviour
    {
        void Start()
        {
#if UNITY_EDITOR
            try
            {
                PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            } catch
            {
                MonoBehaviourExtended.FlexibleDestroy(this); // Remove this script
            }
#endif
            MonoBehaviourExtended.FlexibleDestroy(this); // Remove this script
        }
    }
}