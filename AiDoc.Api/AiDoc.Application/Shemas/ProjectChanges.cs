using System.Text.Json.Serialization;

namespace AiDoc.Application.Shemas;

public class ProjectChanges
{
    [JsonPropertyName("files")] public string[] Files { get; set; } = [];
}