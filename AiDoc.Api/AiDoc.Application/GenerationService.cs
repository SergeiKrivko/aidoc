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
        IDocumentationStorage documentationStorage, bool full = true)
    {
        Init(sourceStorage, documentationStorage);

        var structure = new ProjectStructure
        {
            ProjectName = projectName,
            Files = (await sourceStorage.GetStructureAsync()).Select(e => e.Path).ToArray(),
        };
        var changed = new ProjectChanges
        {
            Files = full
                ? []
                : (await sourceStorage.GetDiffStructureAsync(await documentationStorage.GetLatestCommitHashAsync())
                )
                .Select(e => e.Path).ToArray(),
        };

        var features = await aiClient.ProcessAsync<FeaturesRequest, Feature[]>("api/agent/features", new FeaturesRequest
        {
            Structure = structure,
            Changes = changed,
            Documentation = (await documentationStorage.GetStructureAsync()).Files.ToArray()
        });
        if (features == null)
            throw new Exception("Failed to get features");

        await Task.WhenAll(features.Select(async feature => 
            await GenerateFeature("", feature, structure, changed, documentationStorage)));

        await GenerateUml(documentationStorage, structure);
    }

    private static string GenerateFeatureName(string name)
    {
        foreach (var c in new char[] { ';', ':', '/', '\\', '?', '!' })
        {
            name = name.Replace(c, '-');
        }

        return name.ToRussianLatin().Kebaberize();
    }

    private async Task GenerateFeature(string rootPath, Feature feature, ProjectStructure structure,
        ProjectChanges changes,
        IDocumentationStorage documentationStorage)
    {
        var featurePath = Path.Join(rootPath, GenerateFeatureName(feature.Name));
        Console.WriteLine($"Generating feature '{featurePath}'...");
        if (feature.Children.Length == 0)
        {
            var newDoc = await aiClient.ProcessAsync("api/agent", new GenerateDocRequest
            {
                Structure = structure,
                Changed = changes,
                Feature = feature.Name,
            });
            await documentationStorage.PutFileAsync(new DocumentationFile
            {
                Path = featurePath + ".md",
                Content = newDoc,
                Position = 0,
            });
        }
        else
        {
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

    private async Task GenerateUml(IDocumentationStorage documentationStorage, ProjectStructure structure)
    {
        try
        {
            var uml = await aiClient.ProcessAsync("api/agent/uml", structure);
            if (uml == null)
                return;
            await documentationStorage.PutFileAsync(new DocumentationFile
            {
                Path = "uml.md",
                Content = "# UML диаграмма классов" +
                          "\n\n![UML](uml.png)",
                Position = 100,
            });
            await using var stream = await aiClient.GenerateUml(uml);
            await documentationStorage.PutFileAsync("uml.png", stream);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}