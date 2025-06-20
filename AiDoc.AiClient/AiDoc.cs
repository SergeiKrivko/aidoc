using AiDoc.Core.Abstractions;

namespace AiDoc.AiClient;

public class AiDocClient : AiDocClientBase
{
    private readonly ISourceStorage _sourceStorage;
    private readonly IDocumentationStorage _documentationStorage;

    public AiDocClient(Uri apiUri, ISourceStorage sourceStorage, IDocumentationStorage documentationStorage) :
        base(apiUri)
    {
        _sourceStorage = sourceStorage;
        _documentationStorage = documentationStorage;
    }

    protected override Task<string> GetFile(string path)
    {
        return _sourceStorage.GetFileAsync(path);
    }

    protected override async Task<string> GetFileDiff(string path)
    {
        return await _sourceStorage.GetFileDiffAsync(path,
            await _documentationStorage.GetLatestCommitHashAsync() ?? throw new InvalidOperationException());
    }
}