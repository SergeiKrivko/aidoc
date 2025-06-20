﻿using AiDoc.Core.Models;

namespace AiDoc.Core.Abstractions;

public interface IDocumentationStorage
{
    public Task<string?> GetLatestCommitHashAsync();
    public Task SetLatestCommitHashAsync(string commitHash);

    public Task<DocumentationStructure> GetStructureAsync();
    public Task<string> GetFileAsync(string path);
    public Task PutFileAsync(DocumentationFile file);
    public Task PutFileAsync(string path, Stream content);
    public Task PutDirectoryAsync(DocumentationDirectory directory);
    public Task DeleteNodeAsync(string path);
}