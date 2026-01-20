using System.Collections;
using UnityEditor;

namespace BasePuzzle.PuzzlePackages.Core
{
    public static class EditorCoroutine
    {
        public static IEnumerator StartCoroutine(IEnumerator routine, System.Action onComplete = null)
        {
            void Execution()
            {
                if (routine.MoveNext()) return;
                EditorApplication.update -= Execution;
                onComplete?.Invoke();
            }

            EditorApplication.update += Execution;
            return routine;
        }
    }
}