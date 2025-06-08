namespace AiDoc.Core.Models;

public class GenerationTaskResult
{
    public class ResultFile
    {
        public required string Path { get; set; }
        public required string Content { get; set; }
    }

    public ResultFile[] UpdatedFiles { get; set; } = [];
    public string[] DeletedFiles { get; set; } = [];
}