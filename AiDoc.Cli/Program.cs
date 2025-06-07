// See https://aka.ms/new-console-template for more information

using AiDoc.Api.Client;
using CommandLine;

namespace AiDoc.Cli;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        return await Parser.Default.ParseArguments<ProcessOptions>(args)
            .MapResult(
                async (ProcessOptions opts) =>
                {
                    try
                    {
                        var httpClient = new HttpClient
                        {
                            BaseAddress = new Uri("https://example.com/")
                        };
                        var apiClient = new AiDocApiClient(httpClient);
                        var processor = new DocumentProcessor(apiClient);
                        await processor.ProcessDocumentsAsync(opts);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Ошибка: {ex.Message}");
                        return 1;
                    }
                },
                errors =>
                {
                    Console.Error.WriteLine("Ошибка при разборе аргументов командной строки");
                    return Task.FromResult(1);
                });
    }
}