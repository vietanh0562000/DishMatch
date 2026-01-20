using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoUtils 
{
    /// <summary>
    /// Grabs a component on the object, or adds it to the object if none were found
    /// </summary>
    /// <param name="this"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetComponentOrAdd<T>(this GameObject @this) where T : Component
    {
        var t = @this.TryGetComponent<T>(out var component);
        if (!t)
        {
            component = @this.AddComponent<T>();
        }
        return component;
    }


    /// <summary>
    /// Dừng chạy khi gameObject bị disable, MonoBehaviour bị disable không ảnh hưởng
    /// </summary>
    public static Coroutine Delay<T>(this T t, float timeSec, Action action, bool ignoreTimeScale = false) where T : MonoBehaviour
    {
        if (action == null || !t.gameObject.activeInHierarchy) return null;
        if (timeSec < 0)
        {
            return null;//Không làm gì cả khi thời gian < 0
        }
        else if (timeSec == 0)
        {
            action();//Thực hiện ngay lập tức nếu thời gian bằng 0
            return null;
        }
        else
        {
            return t.StartCoroutine(DelayCoroutine(timeSec, action, ignoreTimeScale));
        }
    }

    private static IEnumerator DelayCoroutine(float timeSec, Action action, bool ignoreTimeScale = false)
    {
        if (ignoreTimeScale)
        {
            yield return new WaitForSecondsRealtime(timeSec);
        }
        else
        {
            yield return new WaitForSeconds(timeSec);
        }
        action();
    }
}
