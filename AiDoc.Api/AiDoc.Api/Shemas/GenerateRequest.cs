using AiDoc.Core.Models;

namespace AiDoc.Api.Shemas;

public class GenerateRequest
{
    public required IFormFile SourceZip { get; set; }
    public required IFormFile DocZip { get; set; }
    public ModifiedSourceFile[] Diff { get; set; } = [];
    public string? ProjectName { get; set; }
}