// https://answers.unity.com/questions/622154/is-there-a-callback-function-or-event-for-a-resolu.html

using System.Collections;
using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public static class MonoBehaviourExtensions
    {
        // Performs an Action after a YieldInstruction. 
        public static void StartCoroutineAfterYield<T>(this MonoBehaviour monoBehaviour, System.Action action)
            where T : YieldInstruction, new()
        {
            monoBehaviour.StartCoroutine(CoroutineAfterYield<T>(action));
        }

        static IEnumerator CoroutineAfterYield<T>(System.Action action) where T : YieldInstruction, new()
        {
            yield return new T();
            action();
        }
    }
}