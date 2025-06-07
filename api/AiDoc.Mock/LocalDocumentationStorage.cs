using System.Text.Json;
using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;

namespace AiDoc.Mock;

public class LocalDocumentationStorage : IDocumentationStorage
{
    private string DocumentationPath { get; } = Environment.GetEnvironmentVariable("TEST_DOCUMENTATION_PATH") ?? ".";
    private string MetadataPath => Path.Join(DocumentationPath, ".aidoc.json");

    private async Task<DocumentationMetadata?> ReadMetadataAsync()
    {
        await using var fileStream = File.OpenRead(MetadataPath);
        return await JsonSerializer.DeserializeAsync<DocumentationMetadata>(fileStream);
    }

    public async Task<string?> GetLatestCommitHashAsync()
    {
        var metadata = await ReadMetadataAsync();
        return metadata?.LatestCommitHash;
    }

    public async Task SetLatestCommitHashAsync(string commitHash)
    {
        var metadata = await ReadMetadataAsync() ?? new DocumentationMetadata();
        metadata.LatestCommitHash = commitHash;
        await using var fileStream = File.OpenWrite(MetadataPath);
        await JsonSerializer.SerializeAsync(fileStream, metadata);
    }

    public Task<DocumentationStructure> GetStructureAsync()
    {
        return Task.FromResult(GetStructure(DocumentationPath));
    }

    private DocumentationStructure GetStructure(string path)
    {
        return new DocumentationStructure()
        {
            Files = Directory.EnumerateFiles(path)
                .Where(p => p != MetadataPath)
                .Select(p => new DocumentationFile { Path = p, Position = 0 })
                .ToArray(),
            Directories = Directory.EnumerateDirectories(path)
                .Select(GetDirectory)
                .ToArray(),
        };
    }

    private DocumentationDirectory GetDirectory(string path)
    {
        return new DocumentationDirectory
        {
            Path = path,
            Position = 0, 
            Label = path,
            Description = path, 
            Children = GetStructure(path)
        };
    }

    public Task<string> GetFileAsync(string path)
    {
        return File.ReadAllTextAsync(path);
    }

    public Task PutFileAsync(DocumentationFile file, string content)
    {
        return File.WriteAllTextAsync(file.Path, content);
    }

    public Task PutDirectoryAsync(DocumentationDirectory directory)
    {
        return Task.CompletedTask;
    }

    public Task DeleteNodeAsync(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
        else
            Directory.Delete(path);
        return Task.CompletedTask;
    }
}