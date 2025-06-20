using System.Text.Json.Serialization;

namespace AiDoc.Application.Shemas;

public class ProjectStructure
{
    [JsonPropertyName("name")] public required string ProjectName { get; set; }
    [JsonPropertyName("files")] public string[] Files { get; set; } = [];
}