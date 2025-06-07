namespace AiDoc.Application.Shemas;

public class FeaturesRequest
{
    public string[] Files { get; set; } = [];
    public string[] ChangedFiles { get; set; } = [];
    public string[] DocumentationStructure { get; set; } = [];
}