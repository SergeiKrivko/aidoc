using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using AiDoc.ApiClient.Models;

namespace AiDoc.ApiClient;

public class AiDocApiClient : IAiDocApiClient
{
    private readonly HttpClient _httpClient;

    public AiDocApiClient(string? apiUrl = null)
    {
        _httpClient = new HttpClient
        {
            BaseAddress =
                new Uri(apiUrl ?? Environment.GetEnvironmentVariable("API_URL") ?? "https://aidoc-api.nachert.art")
        };
    }

    public AiDocApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> StartDocumentationGenerationAsync(DocumentationGenerationRequest request)
    {
        var apiRequest = request.ToApiRequest();

        using var multipartContent = new MultipartFormDataContent();

        var infoJson = JsonSerializer.Serialize(apiRequest.Info);
        multipartContent.Add(new StringContent(infoJson), "info");

        var sourcesContent = new ByteArrayContent(apiRequest.Sources);
        multipartContent.Add(sourcesContent, "sources", "sources.zip");

        if (apiRequest.Docs != null)
        {
            var docsContent = new ByteArrayContent(apiRequest.Docs);
            multipartContent.Add(docsContent, "docs", "docs.zip");
        }

        var response = await _httpClient.PostAsync("/api/v1/documentation", multipartContent);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DocCreateResponse>()
                     ?? throw new Exception("Failed to parse response");

        return result.Data.Id;
    }

    public async Task<DocumentationGenerationResult> GetDocumentationStatusAsync(string taskId)
    {
        var response = await _httpClient.GetAsync($"/api/v1/documentation/{taskId}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DocReadResponse>()
                     ?? throw new Exception("Failed to parse response");

        return new DocumentationGenerationResult
        {
            TaskId = result.Data.Id,
            Status = result.Data.Status,
            ErrorDescription = result.Data.ErrorDescription,
            OriginalSourcesUrl = result.Data.OriginalSourcesUrl,
            OriginalDocsUrl = result.Data.OriginalDocsUrl,
            ResultDocsUrl = result.Data.ResultDocsUrl
        };
    }

    public async Task<DocumentationGenerationResult> WaitForDocumentationCompletionAsync(
        string taskId,
        int pollingIntervalMs = 5000,
        int timeoutMs = 3600000,
        CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;

        while (true)
        {
            if ((DateTime.UtcNow - startTime).TotalMilliseconds > timeoutMs)
            {
                throw new TimeoutException($"Documentation generation timed out after {timeoutMs}ms");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var status = await GetDocumentationStatusAsync(taskId);

            if (status.IsCompleted)
            {
                return status;
            }

            await Task.Delay(pollingIntervalMs, cancellationToken);
        }
    }

    public async Task<byte[]> DownloadDocumentationResultAsync(string resultDocsUrl)
    {
        if (string.IsNullOrEmpty(resultDocsUrl))
        {
            throw new ArgumentException("URL результата не может быть пустым", nameof(resultDocsUrl));
        }

        try
        {
            var response = await _httpClient.GetAsync(resultDocsUrl);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Ошибка при скачивании результата документации: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}