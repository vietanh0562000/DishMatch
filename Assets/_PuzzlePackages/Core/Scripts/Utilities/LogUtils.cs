using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class LogUtils
{
    [Conditional("ENABLE_LOG")]
    public static void LogError(object message)
    {
        Debug.LogError(message);
    }

    [Conditional("ENABLE_LOG")]
    public static void LogError(object message, Object context)
    {
        Debug.LogError(message, context);
    }

    [Conditional("ENABLE_LOG")]
    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }

    [Conditional("ENABLE_LOG")]
    public static void LogWarning(object message, Object context)
    {
        Debug.LogWarning(message, context);
    }

    [Conditional("ENABLE_LOG")]
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    [Conditional("ENABLE_LOG")]
    public static void Log(object message, Object context)
    {
        Debug.Log(message);
    }

}
