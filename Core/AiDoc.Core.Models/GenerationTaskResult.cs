namespace AiDoc.Core.Models;

public class GenerationTaskResult
{
    public string UpdatedDocZip { get; set; } = null!;
    public string[] DeletedDocFileList { get; set; } = [];
} 