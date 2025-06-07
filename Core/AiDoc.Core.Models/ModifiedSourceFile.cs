namespace AiDoc.Core.Models;

public class ModifiedSourceFile : SourceFile
{
    public required string ChangeType { get; set; }
}