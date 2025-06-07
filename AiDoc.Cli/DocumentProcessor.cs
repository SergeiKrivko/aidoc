using System.IO.Compression;
using AiDoc.Api.Client;
using AiDoc.Core.Models;
using AiDoc.Git;
using TaskStatus = AiDoc.Core.Models.TaskStatus;

namespace AiDoc.Cli;

public class DocumentProcessor
{
    private readonly IAiDocApiClient _apiClient;

    public DocumentProcessor(IAiDocApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task ProcessDocumentsAsync(ProcessOptions options)
    {
        var sourcePath = options.SourcePath ?? Directory.GetCurrentDirectory();
        var docPath = options.DocPath;

        // Создаем zip-архивы
        var sourceZipPath = await CreateZipArchiveAsync(sourcePath, true);
        var docZipPath = await CreateZipArchiveAsync(docPath);

        // Отправляем запрос на обработку
        await using var sourceZipStream = File.OpenRead(sourceZipPath);
        await using var docZipStream = File.OpenRead(docZipPath);
        var processId = await _apiClient.StartProcessingAsync(sourceZipStream, docZipStream);

        if (File.Exists(sourceZipPath)) File.Delete(sourceZipPath);
        if (File.Exists(docZipPath)) File.Delete(docZipPath);

        // Ожидаем результат
        await WaitForResultAsync(processId, options);
    }

    private async Task<string> CreateZipArchiveAsync(string sourcePath, bool checkGitignore = false)
    {
        var zipPath = Path.GetTempFileName();
        if (File.Exists(zipPath)) File.Delete(zipPath);

        Directory.CreateDirectory(sourcePath);

        // Получаем список игнорируемых файлов
        var ignoredFilesSet = new HashSet<string>(checkGitignore ? GitClient.GetIgnoredFiles(sourcePath) : [],
            StringComparer.OrdinalIgnoreCase);
        Console.WriteLine(string.Join('\n', ignoredFilesSet));

        using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
        {
            foreach (var file in Directory.EnumerateFiles(sourcePath, "*.*", SearchOption.AllDirectories)
                         .Where(e => !ignoredFilesSet.Contains(Path.GetFullPath(e))))
            {
                var entry = zip.CreateEntry(Path.GetRelativePath(sourcePath, file));
                await using var fileStream = File.OpenRead(file);
                await using var entryStream = entry.Open();
                await fileStream.CopyToAsync(entryStream);
            }
        }

        return zipPath;
    }

    private async Task WaitForResultAsync(string processId, ProcessOptions options)
    {
        var startTime = DateTime.UtcNow;
        var timeout = TimeSpan.FromMilliseconds(options.Timeout);
        var pollInterval = TimeSpan.FromMilliseconds(options.PollInterval);
        var cts = new CancellationTokenSource(timeout);

        while (!cts.Token.IsCancellationRequested)
        {
            var task = await _apiClient.PollResultAsync(processId, cts.Token);
            if (task?.Status == TaskStatus.Failure)
                throw new Exception("Generation failed");
            if (task is { Status: TaskStatus.Success, Result: not null })
            {
                await ProcessResultAsync(task.Result, options.DocPath);
                return;
            }

            await Task.Delay(pollInterval, cts.Token);
        }

        throw new TimeoutException("Превышено время ожидания результата");
    }

    private async Task ProcessResultAsync(GenerationTaskResult result, string docPath)
    {
        // Удаляем указанные файлы
        foreach (var fileToDelete in result.DeletedDocFileList)
        {
            var fullPath = Path.Combine(docPath, fileToDelete);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        // Скачиваем и распаковываем обновленные документы
        var tempZipPath = Path.GetTempFileName();
        try
        {
            var zipContent = await _apiClient.DownloadFileAsync(result.UpdatedDocZip);
            await File.WriteAllBytesAsync(tempZipPath, zipContent);
            System.IO.Compression.ZipFile.ExtractToDirectory(tempZipPath, docPath, true);
        }
        finally
        {
            if (File.Exists(tempZipPath)) File.Delete(tempZipPath);
        }
    }
}