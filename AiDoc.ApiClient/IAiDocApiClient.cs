using AiDoc.ApiClient.Models;

namespace AiDoc.ApiClient;

/// <summary>
/// Интерфейс для работы с API генерации документации
/// </summary>
public interface IAiDocApiClient
{
    /// <summary>
    /// Начинает генерацию документации
    /// </summary>
    /// <param name="request">Запрос на генерацию документации</param>
    /// <returns>ID задачи генерации</returns>
    Task<string> StartDocumentationGenerationAsync(DocumentationGenerationRequest request);
    
    /// <summary>
    /// Получает статус генерации документации
    /// </summary>
    /// <param name="taskId">ID задачи генерации</param>
    /// <returns>Результат генерации</returns>
    Task<DocumentationGenerationResult> GetDocumentationStatusAsync(string taskId);
    
    /// <summary>
    /// Ожидает завершения генерации документации с поллингом
    /// </summary>
    /// <param name="taskId">ID задачи генерации</param>
    /// <param name="pollingIntervalMs">Интервал поллинга в миллисекундах (по умолчанию 5000)</param>
    /// <param name="timeoutMs">Таймаут ожидания в миллисекундах (по умолчанию 300000 = 5 минут)</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат генерации</returns>
    Task<DocumentationGenerationResult> WaitForDocumentationCompletionAsync(
        string taskId, 
        int pollingIntervalMs = 5000, 
        int timeoutMs = 300000, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Скачивает результат генерации документации
    /// </summary>
    /// <param name="resultDocsUrl">URL для скачивания результата</param>
    /// <returns>Поток с содержимым архива</returns>
    Task<Stream> DownloadDocumentationResultAsync(string resultDocsUrl);
}


