using UnityEngine;

/// <summary>
/// Интерфейс для объектов с логированием.
/// Реализуй LogCategory и используй this.LogInfo(), this.LogWarning(), this.LogError()
/// </summary>
public interface ILoggable
{
    string LogCategory { get; }
}

/// <summary>
/// Extension methods для логирования.
/// </summary>
public static class LogExtensions
{
    public static void LogInfo(this ILoggable loggable, string message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"[{loggable.LogCategory}] {message}");
#endif
    }
    
    public static void LogInfo(this ILoggable loggable, string message, Object context)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log($"[{loggable.LogCategory}] {message}", context);
#endif
    }
    
    public static void LogWarning(this ILoggable loggable, string message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning($"[{loggable.LogCategory}] {message}");
#endif
    }
    
    public static void LogWarning(this ILoggable loggable, string message, Object context)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarning($"[{loggable.LogCategory}] {message}", context);
#endif
    }
    
    public static void LogError(this ILoggable loggable, string message)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogError($"[{loggable.LogCategory}] {message}");
#endif
    }
    
    public static void LogError(this ILoggable loggable, string message, Object context)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogError($"[{loggable.LogCategory}] {message}", context);
#endif
    }
}
