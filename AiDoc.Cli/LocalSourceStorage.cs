using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;
using AiDoc.Git;

namespace AiDoc.Cli;

public class LocalSourceStorage(string sourcePath) : ISourceStorage
{

    public Task<SourceFile[]> GetStructureAsync()
    {
        return Task.FromResult(GetStructure(sourcePath).ToArray());
    }

    private static IEnumerable<SourceFile> GetStructure(string path)
    {
        foreach (var file in Directory.EnumerateFiles(path))
        {
            yield return new SourceFile { Path = path };
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