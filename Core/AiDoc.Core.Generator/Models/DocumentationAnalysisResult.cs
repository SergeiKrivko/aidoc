namespace AiDoc.Core.Generator.Models;

/// <summary>
/// Результат анализа существующей документации
/// </summary>
public class DocumentationAnalysisResult
{
    /// <summary>
    /// Список файлов документации (относительные пути)
    /// </summary>
    public required List<string> DocumentationFiles { get; set; }
    
    /// <summary>
    /// Архив с документацией
    /// </summary>
    public required byte[] DocumentationArchive { get; set; }
    
    /// <summary>
    /// Список измененных файлов документации
    /// </summary>
    public required List<string> ChangedDocs { get; set; }
    
    /// <summary>
    /// Существует ли документация
    /// </summary>
    public bool Exists => DocumentationFiles.Count > 0;
}
