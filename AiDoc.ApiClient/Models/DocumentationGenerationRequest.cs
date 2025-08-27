using System.IO;

namespace AiDoc.ApiClient.Models;

public class DocumentationGenerationRequest
{
    /// <summary>
    /// Название приложения
    /// </summary>
    public required string ApplicationName { get; set; }
    
    /// <summary>
    /// URL GitHub репозитория
    /// </summary>
    public string? GithubRepoUrl { get; set; }
    
    /// <summary>
    /// Список измененных исходных файлов
    /// </summary>
    public required string[] ChangedSources { get; set; }
    
    /// <summary>
    /// Список измененных файлов документации
    /// </summary>
    public required string[] ChangedDocs { get; set; }
    
    /// <summary>
    /// Архив исходных файлов
    /// </summary>
    public required Stream SourcesArchive { get; set; }
    
    /// <summary>
    /// Архив файлов документации (опционально)
    /// </summary>
    public Stream? DocsArchive { get; set; }
    
    /// <summary>
    /// Преобразует в DocCreateRequest для API
    /// </summary>
    public DocCreateRequest ToApiRequest()
    {
        return new DocCreateRequest
        {
            Info = new DocInfo
            {
                ApplicationInfo = new AppInfo
                {
                    Name = ApplicationName,
                    GithubRepo = GithubRepoUrl ?? "https://github.com/SergeiKrivko/aidoc"
                },
                ChangedSources = ChangedSources,
                ChangedDocs = ChangedDocs
            },
            Sources = SourcesArchive,
            Docs = DocsArchive
        };
    }
}
