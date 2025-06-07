namespace AiDoc.Core.Models;

public class DocumentationStructure
{
    public DocumentationFile[] Files { get; set; } = [];
    public DocumentationDirectory[] Directories { get; set; } = [];
}