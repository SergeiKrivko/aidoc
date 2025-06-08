using System.Text.Json.Serialization;

namespace AiDoc.Application.Shemas;

public class Feature
{
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("children")] public Feature[] Children { get; set; } = [];
}