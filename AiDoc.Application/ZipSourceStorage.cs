using System.IO.Compression;
using System.Text;
using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;

namespace AiDoc.Application;

public class ZipSourceStorage(ZipArchive zip, ModifiedSourceFile[] diff) : ISourceStorage
{
    public record UpdatedDocumentationFile(string Path, string NewContent);

    public Task<SourceFile[]> GetStructureAsync()
    {
        return Task.FromResult(zip.Entries.Select(e => new SourceFile
        {
            Path = e.FullName
        }).ToArray());
    }

    public Task<ModifiedSourceFile[]> GetDiffStructureAsync(string? commitSha)
    {
        return Task.FromResult(diff);
    }

    public async Task<string> GetFileAsync(string path)
    {
        await using var stream = zip.Entries.First(e => e.FullName == path).Open();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    public Task<string> GetFileDiffAsync(string path, string commitSha)
    {
        return Task.FromResult(diff.First(e => e.Path == path).Content);
    }
}