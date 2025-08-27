using LibGit2Sharp;

namespace AiDoc.Generator;

public static class GitClient
{
    public static IEnumerable<string> GetFiles(string repoPath)
    {
        using var repo = FindRepo(repoPath);
        if (repo is null)
            throw new Exception("Repo not found");

        foreach (var entry in Directory.GetFiles(repo.Info.WorkingDirectory, "*.*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(repoPath, entry);
            if (!repo.Ignore.IsPathIgnored(relativePath.Replace('\\', '/')))
            {
                yield return Path.GetFullPath(entry);
            }
        }
    }

    public static IEnumerable<string> GetChangedFiles(string repoPath, string commitSha)
    {
        using var repo = FindRepo(repoPath);
        if (repo is null)
            throw new Exception("Repo not found");

        var commit = repo.Lookup<Commit>(commitSha);
        if (commit == null)
            throw new Exception("Commit not found");

        // Получаем разницу между текущим состоянием и указанным коммитом
        var diff = repo.Diff.Compare<TreeChanges>(commit.Tree, repo.Head.Tip.Tree);

        foreach (var change in diff)
        {
            yield return change.Path;
        }
    }

    private static Repository? FindRepo(string repoPath)
    {
        var foundRepoPath = Repository.Discover(repoPath);

        return foundRepoPath is null ? null : new Repository(foundRepoPath);
    }
}