// See https://aka.ms/new-console-template for more information

using System.Reflection;
using CommandLine;

namespace AiDoc.Cli;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        return await Parser.Default.ParseArguments(args, Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray())
            .MapResult(
                async (GenerateOptions opts) =>
                {
                    try
                    {
                        var processor = new DocumentProcessor();
                        await processor.ProcessDocumentsAsync(opts, true);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        await Console.Error.WriteLineAsync($"Ошибка: {ex}");
                        return 1;
                    }
                },
                async (UpdateOptions opts) =>
                {
                    try
                    {
                        var processor = new DocumentProcessor();
                        await processor.ProcessDocumentsAsync(opts, false);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        await Console.Error.WriteLineAsync($"Ошибка: {ex}");
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