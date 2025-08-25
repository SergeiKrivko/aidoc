namespace AiDoc.Generator;

/// <summary>
/// Настройки для генерации документации
/// </summary>
public class GenerationOptions
{
    /// <summary>
    /// Путь к исходному коду проекта
    /// </summary>
    public required string SourcesPath { get; set; }
    
    /// <summary>
    /// Путь к документации (по умолчанию проект/docs)
    /// </summary>
    public string? DocsPath { get; set; }
    
    /// <summary>
    /// Имя проекта (по умолчанию имя папки)
    /// </summary>
    public string? ProjectName { get; set; }
    
    /// <summary>
    /// URL API для генерации
    /// </summary>
    public string? ApiUrl { get; set; }
    
    /// <summary>
    /// GitHub репозиторий (будет указан в документации)
    /// </summary>
    public string? GithubRepoUrl { get; set; }
    
    /// <summary>
    /// Базовый коммит для сравнения изменений
    /// </summary>
    public string? BaseCommitSha { get; set; }
    
    /// <summary>
    /// Получить путь к исходному коду
    /// </summary>
    public string GetSourcesPath() => 
        Path.GetFullPath(SourcesPath);
    
    /// <summary>
    /// Получить путь к документации по умолчанию
    /// </summary>
    public string GetDocsPath() => 
        string.IsNullOrEmpty(DocsPath) ? Path.Join(GetSourcesPath(), "docs") : Path.GetFullPath(DocsPath);
    
    /// <summary>
    /// Получить имя проекта по умолчанию
    /// </summary>
    public string GetProjectName() => 
        ProjectName ?? Path.GetFileName(SourcesPath);
}
