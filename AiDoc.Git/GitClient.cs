using AiDoc.Core.Models;
using LibGit2Sharp;

namespace AiDoc.Git;

public class GitClient
{
    public static IEnumerable<ModifiedSourceFile> GetStructureDiff(string repoPath, string commitSha)
    {
        using (var repo = new Repository(repoPath))
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
        using (var repo = new Repository(repoPath))
        {
            var commit = repo.Lookup<Commit>(commitSha);
            if (commit == null)
                throw new Exception("Commit not found");

            // Получаем разницу между текущим состоянием и указанным коммитом
            var diff = repo.Diff.Compare<Patch>(commit.Tree, repo.Head.Tip.Tree, [filePath]);
            return diff.Content;
        }
    }
}