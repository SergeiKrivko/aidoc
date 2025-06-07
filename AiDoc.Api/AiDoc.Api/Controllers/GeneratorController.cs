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
        await taskService.AddTask(new GenerationTask
        {
            Id = id,
            CreatedAt = DateTime.Now,
            Status = TaskStatus.InProgress,
        });

        var task = generationService.GenerateAsync(
            new ZipSourceStorage(new ZipArchive(request.SourceZip.OpenReadStream(), ZipArchiveMode.Read),
                request.Diff),
            new ZipDocumentationStorage(new ZipArchive(request.DocZip.OpenReadStream(), ZipArchiveMode.Read)));

        return Ok(id);
    }

    [HttpGet("poll/{taskId:guid}")]
    public async Task<ActionResult<GenerationTask>> Poll(Guid taskId)
    {
        var task = await taskService.GetTask(taskId);
        if (task == null)
            return NotFound();
        return Ok(task);
    }
}