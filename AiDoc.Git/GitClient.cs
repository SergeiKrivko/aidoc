using AiDoc.Core.Models;
using LibGit2Sharp;

namespace AiDoc.Git;

public class GitClient
{
    public static IEnumerable<ModifiedSourceFile> GetDiff(string repoPath, string commitSha)
    {
        using (var repo = FindRepo(repoPath))
        {
            if (repo is null)
                throw new Exception("Repo not found");

            var commit = repo.Lookup<Commit>(commitSha);
            if (commit is null)
                throw new Exception("Commit not found");

            // Получаем разницу между текущим состоянием и указанным коммитом
            var diff = repo.Diff.Compare<Patch>(commit.Tree, repo.Head.Tip.Tree);

            foreach (var change in diff)
            {
                yield return new ModifiedSourceFile
                {
                    Path = change.Path,
                    ChangeType = change.Status.ToString(),
                    Content = diff.Content,
                };
            }
        }
    }

    public static IEnumerable<string> GetIgnoredFiles(string repoPath)
    {
        using (var repo = FindRepo(repoPath))
        {
            if (repo is null)
                throw new Exception("Repo not found");

            var ignoredFiles = new List<string>();

            foreach (var entry in Directory.GetFiles(repo.Info.WorkingDirectory, "*.*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(repoPath, entry);
                if (repo.Ignore.IsPathIgnored(relativePath.Replace('\\', '/')))
                {
                    ignoredFiles.Add(entry);
                }
            }

            return ignoredFiles;
        }
    }

    public static IEnumerable<string> GetAllFiles(string repoPath)
    {
        using (var repo = FindRepo(repoPath))
        {
            if (repo is null)
                throw new Exception("Repo not found");

            var allFiles = new List<string>();

            foreach (var entry in Directory.GetFiles(repo.Info.WorkingDirectory, "*.*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(repoPath, entry);
                if (!repo.Ignore.IsPathIgnored(relativePath.Replace('\\', '/')))
                {
                    allFiles.Add(relativePath);
                }
            }

            return allFiles;
        }
    }

    private static Repository? FindRepo(string repoPath)
    {
        var foundRepoPath = Repository.Discover(repoPath);

        return foundRepoPath is null ? null : new Repository(foundRepoPath);
    }

    public static IEnumerable<ModifiedSourceFile> GetStructureDiff(string repoPath, string commitSha)
    {
        using (var repo = FindRepo(repoPath))
        {
            var commit = repo.Lookup<Commit>(commitSha);
            if (commit == null)
                throw new Exception("Commit not found");

            // Получаем разницу между текущим состоянием и указанным коммитом
            var diff = repo.Diff.Compare<TreeChanges>(commit.Tree, repo.Head.Tip.Tree);

            foreach (var change in diff)
            {
                yield return new ModifiedSourceFile
                {
                    Path = change.Path,
                    ChangeType = change.Status.ToString()
                };
            }
        }
    }

    public static string GetFileDiff(string repoPath, string filePath, string commitSha)
    {
        using (var repo = FindRepo(repoPath))
        {
            if (repo is null)
                throw new Exception("Repo not found");
            var commit = repo.Lookup<Commit>(commitSha);
            if (commit == null)
                throw new Exception("Commit not found");

            // Получаем разницу между текущим состоянием и указанным коммитом
            var diff = repo.Diff.Compare<Patch>(commit.Tree, repo.Head.Tip.Tree, [filePath]);
            return diff.Content;
        }
    }

    public static Task<string> GetCurrentCommit(string repoPath)
    {
        using (var repo = FindRepo(repoPath))
        {
            if (repo is null)
                throw new Exception("Repo not found");
            var commit = repo.Head.Tip.Sha;
            if (commit == null)
                throw new Exception("Commit not found");
            
            return Task.FromResult(commit);
        }
    }
    
    /// <summary>
    /// Получает последний тег в репозитории
    /// </summary>
    public static string? GetLastTag(string repoPath)
    {
        using (var repo = FindRepo(repoPath))
        {
            if (repo is null)
                return null;
                
            var tags = repo.Tags.OrderByDescending(t => t.Target.Peel<Commit>().Committer.When).ToList();
            return tags.FirstOrDefault()?.FriendlyName;
        }
    }
    
    /// <summary>
    /// Получает коммит по тегу
    /// </summary>
    public static string? GetCommitByTag(string repoPath, string tagName)
    {
        using (var repo = FindRepo(repoPath))
        {
            if (repo is null)
                return null;
                
            var tag = repo.Tags[tagName];
            return tag?.Target.Sha;
        }
    }
}