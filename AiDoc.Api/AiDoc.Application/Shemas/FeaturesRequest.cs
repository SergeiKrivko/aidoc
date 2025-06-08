using System.Text.Json.Serialization;
using AiDoc.Core.Models;

namespace AiDoc.Application.Shemas;

public class FeaturesRequest
{
    [JsonPropertyName("structure")] public required ProjectStructure Structure { get; set; }
    [JsonPropertyName("changed")] public required ProjectChanges Changes { get; set; }
    [JsonPropertyName("documentation")] public required DocumentationFile[] Documentation { get; set; }
}