using System.Text.Json.Serialization;

namespace AiDoc.Application.Shemas;

public class GetFileRequest
{
    [JsonPropertyName("file_path")] public required string FilePath { get; set; }
}