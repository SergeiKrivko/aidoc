namespace AiDoc.Core.Models;

public enum TaskStatus
{
    InProgress,
    Success,
    Failure,
}

public class GenerationTask
{
    public required Guid Id { get; set; }
    public TaskStatus Status { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public GenerationTaskResult? Result { get; set; }
}