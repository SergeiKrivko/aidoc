using AiDoc.Core.Abstractions;

namespace AiDoc.Application;

public class GenerationService : IGenerationService
{
    public GenerationService(IAiClient aiClient)
    {
    }

    public async Task GenerateAsync(ISourceStorage sourceStorage, IDocumentationStorage documentationStorage)
    {
        throw new NotImplementedException();
    }
}