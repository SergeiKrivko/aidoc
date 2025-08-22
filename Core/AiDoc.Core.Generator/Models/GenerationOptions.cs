namespace AiDoc.Core.Generator.Models;

/// <summary>
/// Настройки для генерации документации
/// </summary>
public class GenerationOptions
{
    /// <summary>
    /// Путь к исходному коду проекта
    /// </summary>
    public required string SourcePath { get; set; }
    
    /// <summary>
    /// Путь к документации (по умолчанию проект/docs)
    /// </summary>
    public string? DocumentationPath { get; set; }
    
    /// <summary>
    /// Имя проекта (по умолчанию имя папки)
    /// </summary>
    public string? ProjectName { get; set; }
    
    /// <summary>
    /// URL API для генерации
    /// </summary>
    public string? ApiUrl { get; set; }
    
    /// <summary>
    /// GitHub репозиторий (опционально)
    /// </summary>
    public string? GithubRepoUrl { get; set; }
    
    /// <summary>
    /// Базовый коммит для сравнения изменений (SHA, тег или ветка)
    /// </summary>
    public string? BaseCommitSha { get; set; }
    
    /// <summary>
    /// Получить путь к документации по умолчанию
    /// </summary>
    public string GetDocumentationPath() => 
        DocumentationPath ?? Path.Join(SourcePath, "docs");
    
    /// <summary>
    /// Получить имя проекта по умолчанию
    /// </summary>
    public string GetProjectName() => 
        ProjectName ?? Path.GetFileName(SourcePath);
}
