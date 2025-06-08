using System.Text.Json;
using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;

namespace AiDoc.Cli;

public class LocalDocumentationStorage(string documentationPath) : IDocumentationStorage
{
    private string MetadataPath => Path.Join(documentationPath, ".aidoc.json");

    private async Task<DocumentationMetadata?> ReadMetadataAsync()
    {
        try
        {
            await using var fileStream = File.OpenRead(MetadataPath);
            return await JsonSerializer.DeserializeAsync<DocumentationMetadata>(fileStream);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
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
        return Task.FromResult(new DocumentationStructure
        {
            Files = Directory.EnumerateFiles(documentationPath, "*", SearchOption.AllDirectories)
                .Select(f => new DocumentationFile
                {
                    Content = File.ReadAllText(f),
                    Path = f,
                    Position = 0,
                }).ToArray(),
            Directories = Directory.EnumerateDirectories(documentationPath, "*", SearchOption.AllDirectories)
                .Select(GetDirectory).ToArray()
        });
    }

    private static DocumentationDirectory GetDirectory(string path)
    {
        return new DocumentationDirectory
        {
            Path = path,
            Position = 0,
            Label = path,
            Description = path
        };
    }

    public Task<string> GetFileAsync(string path)
    {
        return File.ReadAllTextAsync(path);
    }

    public Task PutFileAsync(DocumentationFile file)
    {
        return File.WriteAllTextAsync(file.Path, file.Content);
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