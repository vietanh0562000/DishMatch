using System.Collections;
using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public class CoroutineBehaviour : MonoBehaviour
    {
        static MonoBehaviour Instance;

        public static void StaticStartCoroutine(IEnumerator iEnumerator)
        {
            InitializeInstance();
            Instance.StartCoroutine(iEnumerator);
        }

        public static void StaticStartCoroutineAfterYield<T>(System.Action action) where T : YieldInstruction, new()
        {
            InitializeInstance();
            Instance.StartCoroutineAfterYield<T>(action);
        }

        // Inspired by: https://www.feelouttheform.net/
        public static void StaticWaitForCoroutine(IEnumerator func)
        {
            while (func.MoveNext())
            {
                if (func.Current != null)
                {
                    IEnumerator current;
                    try
                    {
                        current = (IEnumerator)func.Current;
                    }
                    catch (System.InvalidCastException)
                    {
                        if (func.Current.GetType() == typeof(WaitForSeconds))
                            Debug.LogWarning("Skipped call to WaitForSeconds. Use WaitForSecondsRealtime instead.");
                        return;  // Skip WaitForSeconds, WaitForEndOfFrame and WaitForFixedUpdate
                    }
                    StaticWaitForCoroutine(current);
                }
            }
        }

        static void InitializeInstance()
        {
            if (Instance == null)
                Instance = new GameObject { hideFlags = HideFlags.HideAndDontSave }
                    .AddComponent<CoroutineBehaviour>();
        }
    }
}