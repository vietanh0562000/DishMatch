using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NMSingleton<T> where T : class, new()
{
    private static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }

    protected NMSingleton()
    {
        Init();
    }


    protected abstract void Init();
    
}
