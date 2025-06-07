using AiDoc.Core.Models;

namespace AiDoc.Api.Client;

public interface IAiDocApiClient
{
    Task<string> StartProcessingAsync(Stream sourceZip, Stream docZip, CancellationToken cancellationToken = default);
    Task<GenerationTask?> PollResultAsync(string processId, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadFileAsync(string url, CancellationToken cancellationToken = default);
} 