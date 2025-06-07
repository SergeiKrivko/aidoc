using AiDoc.Application.Shemas;
using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;
using Cyrillic.Convert;
using Humanizer;

namespace AiDoc.Application;

public class GenerationService(IAiClient aiClient) : IGenerationService
{
    private void Init(ISourceStorage sourceStorage, IDocumentationStorage documentationStorage)
    {
        aiClient.AddFunction<GetFileRequest, string?>("get_file",
            async r => r == null ? null : await sourceStorage.GetFileAsync(r.FilePath));
    }

    public async Task GenerateAsync(string projectName, ISourceStorage sourceStorage,
        IDocumentationStorage documentationStorage)
    {
        Init(sourceStorage, documentationStorage);

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

        var features = await aiClient.ProcessAsync<FeaturesRequest, Feature[]>("api/agent/features", new FeaturesRequest
        {
            Structure = structure,
            Changes = changed,
        });
        if (features == null)
            throw new Exception("Failed to get features");

        foreach (var feature in features)
        {
            await GenerateFeature("", feature, structure, changed, documentationStorage);
        }
    }

    private async Task GenerateFeature(string rootPath, Feature feature, ProjectStructure structure,
        ProjectChanges changes,
        IDocumentationStorage documentationStorage)
    {
        var featurePath = Path.Join(rootPath, feature.Name.ToRussianLatin().Kebaberize());
        Console.WriteLine($"Generating feature '{featurePath}'...");
        if (feature.Children.Length == 0)
        {
            Console.WriteLine("Simple feature");
            var newDoc = await aiClient.ProcessAsync<GenerateDocRequest, string>("api/agent", new GenerateDocRequest
            {
                Structure = structure,
                Changed = changes,
                Feature = feature.Name,
            });
            await documentationStorage.PutFileAsync(new DocumentationFile
            {
                Path = featurePath,
                Content = newDoc,
                Position = 0,
            });
        }
        else
        {
            Console.WriteLine("Complex feature");
            await documentationStorage.PutDirectoryAsync(new DocumentationDirectory
            {
                Path = featurePath,
                Label = feature.Name,
                Position = 0,
                Description = feature.Name,
            });
            foreach (var child in feature.Children)
            {
                await GenerateFeature(featurePath, child, structure, changes, documentationStorage);
            }
        }
    }
}