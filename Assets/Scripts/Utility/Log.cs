using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
/// Вызовы автоматически удаляются из билда (Conditional атрибут).
/// </summary>
public static class LogExtensions
{
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogInfo(this ILoggable loggable, string message)
    {
        Debug.Log(Format(loggable.LogCategory, message));
    }
    
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogInfo(this ILoggable loggable, string message, Object context)
    {
        Debug.Log(Format(loggable.LogCategory, message), context);
    }
    
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(this ILoggable loggable, string message)
    {
        Debug.LogWarning(Format(loggable.LogCategory, message));
    }
    
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogWarning(this ILoggable loggable, string message, Object context)
    {
        Debug.LogWarning(Format(loggable.LogCategory, message), context);
    }
    
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogError(this ILoggable loggable, string message)
    {
        Debug.LogError(Format(loggable.LogCategory, message));
    }
    
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void LogError(this ILoggable loggable, string message, Object context)
    {
        Debug.LogError(Format(loggable.LogCategory, message), context);
    }
    
    private static string Format(string category, string message) => $"[{category}] {message}";
}
