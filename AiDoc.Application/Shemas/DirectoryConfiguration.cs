using System.Text.Json.Serialization;

namespace AiDoc.Application.Shemas;

public class DirectoryConfiguration
{
    [JsonPropertyName("label")] public string Label { get; set; } = "";
    [JsonPropertyName("position")] public int Position { get; set; } = 0;
}