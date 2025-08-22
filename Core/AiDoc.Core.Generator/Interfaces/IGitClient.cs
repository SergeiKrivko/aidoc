namespace AiDoc.Core.Generator.Interfaces;

/// <summary>
/// Интерфейс для работы с Git
/// </summary>
public interface IGitClient
{
    /// <summary>
    /// Получает список игнорируемых файлов
    /// </summary>
    IEnumerable<string> GetIgnoredFiles(string repoPath);
    
    /// <summary>
    /// Получает все файлы в репозитории с учетом Git-игнора
    /// </summary>
    IEnumerable<string> GetAllFiles(string repoPath);
    
    /// <summary>
    /// Получает текущий коммит
    /// </summary>
    Task<string> GetCurrentCommit(string repoPath);
    
    /// <summary>
    /// Получает изменения между указанным коммитом и текущим состоянием
    /// </summary>
    IEnumerable<string> GetChangedFiles(string repoPath, string commitSha);
    
    /// <summary>
    /// Получает изменения между двумя коммитами
    /// </summary>
    IEnumerable<string> GetChangedFilesBetweenCommits(string repoPath, string fromCommitSha, string toCommitSha);
    
    /// <summary>
    /// Получает последний тег в репозитории
    /// </summary>
    string? GetLastTag(string repoPath);
    
    /// <summary>
    /// Получает коммит по тегу
    /// </summary>
    string? GetCommitByTag(string repoPath, string tagName);
}
