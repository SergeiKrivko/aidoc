using System.IO.Compression;
using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;

namespace AiDoc.Application;

public class ZipDocumentationStorage(ZipArchive zip) : IDocumentationStorage
{
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