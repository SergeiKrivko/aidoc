using System.Text.Json.Serialization;

namespace AiDoc.Core.Models;

public class FunctionCall
{
    [JsonPropertyName("name")] public required string Name { get; init; }

    [JsonPropertyName("arguments")] public required string Arguments { get; init; }
}