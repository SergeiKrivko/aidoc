using System.IO.Compression;
using System.Text;
using System.Text.Json;
using AiDoc.Application.Shemas;
using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;

namespace AiDoc.Application;

public class ZipDocumentationStorage(List<DocumentationDirectory> directories, List<DocumentationFile> files)
    : IDocumentationStorage
{
    private readonly HashSet<string> _updatedFiles = [];
    private readonly HashSet<string> _updatedDirectories = [];
    private readonly HashSet<string> _deletedNodes = [];

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
                zip.Entries.First(e => e.FullName == Path.Join(directory, DirectoryConfigFileName))));
        }

        var files = new List<DocumentationFile>();

        foreach (var entry in zip.Entries
                     .Where(f => Path.GetFileName(f.FullName) != DirectoryConfigFileName))
        {
            using var memoryStream = new MemoryStream();
            await using var entryStream = entry.Open();
            await entryStream.CopyToAsync(memoryStream);
            var content = Encoding.UTF8.GetString(memoryStream.ToArray());
            files.Add(new DocumentationFile
            {
                Content = content,
                Path = entry.FullName,
                Position = 0,
            });
        }

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
        return Task.FromResult(new DocumentationStructure
        {
            Files = files.ToArray(),
            Directories = directories.ToArray(),
        });
    }

    public Task<string> GetFileAsync(string path)
    {
        return Task.FromResult(files.Single(e => e.Path == path).Content ?? "");
    }

    public Task PutFileAsync(DocumentationFile file)
    {
        _updatedFiles.Add(file.Path);
        var existingFile = files.SingleOrDefault(f => f.Path == file.Path);
        if (existingFile == null)
        {
            files.Add(file);
        }
        else
        {
            existingFile.Content = file.Content;
            existingFile.Position = file.Position;
        }

        return Task.CompletedTask;
    }

    public Task PutDirectoryAsync(DocumentationDirectory directory)
    {
        _updatedDirectories.Add(directory.Path);
        var existingDirectory = directories.SingleOrDefault(f => f.Path == directory.Path);
        if (existingDirectory == null)
        {
            directories.Add(directory);
        }
        else
        {
            existingDirectory.Label = directory.Label;
            existingDirectory.Description = directory.Description;
            existingDirectory.Position = directory.Position;
        }

        return Task.CompletedTask;
    }

    public Task DeleteNodeAsync(string path)
    {
        var existingFile = files.SingleOrDefault(f => f.Path == path);
        if (existingFile != null)
        {
            files.Remove(existingFile);
            _deletedNodes.Add(existingFile.Path);
        }

        var existingDirectory = directories.SingleOrDefault(f => f.Path == path);
        if (existingDirectory != null)
        {
            directories.Remove(existingDirectory);
            _deletedNodes.Add(Path.Join(existingDirectory.Path, DirectoryConfigFileName));
            foreach (var file in files.Where(f => f.Path.StartsWith(path)))
            {
                _updatedFiles.Add(file.Path);
            }
        }

        return Task.CompletedTask;
    }

    public GenerationTaskResult GetResult()
    {
        var resultFiles = new List<GenerationTaskResult.ResultFile>();
        foreach (var updatedDirectory in _updatedDirectories)
        {
            var directory = directories.Single(d => d.Path == updatedDirectory);
            resultFiles.Add(new GenerationTaskResult.ResultFile()
            {
                Path = Path.Join(directory.Path, DirectoryConfigFileName),
                Content = JsonSerializer.Serialize(new DirectoryConfiguration
                {
                    Label = directory.Label,
                    Position = directory.Position,
                })
            });
        }

        foreach (var updatedFile in _updatedFiles)
        {
            var file = files.Single(d => d.Path == updatedFile);
            resultFiles.Add(new GenerationTaskResult.ResultFile
            {
                Path = file.Path,
                Content = file.Content ?? "",
            });
        }

        return new GenerationTaskResult
        {
            UpdatedFiles = resultFiles.ToArray(),
            DeletedFiles = _deletedNodes.ToArray(),
        };
    }
}