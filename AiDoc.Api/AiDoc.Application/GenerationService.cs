using AiDoc.Application.Shemas;
using AiDoc.Core.Abstractions;

namespace AiDoc.Application;

public class GenerationService(IAiClient aiClient) : IGenerationService
{
    private void Init(ISourceStorage sourceStorage, IDocumentationStorage documentationStorage)
    {
        aiClient.AddFunction<string, string?>("get_file",
            async path => path == null ? null : await sourceStorage.GetFileAsync(path));
    }

    public async Task GenerateAsync(ISourceStorage sourceStorage, IDocumentationStorage documentationStorage)
    {
        Init(sourceStorage, documentationStorage);

        var features = await aiClient.ProcessAsync<FeaturesRequest, Feature[]>("api/v1/features", new FeaturesRequest
        {
            Files = (await sourceStorage.GetStructureAsync()).Select(e => e.Path).ToArray(),
            ChangedFiles = (await sourceStorage.GetDiffStructureAsync(
                    await documentationStorage.GetLatestCommitHashAsync())
                )
                .Select(e => e.Path).ToArray(),
            DocumentationStructure = (await documentationStorage.GetStructureAsync())
                .GetAllFiles().ToArray()
        });
        if (features == null)
            throw new Exception("Failed to get features");
    }
}