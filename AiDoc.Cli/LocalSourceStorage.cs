using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;
using AiDoc.Git;

namespace AiDoc.Cli;

public class LocalSourceStorage(string sourcePath) : ISourceStorage
{
    private HashSet<string>? _ignoredFiles;

    public Task<SourceFile[]> GetStructureAsync()
    {
        _ignoredFiles ??= GitClient.GetIgnoredFiles(sourcePath)
            .Select(Path.GetFullPath)
            .ToHashSet();
        Console.WriteLine("IGNORED:");
        Console.WriteLine(string.Join('\n', _ignoredFiles.Where(p => p.Contains("/api/")).Order().Take(200)));
        Console.WriteLine("NOT IGNORED:");
        Console.WriteLine(string.Join('\n', Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
            .Where(p => !_ignoredFiles.Contains(p))
            .Order().Take(200)));
        var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
            .Where(p => !_ignoredFiles.Contains(p))
            .Select(p => new SourceFile
            {
                Path = Path.GetRelativePath(sourcePath, p),
            }).ToArray();
        return Task.FromResult(files);
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