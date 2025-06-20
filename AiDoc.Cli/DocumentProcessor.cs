using System.IO.Compression;
using AiDoc.Application;
using AiDoc.Git;

namespace AiDoc.Cli;

public class DocumentProcessor
{
    public async Task ProcessDocumentsAsync(IProcessOptions options, bool full)
    {
        var sourcePath = options.SourcePath ?? Directory.GetCurrentDirectory();
        var docusaurusPath = options.DocPath ?? Path.Join(sourcePath, "docs");
        var docPath = Path.Join(docusaurusPath, "docs");
        Directory.CreateDirectory(docPath);

        var generationService = new GenerationService(new AiClient(options.ApiUrl));

        var sourceService = new LocalSourceStorage(sourcePath, docusaurusPath);
        var documentationService = new LocalDocumentationStorage(docPath);

        await generationService.GenerateAsync(options.Name ?? Path.GetFileName(sourcePath),
            sourceService, documentationService, full);

        await documentationService.SetLatestCommitHashAsync(await GitClient.GetCurrentCommit(sourcePath));
    }

    public async Task RenderStaticAsync(IProcessOptions options)
    {
        var client = new AiClient(options.ApiUrl);
        using (var stream =
               await client.DownloadStatic(options.Name ??
                                           Path.GetFileName(options.SourcePath ?? Directory.GetCurrentDirectory())))
        {
            var docPath = options.DocPath ?? Path.Join(options.SourcePath ?? Directory.GetCurrentDirectory(), "docs");
            if (Directory.Exists(docPath))
            {
                Directory.Delete(docPath, true);
            }

            ZipFile.ExtractToDirectory(stream, docPath);
        }
    }
}