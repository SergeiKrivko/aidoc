using AiDoc.Core.Models;

namespace AiDoc.Core.Abstractions;

public interface ITaskService
{
    public Task AddTask(GenerationTask task);
    public Task<GenerationTask?> GetTask(Guid taskId);
}