using AiDoc.Core.ApiClient;
using AiDoc.Core.ApiClient.Models;
using AiDoc.Core.Generator.Models;
using AiDoc.Core.Generator.Interfaces;
using AiDoc.Core.Generator.Adapters;
using System.IO.Compression;

namespace AiDoc.Core.Generator.Services;

/// <summary>
/// Основной сервис для генерации документации
/// </summary>
public class DocumentationGeneratorService
{
    private readonly IDocumentationApiClient _apiClient;
    private readonly IGitClient _gitClient;
    private readonly ILogger _logger;
    private readonly DocumentationResultApplier _resultApplier;

    public DocumentationGeneratorService(string? apiUrl = null, ILogger? logger = null)
    {
        _apiClient = new DocumentationApiClient(apiUrl);
        _gitClient = new GitClientAdapter();
        _logger = logger ?? new ConsoleLogger();
        _resultApplier = new DocumentationResultApplier(_logger, _apiClient);
    }

    /// <summary>
    /// Генерирует документацию на основе указанных настроек
    /// </summary>
    public async Task<DocumentationGenerationResult> GenerateDocumentationAsync(GenerationOptions options)
    {
        var baseCommitSha = await GetBaseCommitForComparisonAsync(options.SourcePath, options.BaseCommitSha);

        var sourceAnalysis = await AnalyzeSourceCodeAsync(options.SourcePath, baseCommitSha);
        var docAnalysis = await AnalyzeDocumentationAsync(options.GetDocumentationPath(), baseCommitSha);

        var request = new DocumentationGenerationRequest
        {
            ApplicationName = options.GetProjectName(),
            GithubRepoUrl = options.GithubRepoUrl ?? "https://github.com/SergeiKrivko/aidoc",
            ChangedSources = sourceAnalysis.ChangedSources,
            ChangedDocs = docAnalysis.ChangedDocs,
            SourcesArchive = sourceAnalysis.SourcesArchive,
            DocsArchive = docAnalysis.Exists ? docAnalysis.DocumentationArchive : null
        };

        var taskId = await _apiClient.StartDocumentationGenerationAsync(request);
        var result = await _apiClient.WaitForDocumentationCompletionAsync(taskId);

        if (result.IsSuccess && !string.IsNullOrEmpty(result.ResultDocsUrl))
        {
            await _resultApplier.ApplyAsync(result.ResultDocsUrl, options.GetDocumentationPath());
        }

        return result;
    }

    /// <summary>
    /// Получает базовый коммит для сравнения изменений
    /// </summary>
    private async Task<string> GetBaseCommitForComparisonAsync(string sourcePath, string? baseCommitSha)
    {
        if (!string.IsNullOrEmpty(baseCommitSha))
        {
            return baseCommitSha;
        }

        try
        {
            // Если базовый коммит не указан, пытаемся использовать последний тег
            var lastTag = _gitClient.GetLastTag(sourcePath);
            if (!string.IsNullOrEmpty(lastTag))
            {
                var tagCommit = _gitClient.GetCommitByTag(sourcePath, lastTag);
                if (!string.IsNullOrEmpty(tagCommit))
                {
                    _logger.LogInfo($"Используется последний тег '{lastTag}' ({tagCommit}) как базовый коммит");
                    return tagCommit;
                }
            }
            
            // Если тег не найден, используем текущий коммит
            var currentCommit = await _gitClient.GetCurrentCommit(sourcePath);
            _logger.LogWarning($"Тег не найден, используется текущий коммит {currentCommit}. Все файлы будут считаться измененными.");
            return currentCommit;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Не удалось получить базовый коммит: {ex.Message}. Будет использован текущий коммит.");
            return await _gitClient.GetCurrentCommit(sourcePath);
        }
    }

    /// <summary>
    /// Анализирует исходный код проекта
    /// </summary>
    private async Task<SourceAnalysisResult> AnalyzeSourceCodeAsync(string sourcePath, string? baseCommitSha)
    {
        var sourceFiles = _gitClient.GetAllFiles(sourcePath).ToList();
        var sourcesArchive = await CreateArchiveAsync(sourcePath, sourceFiles);
        var changedSources = await GetChangedSourcesAsync(sourcePath, baseCommitSha);

        return new SourceAnalysisResult
        {
            SourceFiles = sourceFiles,
            SourcesArchive = sourcesArchive,
            ChangedSources = changedSources
        };
    }

    /// <summary>
    /// Анализирует существующую документацию
    /// </summary>
    private async Task<DocumentationAnalysisResult> AnalyzeDocumentationAsync(string docPath, string? baseCommitSha)
    {
        if (!Directory.Exists(docPath))
        {
            return new DocumentationAnalysisResult
            {
                DocumentationFiles = new List<string>(),
                DocumentationArchive = new byte[0],
                ChangedDocs = new List<string>()
            };
        }

        var docFiles = _gitClient.GetAllFiles(docPath).ToList();
        var docArchive = await CreateArchiveAsync(docPath, docFiles);
        var changedDocs = await GetChangedDocsAsync(docPath, baseCommitSha);

        return new DocumentationAnalysisResult
        {
            DocumentationFiles = docFiles,
            DocumentationArchive = docArchive,
            ChangedDocs = changedDocs
        };
    }

    /// <summary>
    /// Создает архив из указанных файлов
    /// </summary>
    private async Task<byte[]> CreateArchiveAsync(string basePath, List<string> files)
    {
        using var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);

        foreach (var file in files)
        {
            var fullPath = Path.Join(basePath, file);
            if (File.Exists(fullPath))
            {
                var entry = archive.CreateEntry(file);
                using var entryStream = entry.Open();
                using var fileStream = File.OpenRead(fullPath);
                await fileStream.CopyToAsync(entryStream);
            }
        }

        return memoryStream.ToArray();
    }

    /// <summary>
    /// Получает список измененных исходных файлов
    /// </summary>
    private async Task<List<string>> GetChangedSourcesAsync(string sourcePath, string? baseCommitSha)
    {
        if (string.IsNullOrEmpty(baseCommitSha))
        {
            _logger.LogWarning("Базовый коммит не установлен, возвращаем все файлы");
            return _gitClient.GetAllFiles(sourcePath).ToList();
        }

        try
        {
            var changedFiles = _gitClient.GetChangedFiles(sourcePath, baseCommitSha).ToList();
            _logger.LogInfo($"Найдено {changedFiles.Count} измененных исходных файлов");
            return changedFiles;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Не удалось получить измененные файлы через git: {ex.Message}. Возвращаем все файлы.");
            return _gitClient.GetAllFiles(sourcePath).ToList();
        }
    }

    /// <summary>
    /// Получает список измененных файлов документации
    /// </summary>
    private async Task<List<string>> GetChangedDocsAsync(string docPath, string? baseCommitSha)
    {
        if (!Directory.Exists(docPath))
            return new List<string>();

        if (string.IsNullOrEmpty(baseCommitSha))
        {
            _logger.LogWarning("Базовый коммит не установлен, возвращаем все файлы документации");
            return _gitClient.GetAllFiles(docPath).ToList();
        }

        try
        {
            var changedFiles = _gitClient.GetChangedFiles(docPath, baseCommitSha).ToList();
            _logger.LogInfo($"Найдено {changedFiles.Count} измененных файлов документации");
            return changedFiles;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Не удалось получить измененные файлы документации через git: {ex.Message}. Возвращаем все файлы.");
            return _gitClient.GetAllFiles(docPath).ToList();
        }
    }
}
