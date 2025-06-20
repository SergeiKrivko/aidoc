using System.Text.Json.Serialization;

namespace AiDoc.Application.Shemas;

public class GenerateDocRequest
{
    [JsonPropertyName("structure")] public required ProjectStructure Structure { get; set; }
    [JsonPropertyName("changed")] public required ProjectChanges Changed { get; set; }
    [JsonPropertyName("feature")] public required string Feature { get; set; }
    [JsonPropertyName("current_doc")] public string? CurrentDoc { get; set; }
}