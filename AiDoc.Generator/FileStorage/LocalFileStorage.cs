using System;
using System.IO;
using System.IO.Compression;

namespace AiDoc.Generator.FileStorage;

public class LocalFileStorage : IFileStorage
{
    private readonly string _lastGenerationBaseCommitShaPath;
    private readonly string _internalDirectoryPath;
    private readonly string _sourcePath;
    private readonly string _docsPath;

    public LocalFileStorage(string sourcePath, string docsPath)
    {
        _sourcePath = sourcePath;
        _docsPath = docsPath;
        _internalDirectoryPath = Path.Join(_sourcePath, ".aidoc");
        _lastGenerationBaseCommitShaPath = Path.Join(_internalDirectoryPath, "lastGenerationBaseCommitSha");
    }

    public Task<Stream> CreateSourcesArchiveAsync()
    {
        Console.WriteLine("Creating archive of sources");
        return CreateFilesArchiveAsync(_sourcePath, [_docsPath]);
    }

    public async Task<Stream?> CreateDocsArchiveAsync()
    {
        Console.WriteLine("Creating archive of docs");
        return DocsExist() ? await CreateFilesArchiveAsync(_docsPath) : null;
    }

    public Task<string[]> GetChangedSourcesAsync(string baseCommitSha)
    {
        Console.WriteLine("Getting changed sources");
        return GetChangedFilesAsync(_sourcePath, baseCommitSha, [_docsPath]);
    }

    public Task<string[]> GetChangedDocsAsync(string baseCommitSha)
    {
        Console.WriteLine("Getting changed docs");
        return DocsExist() ? GetChangedFilesAsync(_docsPath, baseCommitSha) : Task.FromResult(Array.Empty<string>());
    }

    public async Task<string?> GetLastGenerationBaseCommitShaAsync()
    {
        if (!Directory.Exists(_lastGenerationBaseCommitShaPath)) return null;
        try { return await File.ReadAllTextAsync(_lastGenerationBaseCommitShaPath); }
        catch { return null; }
    }

    public async Task SaveLastGenerationBaseCommitShaAsync(string baseCommitSha)
    {
        if (!Directory.Exists(_internalDirectoryPath))
            Directory.CreateDirectory(_internalDirectoryPath);

        await File.WriteAllTextAsync(_lastGenerationBaseCommitShaPath, baseCommitSha);
    }

    public async Task ExtractDocsArchiveAsync(Stream archive)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var archivePath = Path.Combine(tempDir, "result.zip");
            using (var fileStream = File.Create(archivePath))
            {
                await archive.CopyToAsync(fileStream);
            }

            var unpackedDir = Path.Join(tempDir, "unpacked");
            Directory.CreateDirectory(unpackedDir);

            ZipFile.ExtractToDirectory(archivePath, unpackedDir, true);

            await CopyFilesIntoDocsAsync(unpackedDir);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    private Task CopyFilesIntoDocsAsync(string path)
    {
        return Task.Run(() =>
        {
            if (!DocsExist())
                Directory.CreateDirectory(_docsPath);

            foreach (var sourceFile in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(path, sourceFile);
                var targetFile = Path.Combine(_docsPath, relativePath);
                var targetFileDir = Path.GetDirectoryName(targetFile);

                if (!string.IsNullOrEmpty(targetFileDir) && !Directory.Exists(targetFileDir))
                    Directory.CreateDirectory(targetFileDir);

                File.Copy(sourceFile, targetFile, overwrite: true);
            }
        });
    }

    private static string EnsureTrailingSeparator(string path)
    {
        if (string.IsNullOrEmpty(path)) return path;
        var sep = Path.DirectorySeparatorChar;
        return path.EndsWith(sep) ? path : path + sep;
    }

    private static string[] PrepareExcludeAbs(string[]? excludePaths)
        => excludePaths == null
            ? Array.Empty<string>()
            : excludePaths
                .Where(p => !string.IsNullOrWhiteSpace(p) && Directory.Exists(p))
                .Select(p => EnsureTrailingSeparator(Path.GetFullPath(p)))
                .ToArray();

    private static bool IsUnder(string fullPath, string rootWithSep)
        => fullPath.StartsWith(rootWithSep, StringComparison.OrdinalIgnoreCase);

    private static bool IsExcluded(string fullPath, string[] excludeAbs)
        => excludeAbs.Length > 0 && excludeAbs.Any(ex => fullPath.StartsWith(ex, StringComparison.OrdinalIgnoreCase));

    private static string ToRelativeUnix(string root, string fullPath)
        => Path.GetRelativePath(root, fullPath).Replace('\\', '/');

    /// <summary>
    /// Универсальный фильтр: оставляет файлы, которые лежат в root, не попадают под exclude,
    /// и возвращает относительные пути с '/'.
    /// </summary>
    private static string[] FilterAndRelativize(IEnumerable<string> paths, string root, string[]? excludePaths = null)
    {
        var rootAbs = Path.GetFullPath(root);
        var rootSep = EnsureTrailingSeparator(rootAbs);
        var excludeAbs = PrepareExcludeAbs(excludePaths);

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var result = new List<string>();

        foreach (var original in paths)
        {
            if (string.IsNullOrWhiteSpace(original)) continue;

            var full = Path.IsPathRooted(original)
                ? Path.GetFullPath(original)
                : Path.GetFullPath(Path.Combine(rootAbs, original));

            if (!IsUnder(full, rootSep)) continue;
            if (IsExcluded(full, excludeAbs)) continue;

            var rel = ToRelativeUnix(rootAbs, full);
            if (string.IsNullOrWhiteSpace(rel) || rel == ".") continue;

            if (seen.Add(rel))
                result.Add(rel);
        }

        return result.ToArray();
    }

    private Task<string[]> GetFilesAsync(string path, string[]? excludePaths = null)
    {
        return Task.Run(() =>
        {
            var all = GitClient.GetFiles(path);
            return FilterAndRelativize(all, path, excludePaths);
        });
    }

    private Task<string[]> GetChangedFilesAsync(string path, string baseCommitSha, string[]? excludePaths = null)
    {
        return Task.Run(() =>
        {
            var changed = GitClient.GetChangedFiles(path, baseCommitSha);
            return FilterAndRelativize(changed, path, excludePaths);
        });
    }

    private async Task<Stream> CreateFilesArchiveAsync(string path, string[]? excludePaths = null)
    {
        var relFiles = await GetFilesAsync(path, excludePaths);

        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var rel in relFiles)
            {
                var full = Path.Combine(path, rel);

                if (!File.Exists(full)) continue;

                var entry = archive.CreateEntry(rel);
                Console.WriteLine($"Put file in archive: {full}, relative: {rel}");

                await using var entryStream = entry.Open();
                await using var fileStream = File.OpenRead(full);
                await fileStream.CopyToAsync(entryStream);
            }
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    private bool DocsExist() => Directory.Exists(_docsPath);
}
