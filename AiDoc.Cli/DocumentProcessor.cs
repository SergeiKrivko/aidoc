using System.IO.Compression;
using AiDoc.Application;
using AiDoc.Core.Abstractions;

namespace AiDoc.Cli;

public class DocumentProcessor
{
    public async Task ProcessDocumentsAsync(IProcessOptions options, bool full)
    {
        var sourcePath = options.SourcePath ?? Directory.GetCurrentDirectory();
        Directory.CreateDirectory(sourcePath);
        var docPath = options.DocPath;

        var generationService = new GenerationService(new AiClient(options.ApiUrl));

        await generationService.GenerateAsync(options.Name ?? Path.GetFileName(sourcePath),
            new LocalSourceStorage(sourcePath),
            new LocalDocumentationStorage(docPath), full);
    }

    public async Task RenderStaticAsync(IProcessOptions options)
    {
        var client = new AiClient(options.ApiUrl);
        using (var stream = await client.DownloadStatic())
        {
            var docPath = options.DocPath;
            if (Directory.Exists(docPath))
            {
                Directory.Delete(docPath, true);
            }
            
            ZipFile.ExtractToDirectory(stream, docPath);
        }
    }
}