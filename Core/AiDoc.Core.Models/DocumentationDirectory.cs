namespace AiDoc.Core.Models;

public class DocumentationDirectory : IDocumentationNode
{
    public required string Path { get; set; }
    public required string Label  { get; set; }
    public required string Description  { get; set; }
    public int Position { get; set; }
}