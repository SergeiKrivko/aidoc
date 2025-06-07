using AiDoc.Core.Models;

namespace AiDoc.Core.Abstractions;

public interface ISourceStorage
{
    public Task<SourceNode> GetStructureAsync();
    public Task<SourceNode> GetDiffStructureAsync();
    public Task<string> GetFileAsync(string path);
    public Task<string> GetFileDiffAsync(string path);
    public Task DeleteFileAsync(string path);
    public Task PutFileAsync(string path, string content);
}