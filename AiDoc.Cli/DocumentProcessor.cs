using AiDoc.Application;
using AiDoc.Core.Abstractions;

namespace AiDoc.Cli;

public class DocumentProcessor
{
    public async Task ProcessDocumentsAsync(IProcessOptions options, bool full)
    {
        var sourcePath = options.SourcePath ?? Directory.GetCurrentDirectory();
        var docPath = Path.Join(options.DocPath ?? Path.Join(sourcePath, "docs"), "docs");
        Directory.CreateDirectory(docPath);

        var generationService = new GenerationService(new AiClient(options.ApiUrl));

        await generationService.GenerateAsync(options.Name ?? Path.GetFileName(sourcePath),
            new LocalSourceStorage(sourcePath),
            new LocalDocumentationStorage(docPath), full);
    }
}