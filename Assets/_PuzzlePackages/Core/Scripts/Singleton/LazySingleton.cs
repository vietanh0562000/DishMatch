using UnityEngine;

public class LazySingleton<T> : MonoBehaviour where T : Component
{
    protected static T _instance;

    public static bool HaveInstance()
    {
        return _instance != null;
    }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }

    protected virtual void OnDestroy()
    {
        if (this == _instance)
        {
            _instance = null;
        }
    }
}
