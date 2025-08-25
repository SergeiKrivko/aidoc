namespace AiDoc.Generator.FileStorage;

public interface IFileStorage
{
    Task<byte[]> CreateSourcesArchiveAsync();
    Task<byte[]?> CreateDocsArchiveAsync();
    Task<string[]> GetChangedSourcesAsync(string baseCommitSha);
    Task<string[]> GetChangedDocsAsync(string baseCommitSha);
    Task <string?> GetLastGenerationBaseCommitShaAsync();
    Task SaveLastGenerationBaseCommitShaAsync(string baseCommitSha);
    Task ExtractDocsArchiveAsync(byte[] archive);
}
