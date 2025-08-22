namespace AiDoc.Core.Generator.Models;

/// <summary>
/// Результат анализа исходного кода
/// </summary>
public class SourceAnalysisResult
{
    /// <summary>
    /// Список файлов исходного кода (относительные пути)
    /// </summary>
    public required List<string> SourceFiles { get; set; }
    
    /// <summary>
    /// Архив с исходным кодом
    /// </summary>
    public required byte[] SourcesArchive { get; set; }
    
    /// <summary>
    /// Список измененных исходных файлов
    /// </summary>
    public required List<string> ChangedSources { get; set; }
}
