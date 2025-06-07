using AiDoc.Application.Shemas;
using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;

namespace AiDoc.Application;

public class GenerationService(IAiClient aiClient) : IGenerationService
{
    private void Init(ISourceStorage sourceStorage, IDocumentationStorage documentationStorage)
    {
        aiClient.AddFunction<GetFileRequest, string?>("get_file",
            async r => r == null ? null : await sourceStorage.GetFileAsync(r.FilePath));
    }

    public async Task GenerateAsync(string projectName, ISourceStorage sourceStorage, IDocumentationStorage documentationStorage)
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

        var structure = new ProjectStructure
        {
            ProjectName = projectName,
            Files = (await sourceStorage.GetStructureAsync()).Select(e => e.Path).ToArray(),
        };
        var changed = new ProjectChanges
        {
            Files = (await sourceStorage.GetDiffStructureAsync(
                    await documentationStorage.GetLatestCommitHashAsync())
                )
                .Select(e => e.Path).ToArray(),
        };
        foreach (var feature in features)
        {
            var newDoc = await aiClient.ProcessAsync<GenerateDocRequest, string>("api/agent", new GenerateDocRequest
            {
                Structure = structure,
                Changed = changed,
                Feature = feature.Name,
            });
        }
    }

    private async Task<DocumentationDirectory> GenerateFeature(Feature feature, ProjectStructure structure, ProjectChanges changes,
        IDocumentationStorage documentationStorage)
    {
        throw new NotImplementedException();
    }
}