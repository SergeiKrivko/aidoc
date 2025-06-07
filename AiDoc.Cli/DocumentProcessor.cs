using AiDoc.Api.Client;
using AiDoc.Api.Client.Models;
using AiDoc.Git;

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

        // Создаем временные файлы для zip-архивов
        var sourceZipPath = Path.GetTempFileName();
        var docZipPath = Path.GetTempFileName();

        try
        {
            // Создаем zip-архивы
            await CreateZipArchiveAsync(sourcePath, sourceZipPath);
            await CreateZipArchiveAsync(docPath, docZipPath);

            // Отправляем запрос на обработку
            using var sourceZipStream = File.OpenRead(sourceZipPath);
            using var docZipStream = File.OpenRead(docZipPath);
            var processId = await _apiClient.StartProcessingAsync(sourceZipStream, docZipStream);
            
            // Ожидаем результат
            await WaitForResultAsync(processId, options);
        }
        finally
        {
            // Удаляем временные файлы
            if (File.Exists(sourceZipPath)) File.Delete(sourceZipPath);
            if (File.Exists(docZipPath)) File.Delete(docZipPath);
        }
    }

    private async Task CreateZipArchiveAsync(string sourcePath, string zipPath)
    {
        if (File.Exists(zipPath)) File.Delete(zipPath);

        // Получаем список игнорируемых файлов
        var ignoredFiles = GitClient.GetIgnoredFiles(sourcePath);
        var ignoredFilesSet = new HashSet<string>(ignoredFiles, StringComparer.OrdinalIgnoreCase);
        // Console.WriteLine($"Ignored files: {string.Join("\n", ignoredFilesSet)}");

        // Создаем временную директорию для фильтрации файлов
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Копируем все файлы, кроме игнорируемых
            foreach (var file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(sourcePath, file);
                if (ignoredFilesSet.Contains(file))
                {
                    continue;
                }

                var targetPath = Path.Combine(tempDir, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                File.Copy(file, targetPath);
            }

            // Создаем zip-архив из отфильтрованных файлов
            System.IO.Compression.ZipFile.CreateFromDirectory(tempDir, zipPath);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    private async Task WaitForResultAsync(string processId, ProcessOptions options)
    {
        var startTime = DateTime.UtcNow;
        var timeout = TimeSpan.FromMilliseconds(options.Timeout);
        var pollInterval = TimeSpan.FromMilliseconds(options.PollInterval);
        var cts = new CancellationTokenSource(timeout);

        while (!cts.Token.IsCancellationRequested)
        {
            var result = await _apiClient.PollResultAsync(processId, cts.Token);
            if (result != null)
            {
                await ProcessResultAsync(result, options.DocPath);
                return;
            }

            await Task.Delay(pollInterval, cts.Token);
        }

        throw new TimeoutException("Превышено время ожидания результата");
    }

    private async Task ProcessResultAsync(PollResult result, string docPath)
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
