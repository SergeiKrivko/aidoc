using System.Text.Json;
using AiDoc.Api.Client.Models;

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

    public async Task<string> StartProcessingAsync(Stream sourceZip, Stream docZip, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        
        content.Add(new StreamContent(sourceZip), "sourceZip", "source.zip");
        content.Add(new StreamContent(docZip), "docZip", "doc.zip");

        var response = await _httpClient.PostAsync("api/start", content, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<PollResult?> PollResultAsync(string processId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/poll?processId={processId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<PollResult>(content, _jsonOptions);
    }

    public async Task<byte[]> DownloadFileAsync(string url, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetByteArrayAsync(url, cancellationToken);
    }
} 