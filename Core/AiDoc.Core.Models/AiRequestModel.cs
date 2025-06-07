using System.Text.Json.Serialization;

namespace AiDoc.Core.Models;

public class AiRequestModel
{
    [JsonPropertyName("messages")] public AiMessage[] Messages { get; set; } = [];
}