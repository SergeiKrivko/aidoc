using System.Text.Json.Serialization;

namespace AiDoc.Core.Models;

public class AiResponseModel
{
    [JsonPropertyName("messages")] public AiMessage[] Messages { get; set; } = [];
}