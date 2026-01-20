using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Core
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance => _instance;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = GetComponent<T>();
                DontDestroyOnLoad(gameObject);
                return;
            }

            DestroyImmediate(gameObject);
        }
    }
}