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
                            BaseAddress = new Uri("http://localhost:5087/")
                        };
                        var apiClient = new AiDocApiClient(httpClient);
                        var processor = new DocumentProcessor(apiClient);
                        await processor.ProcessDocumentsAsync(opts);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        await Console.Error.WriteLineAsync($"Ошибка: {ex.Message}");
                        throw;
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