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

            foreach (var entry in Directory.GetFiles(repo.Info.Path, "*.*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(repoPath, entry);
                if (repo.Ignore.IsPathIgnored(relativePath) || relativePath.StartsWith($".git{Path.DirectorySeparatorChar}") ||
                    relativePath.Contains($"{Path.DirectorySeparatorChar}.git{Path.DirectorySeparatorChar}"))
                {
                    ignoredFiles.Add(entry);
                }
            }

            return ignoredFiles;
        }
    }

    private static Repository? FindRepo(string repoPath)
    {
        var foundRepoPath = Repository.Discover(repoPath);

        return foundRepoPath is null ? null : new Repository(foundRepoPath);
    }
}