using System.Text.Json;
using AiDoc.Application.Shemas;
using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;

namespace AiDoc.Cli;

public class LocalDocumentationStorage(string documentationPath) : IDocumentationStorage
{
    private const string DirectoryConfigFileName = "_category_.json";
    private string MetadataPath => Path.Join(documentationPath, ".aidoc.json");
    private readonly string[] _ignoreFiles = ["index.md", ".aidoc.json", "uml.md", "uml.png"];

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
                .Where(f => !_ignoreFiles.Contains(Path.GetFileName(f)))
                .Select(f => new DocumentationFile
                {
                    Path = Path.GetRelativePath(documentationPath, f),
                    Position = 0,
                }).ToArray(),
            Directories = Directory.EnumerateDirectories(documentationPath, "*", SearchOption.AllDirectories)
                .Select(GetDirectory).ToArray()
        });
    }

    private static DocumentationDirectory GetDirectory(string path)
    {
        var conf = JsonSerializer.Deserialize<DirectoryConfiguration>(
            File.ReadAllText(Path.Join(path, DirectoryConfigFileName)));
        return new DocumentationDirectory
        {
            Path = path,
            Position = conf?.Position ?? 0,
            Label = conf?.Label ?? "",
            Description = path
        };
    }

    public Task<string> GetFileAsync(string path)
    {
        return File.ReadAllTextAsync(Path.Join(documentationPath, path));
    }

    public Task PutFileAsync(DocumentationFile file)
    {
        return File.WriteAllTextAsync(Path.Join(documentationPath, file.Path), file.Content);
    }

    public async Task PutDirectoryAsync(DocumentationDirectory directory)
    {
        Directory.CreateDirectory(Path.Join(documentationPath, directory.Path));
        await File.WriteAllTextAsync(Path.Join(documentationPath, directory.Path, DirectoryConfigFileName),
            JsonSerializer.Serialize(new DirectoryConfiguration
            {
                Label = directory.Label,
                Position = 0,
            }));
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