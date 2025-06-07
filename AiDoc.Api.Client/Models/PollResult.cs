namespace AiDoc.Api.Client.Models;

public class PollResult
{
    public string UpdatedDocZip { get; set; } = null!;
    public string[] DeletedDocFileList { get; set; } = Array.Empty<string>();
} 