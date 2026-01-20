using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public static class MonoBehaviourExtended
    {
        public static void FlexibleDestroy(Object obj)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                MonoBehaviour.Destroy(obj);
            else
                MonoBehaviour.DestroyImmediate(obj);
#else
        MonoBehaviour.Destroy(obj);
#endif
        }
    }
}