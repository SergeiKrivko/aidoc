namespace AiDoc.Core.Models;

public class SourceNode
{
    public required string Path { get; set; }
    public required bool IsDirectory { get; set; }
    public SourceNode[] Children { get; set; } = [];
}