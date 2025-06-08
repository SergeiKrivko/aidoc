using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;
using AiDoc.Git;

namespace AiDoc.Cli;

public class LocalSourceStorage(string sourcePath) : ISourceStorage
{
    private HashSet<string>? _ignoredFiles;

    public Task<SourceFile[]> GetStructureAsync()
    {
        return Task.FromResult(GetStructure(sourcePath).ToArray());
    }

    private IEnumerable<SourceFile> GetStructure(string path)
    {
        _ignoredFiles ??= GitClient.GetIgnoredFiles(sourcePath)
            .Select(Path.GetFullPath)
            .ToHashSet();
        foreach (var file in Directory.EnumerateFiles(path)
                     .Where(p => !_ignoredFiles.Contains(p)))
        {
            yield return new SourceFile { Path = file };
        }

        foreach (var directory in Directory.EnumerateDirectories(path))
        {
            foreach (var file in GetStructure(directory))
            {
                yield return file;
            }
        }
    }

    public async Task<ModifiedSourceFile[]> GetDiffStructureAsync(string? commitSha)
    {
        return await Task.Run(() => GitClient.GetStructureDiff(sourcePath, commitSha).ToArray());
    }

    public Task<string> GetFileAsync(string path)
    {
        return File.ReadAllTextAsync(path);
    }

    public async Task<string> GetFileDiffAsync(string path, string commitSha)
    {
        return await Task.Run(() => GitClient.GetFileDiff(sourcePath, path, commitSha));
    }

    public Task DeleteFileAsync(string path)
    {
        File.Delete(path);
        return Task.CompletedTask;
    }

    public Task PutFileAsync(string path, string content)
    {
        return File.WriteAllTextAsync(path, content);
    }
}