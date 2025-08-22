using AiDoc.Core.Generator.Interfaces;
using AiDoc.Core.ApiClient;
using System.IO.Compression;
using System.IO;

namespace AiDoc.Core.Generator.Services;

/// <summary>
/// Сервис для применения результата генерации документации
/// </summary>
public class DocumentationResultApplier
{
    private readonly ILogger _logger;
    private readonly IDocumentationApiClient _apiClient;

    public DocumentationResultApplier(ILogger logger, IDocumentationApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    /// <summary>
    /// Применяет сгенерированную документацию
    /// </summary>
    public async Task ApplyAsync(string resultDocsUrl, string documentationPath)
    {
        _logger.LogInfo($"Результат доступен по ссылке: {resultDocsUrl}");
        _logger.LogInfo($"Документация будет применена в: {documentationPath}");
        
        try
        {
            // 1. Скачивание архива с результатом
            _logger.LogInfo("Скачивание результата генерации...");
            var resultArchive = await _apiClient.DownloadDocumentationResultAsync(resultDocsUrl);
            _logger.LogInfo($"Скачано {resultArchive.Length} байт");
            
            // 2. Создание временной директории для распаковки
            var tempDir = Path.Combine(Path.GetTempPath(), $"aidoc_result_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempDir);
            
            try
            {
                // 3. Распаковка архива
                _logger.LogInfo("Распаковка архива...");
                var archivePath = Path.Combine(tempDir, "result.zip");
                await File.WriteAllBytesAsync(archivePath, resultArchive);
                
                ZipFile.ExtractToDirectory(archivePath, tempDir, true);
                _logger.LogInfo("Архив успешно распакован");
                
                // 4. Применение файлов к существующей документации
                await ApplyFilesAsync(tempDir, documentationPath);
                
                _logger.LogInfo("Результат генерации успешно применен");
            }
            finally
            {
                // Очистка временных файлов
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInfo($"Ошибка при применении результата: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Применяет файлы из распакованного архива к целевой документации
    /// </summary>
    private async Task ApplyFilesAsync(string sourceDir, string targetDir)
    {
        // Создаем целевую директорию, если она не существует
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
            _logger.LogInfo($"Создана директория: {targetDir}");
        }
        
        // Получаем все файлы из исходной директории
        var sourceFiles = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
        
        foreach (var sourceFile in sourceFiles)
        {
            var relativePath = Path.GetRelativePath(sourceDir, sourceFile);
            var targetFile = Path.Combine(targetDir, relativePath);
            var targetFileDir = Path.GetDirectoryName(targetFile);
            
            // Создаем целевую директорию, если она не существует
            if (!string.IsNullOrEmpty(targetFileDir) && !Directory.Exists(targetFileDir))
            {
                Directory.CreateDirectory(targetFileDir);
            }
            
            // Копируем файл
            await CopyFileAsync(sourceFile, targetFile);
            _logger.LogInfo($"Применен файл: {relativePath}");
        }
    }
    
    /// <summary>
    /// Копирует файл с обработкой возможных конфликтов
    /// </summary>
    private async Task CopyFileAsync(string sourcePath, string targetPath)
    {
        // Если целевой файл существует, создаем резервную копию
        if (File.Exists(targetPath))
        {
            var backupPath = $"{targetPath}.backup.{DateTime.Now:yyyyMMdd_HHmmss}";
            File.Copy(targetPath, backupPath);
            _logger.LogInfo($"Создана резервная копия: {backupPath}");
        }
        
        // Копируем новый файл
        File.Copy(sourcePath, targetPath);
    }
}
