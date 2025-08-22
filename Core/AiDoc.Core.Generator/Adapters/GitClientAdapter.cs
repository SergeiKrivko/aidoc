using AiDoc.Core.Generator.Interfaces;
using AiDoc.Git;

namespace AiDoc.Core.Generator.Adapters;

/// <summary>
/// Адаптер для GitClient
/// </summary>
public class GitClientAdapter : IGitClient
{
    public IEnumerable<string> GetIgnoredFiles(string repoPath)
    {
        return GitClient.GetIgnoredFiles(repoPath);
    }

    public IEnumerable<string> GetAllFiles(string repoPath)
    {
        return GitClient.GetAllFiles(repoPath);
    }

    public Task<string> GetCurrentCommit(string repoPath)
    {
        return GitClient.GetCurrentCommit(repoPath);
    }
    
    public IEnumerable<string> GetChangedFiles(string repoPath, string commitSha)
    {
        var changes = GitClient.GetStructureDiff(repoPath, commitSha);
        return changes.Select(c => c.Path);
    }
    
    public IEnumerable<string> GetChangedFilesBetweenCommits(string repoPath, string fromCommitSha, string toCommitSha)
    {
        // Для получения изменений между двумя коммитами используем разность
        // изменений от fromCommitSha до HEAD и от toCommitSha до HEAD
        var changesFrom = GitClient.GetStructureDiff(repoPath, fromCommitSha);
        var changesTo = GitClient.GetStructureDiff(repoPath, toCommitSha);
        
        // Объединяем изменения и убираем дубликаты
        var allChanges = changesFrom.Concat(changesTo)
            .Select(c => c.Path)
            .Distinct()
            .ToList();
            
        return allChanges;
    }
    
    public string? GetLastTag(string repoPath)
    {
        return GitClient.GetLastTag(repoPath);
    }
    
    public string? GetCommitByTag(string repoPath, string tagName)
    {
        return GitClient.GetCommitByTag(repoPath, tagName);
    }
}
