using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;

namespace AiDoc.Application;

public class TaskService : ITaskService
{
    private readonly List<GenerationTask> _tasks = [];

    public Task AddTask(GenerationTask task)
    {
        _tasks.Add(task);
        return Task.CompletedTask;
    }

    public Task<GenerationTask?> GetTask(Guid taskId)
    {
        return Task.FromResult(_tasks.FirstOrDefault(t => t.Id == taskId));
    }
}