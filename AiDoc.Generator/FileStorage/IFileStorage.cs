namespace AiDoc.Generator.FileStorage;

public interface IFileStorage
{
    Task<Stream> CreateSourcesArchiveAsync();
    Task<Stream?> CreateDocsArchiveAsync();
    Task<string[]> GetChangedSourcesAsync(string baseCommitSha);
    Task<string[]> GetChangedDocsAsync(string baseCommitSha);
    Task <string?> GetLastGenerationBaseCommitShaAsync();
    Task SaveLastGenerationBaseCommitShaAsync(string baseCommitSha);
    Task ExtractDocsArchiveAsync(Stream archive);
}
