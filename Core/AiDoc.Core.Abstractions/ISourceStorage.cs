using AiDoc.Core.Models;

namespace AiDoc.Core.Abstractions;

public interface ISourceStorage
{
    public Task<SourceFile[]> GetStructureAsync();
    public Task<ModifiedSourceFile[]> GetDiffStructureAsync(string? commitSha);
    public Task<string> GetFileAsync(string path);
    public Task<string> GetFileDiffAsync(string path, string commitSha);
}