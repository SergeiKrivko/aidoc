namespace AiDoc.Core.ApiClient.Models;

public class DocumentationGenerationResult
{
    /// <summary>
    /// ID задачи генерации
    /// </summary>
    public required string TaskId { get; set; }
    
    /// <summary>
    /// Статус генерации
    /// </summary>
    public required string Status { get; set; }
    
    /// <summary>
    /// Описание ошибки (если есть)
    /// </summary>
    public string? ErrorDescription { get; set; }
    
    /// <summary>
    /// URL для скачивания исходных файлов
    /// </summary>
    public string? OriginalSourcesUrl { get; set; }
    
    /// <summary>
    /// URL для скачивания исходной документации
    /// </summary>
    public string? OriginalDocsUrl { get; set; }
    
    /// <summary>
    /// URL для скачивания результата документации
    /// </summary>
    public string? ResultDocsUrl { get; set; }
    
    /// <summary>
    /// Проверяет, завершена ли генерация
    /// </summary>
    public bool IsCompleted => Status == DocCreationStatus.Done || Status == DocCreationStatus.Failed;
    
    /// <summary>
    /// Проверяет, успешно ли завершена генерация
    /// </summary>
    public bool IsSuccess => Status == DocCreationStatus.Done;
    
    /// <summary>
    /// Проверяет, произошла ли ошибка
    /// </summary>
    public bool IsFailed => Status == DocCreationStatus.Failed;
    
    /// <summary>
    /// Проверяет, выполняется ли генерация
    /// </summary>
    public bool IsInProgress => Status == DocCreationStatus.Progress;
}
