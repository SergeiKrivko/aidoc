using AiDoc.Core.Generator.Interfaces;

namespace AiDoc.Core.Generator.Services;

/// <summary>
/// Простая реализация логгера для консоли
/// </summary>
public class ConsoleLogger : ILogger
{
    public void LogInfo(string message)
    {
        Console.WriteLine(message);
    }

    public void LogWarning(string message)
    {
        Console.WriteLine($"Предупреждение: {message}");
    }

    public void LogError(string message)
    {
        Console.Error.WriteLine($"Ошибка: {message}");
    }
}
