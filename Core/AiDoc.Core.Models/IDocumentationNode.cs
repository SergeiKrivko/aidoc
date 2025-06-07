namespace AiDoc.Core.Models;

public interface IDocumentationNode
{
    public string Path { get; set; }
    public int Position { get; set; }
}