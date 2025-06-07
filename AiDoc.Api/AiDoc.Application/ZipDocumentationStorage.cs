using System.IO.Compression;
using System.Text.Json;
using AiDoc.Application.Shemas;
using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;

namespace AiDoc.Application;

public class ZipDocumentationStorage(List<DocumentationDirectory> directories, List<DocumentationFile> files) : IDocumentationStorage
{
    private const string DirectoryConfigFileName = "_category_.json";

    public static async Task<ZipDocumentationStorage> LoadAsync(Stream stream)
    {
        var zip = new ZipArchive(stream, ZipArchiveMode.Read);
        var directories = zip.Entries
            .Select(e => Path.GetDirectoryName(e.FullName))
            .Where(e => e != null)
            .Distinct();
        var dirs = new List<DocumentationDirectory>();
        foreach (var directory in directories)
        {
            dirs.Add(await GetDirectoryAsync(directory!,
                zip.Entries.First(e => e.FullName == Path.Join(directory, "conf.json"))));
        }

        var files = zip.Entries
            .Where(f => Path.GetFileName(f.FullName) != DirectoryConfigFileName)
            .Select(f => new DocumentationFile
            {
                Path = f.FullName,
                Position = 0,
            });
        return new ZipDocumentationStorage(dirs, files.ToList());
    }

    private static async Task<DocumentationDirectory> GetDirectoryAsync(string path, ZipArchiveEntry entry)
    {
        await using var stream = entry.Open();
        var config = await JsonSerializer.DeserializeAsync<DirectoryConfiguration>(stream);
        return new DocumentationDirectory
        {
            Path = path,
            Label = config?.Label ?? Path.GetFileNameWithoutExtension(path),
            Position = config?.Position ?? 0,
            Description = "",
        };
    }

    public Task<string?> GetLatestCommitHashAsync()
    {
        return Task.FromResult<string?>(null);
    }

    public Task SetLatestCommitHashAsync(string commitHash)
    {
        return Task.CompletedTask;
    }

    public Task<DocumentationStructure> GetStructureAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetFileAsync(string path)
    {
        throw new NotImplementedException();
    }

    public async Task PutFileAsync(DocumentationFile file, string content)
    {
        throw new NotImplementedException();
    }

    public async Task PutDirectoryAsync(DocumentationDirectory directory)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteNodeAsync(string path)
    {
        throw new NotImplementedException();
    }
}