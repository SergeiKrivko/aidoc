using System.Text.Json.Serialization;

namespace AiDoc.Application.Shemas;

public class FeaturesRequest
{
    [JsonPropertyName("structure")] public required ProjectStructure Structure { get; set; }
    [JsonPropertyName("changed")] public required ProjectChanges Changes { get; set; }
}