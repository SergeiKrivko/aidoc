namespace AiDoc.Core.Models;

public class DocumentationStructure
{
    public DocumentationFile[] Files { get; set; } = [];
    public DocumentationDirectory[] Directories { get; set; } = [];

    public IEnumerable<string> GetAllFiles()
    {
        foreach (var file in Files)
        {
            yield return file.Path;
        }

        foreach (var directory in Directories)
        {
            foreach (var file in directory.Children?.GetAllFiles() ?? [])
            {
                yield return file;
            }
        }
    }
}