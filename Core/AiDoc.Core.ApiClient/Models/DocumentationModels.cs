using System.Text.Json.Serialization;

namespace AiDoc.Core.ApiClient.Models;

public class AppInfo
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("github_repo")]
    public required string GithubRepo { get; set; }
}

public class DocInfo
{
    [JsonPropertyName("application_info")]
    public required AppInfo ApplicationInfo { get; set; }
    
    [JsonPropertyName("changed_sources")]
    public required List<string> ChangedSources { get; set; }
    
    [JsonPropertyName("changed_docs")]
    public required List<string> ChangedDocs { get; set; }
}

public class DocCreateRequest
{
    public required DocInfo Info { get; set; }
    public required byte[] Sources { get; set; }
    public byte[]? Docs { get; set; }
}

public class DocCreationStatus
{
    public const string Progress = "progress";
    public const string Done = "done";
    public const string Failed = "failed";
}

public class DocRead
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("status")]
    public required string Status { get; set; }
    
    [JsonPropertyName("info")]
    public required DocInfo Info { get; set; }
    
    [JsonPropertyName("original_sources_url")]
    public string? OriginalSourcesUrl { get; set; }
    
    [JsonPropertyName("original_docs_url")]
    public string? OriginalDocsUrl { get; set; }
    
    [JsonPropertyName("result_docs_url")]
    public string? ResultDocsUrl { get; set; }
    
    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; set; }
}

public class DocCreateResponse
{
    [JsonPropertyName("data")]
    public required DocRead Data { get; set; }
    
    [JsonPropertyName("detail")]
    public string Detail { get; set; } = "Documentation generation started.";
}

public class DocReadResponse
{
    [JsonPropertyName("data")]
    public required DocRead Data { get; set; }
    
    [JsonPropertyName("detail")]
    public string Detail { get; set; } = "Documentation was selected.";
}
