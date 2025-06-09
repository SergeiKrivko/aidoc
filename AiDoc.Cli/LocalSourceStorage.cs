using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;
using AiDoc.Git;

namespace AiDoc.Cli;

public class LocalSourceStorage(string sourcePath, string docPath) : ISourceStorage
{
    private HashSet<string>? _ignoredFiles;

    public Task<SourceFile[]> GetStructureAsync()
    {
        _ignoredFiles ??= GitClient.GetIgnoredFiles(sourcePath)
            .Select(Path.GetFullPath)
            .ToHashSet();
        var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
            .Where(p => !_ignoredFiles.Contains(p) && !p.StartsWith(docPath))
            .Select(p => new SourceFile
            {
                Path = Path.GetRelativePath(sourcePath, p),
            }).ToArray();
        return Task.FromResult(files);
    }

    public async Task<ModifiedSourceFile[]> GetDiffStructureAsync(string? commitSha)
    {
        if (commitSha == null)
            throw new Exception("Commit not found");
        return await Task.Run(() => GitClient.GetStructureDiff(sourcePath, commitSha).ToArray());
    }

    public Task<string> GetFileAsync(string path)
    {
        return File.ReadAllTextAsync(Path.Join(sourcePath, path));
    }

    public async Task<string> GetFileDiffAsync(string path, string commitSha)
    {
        return await Task.Run(() => GitClient.GetFileDiff(sourcePath, path, commitSha));
    }
}