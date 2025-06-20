namespace AiDoc.Core.Abstractions;

public interface IGenerationService
{
    public Task GenerateAsync(string projectName, ISourceStorage sourceStorage, IDocumentationStorage documentationStorage);
    public Task UpdateAsync(string projectName, ISourceStorage sourceStorage, IDocumentationStorage documentationStorage);
}