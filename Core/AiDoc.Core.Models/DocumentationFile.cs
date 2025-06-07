namespace AiDoc.Core.Models;

public class DocumentationFile : IDocumentationNode
{
    public required string Path { get; set; }
    public int Position { get; set; }
}