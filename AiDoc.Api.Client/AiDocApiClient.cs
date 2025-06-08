using System.Net.Http.Json;
using System.Text.Json;
using AiDoc.Core.Models;

namespace AiDoc.Api.Client;

public class AiDocApiClient : IAiDocApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public AiDocApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<Guid> StartProcessingAsync(Stream sourceZip, Stream docZip,
        CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StreamContent(sourceZip), "sourceZip", "source.zip");
        content.Add(new StreamContent(docZip), "docZip", "doc.zip");

        var response = await _httpClient.PostAsync("api/v1/generate", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>(cancellationToken);
    }

    public async Task<GenerationTask?> PollResultAsync(Guid processId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/v1/poll/{processId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<GenerationTask>(content, _jsonOptions);
    }

    public async Task<byte[]> DownloadFileAsync(string url, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetByteArrayAsync(url, cancellationToken);
    }
}