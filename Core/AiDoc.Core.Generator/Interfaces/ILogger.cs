namespace AiDoc.Core.Generator.Interfaces;

/// <summary>
/// Простой интерфейс для логирования
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Логирует информационное сообщение
    /// </summary>
    void LogInfo(string message);
    
    /// <summary>
    /// Логирует предупреждение
    /// </summary>
    void LogWarning(string message);
    
    /// <summary>
    /// Логирует ошибку
    /// </summary>
    void LogError(string message);
}
