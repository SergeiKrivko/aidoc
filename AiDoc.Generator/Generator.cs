using AiDoc.ApiClient;
using AiDoc.ApiClient.Models;
using AiDoc.Generator.FileStorage;

namespace AiDoc.Generator;

public class DocumentationGenerator(IAiDocApiClient apiClient)
{
    public DocumentationGenerator() : this(new AiDocApiClient())
    {
    }

    public DocumentationGenerator(HttpClient httpClient) : this(new AiDocApiClient(httpClient))
    {
    }

    public DocumentationGenerator(string? apiUrl = null) : this(new AiDocApiClient(apiUrl))
    {
    }

    public async Task<DocumentationGenerationResult> GenerateDocumentationAsync(GenerationOptions options)
    {
        var fileStorage = new LocalFileStorage(options.GetSourcesPath(), options.GetDocsPath());
        var baseCommitSha = options.BaseCommitSha ?? await fileStorage.GetLastGenerationBaseCommitShaAsync();
        Console.WriteLine($"Base commit sha: {baseCommitSha}");

        var request = new DocumentationGenerationRequest
        {
            ApplicationName = options.GetProjectName(),
            GithubRepoUrl = options.GithubRepoUrl ?? "https://github.com/SergeiKrivko/aidoc",
            ChangedSources = string.IsNullOrEmpty(baseCommitSha)
                ? []
                : await fileStorage.GetChangedSourcesAsync(baseCommitSha),
            ChangedDocs = string.IsNullOrEmpty(baseCommitSha)
                ? []
                : await fileStorage.GetChangedDocsAsync(baseCommitSha),
            SourcesArchive = await fileStorage.CreateSourcesArchiveAsync(),
            DocsArchive = await fileStorage.CreateDocsArchiveAsync()
        };

        Console.WriteLine("Changed sources:");
        foreach (var file in request.ChangedSources)
        {
            Console.WriteLine(file);
        }
        
        Console.WriteLine("Changed docs:");
        foreach (var file in request.ChangedSources)
        {
            Console.WriteLine(file);
        }

        var taskId = await apiClient.StartDocumentationGenerationAsync(request);
        var result = await apiClient.WaitForDocumentationCompletionAsync(taskId);

        if (result.IsSuccess && !string.IsNullOrEmpty(result.ResultDocsUrl))
        {
            var resultArchive = await apiClient.DownloadDocumentationResultAsync(result.ResultDocsUrl);
            await fileStorage.ExtractDocsArchiveAsync(resultArchive);

            if (!string.IsNullOrEmpty(baseCommitSha))
            {
                await fileStorage.SaveLastGenerationBaseCommitShaAsync(baseCommitSha);
            }
        }

        return result;
    }
}