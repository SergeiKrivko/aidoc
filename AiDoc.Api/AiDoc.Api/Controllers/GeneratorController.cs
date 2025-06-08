using System.IO.Compression;
using AiDoc.Api.Shemas;
using AiDoc.Application;
using AiDoc.Core.Abstractions;
using AiDoc.Core.Models;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = AiDoc.Core.Models.TaskStatus;

namespace AiDoc.Api.Controllers;

[ApiController]
[Route("/api/v1")]
public class GeneratorController(ITaskService taskService, IGenerationService generationService) : Controller
{
    [HttpPost("generate")]
    public async Task<ActionResult<Guid>> Generate(GenerateRequest request)
    {
        var id = Guid.NewGuid();
        Console.WriteLine($"Creating new task {id}");
        await taskService.AddTask(new GenerationTask
        {
            Id = id,
            CreatedAt = DateTime.Now,
            Status = TaskStatus.InProgress,
        });
        StartTask(request, id);

        return Ok(id);
    }

    private async void StartTask(GenerateRequest request, Guid taskId)
    {
        try
        {
            var sourceStorage = new ZipSourceStorage(
                new ZipArchive(request.SourceZip.OpenReadStream(), ZipArchiveMode.Read),
                request.Diff);
            var documentationStorage = await ZipDocumentationStorage.LoadAsync(request.DocZip.OpenReadStream());
            await generationService.GenerateAsync(
                request.ProjectName ?? "Unnamed project", sourceStorage, documentationStorage);
            var task = await taskService.GetTask(taskId);
            if (task != null)
            {
                task.Status = TaskStatus.Success;
                task.Result = documentationStorage.GetResult();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            try
            {
                var task = await taskService.GetTask(taskId);
                if (task != null)
                {
                    task.Status = TaskStatus.Failure;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    [HttpGet("poll/{taskId:guid}")]
    public async Task<ActionResult<GenerationTask>> Poll(Guid taskId)
    {
        Console.WriteLine($"Get task {taskId}");
        var task = await taskService.GetTask(taskId);
        if (task == null)
            return NotFound();
        return Ok(task);
    }
}