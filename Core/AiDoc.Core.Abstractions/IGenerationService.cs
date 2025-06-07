namespace AiDoc.Core.Abstractions;

public interface IGenerationService
{
    public Task GenerateAsync(ISourceStorage sourceStorage, IDocumentationStorage documentationStorage);
}