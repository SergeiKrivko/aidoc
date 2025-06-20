using System.Text.Json.Serialization;

namespace AiDoc.Application.Shemas;

public class UmlRequest
{
    [JsonPropertyName("code")] public required string Code { get; set; }
}