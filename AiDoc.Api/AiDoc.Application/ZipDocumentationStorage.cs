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
    private HashSet<string> _updatedFiles = [];
    private HashSet<string> _deletedFiles = [];
    
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

    public Task PutFileAsync(DocumentationFile file, string content)
    {
        _updatedFiles.Add(file.Path);
        var existingFile = files.SingleOrDefault(f => f.Path == file.Path);
        if (existingFile == null)
        {
            files.Add(file);
        }
        else
        {
            existingFile.Content = content;
        }
        return Task.CompletedTask;
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