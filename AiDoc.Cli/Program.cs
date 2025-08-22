// See https://aka.ms/new-console-template for more information

using System.Reflection;
using CommandLine;
using AiDoc.Core.Generator;
using AiDoc.Core.Generator.Models;
using AiDoc.Core.Generator.Services;

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
                        var options = new GenerationOptions
                        {
                            SourcePath = opts.SourcePath ?? Directory.GetCurrentDirectory(),
                            DocumentationPath = opts.DocPath,
                            ProjectName = opts.Name,
                            ApiUrl = opts.ApiUrl
                        };

                        var generator = new DocumentationGeneratorService(options.ApiUrl);
                        var result = await generator.GenerateDocumentationAsync(options);
                        
                        if (result.Status == "done")
                        {
                            Console.WriteLine("Документация успешно сгенерирована!");
                            if (!string.IsNullOrEmpty(result.ResultDocsUrl))
                            {
                                Console.WriteLine($"Результат доступен по ссылке: {result.ResultDocsUrl}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Генерация завершилась со статусом: {result.Status}");
                            if (!string.IsNullOrEmpty(result.ErrorDescription))
                            {
                                Console.WriteLine($"Ошибка: {result.ErrorDescription}");
                            }
                        }
                        
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
                        var options = new GenerationOptions
                        {
                            SourcePath = opts.SourcePath ?? Directory.GetCurrentDirectory(),
                            DocumentationPath = opts.DocPath,
                            ProjectName = opts.Name,
                            ApiUrl = opts.ApiUrl
                        };

                        var generator = new DocumentationGeneratorService(options.ApiUrl);
                        var result = await generator.GenerateDocumentationAsync(options);
                        
                        if (result.Status == "done")
                        {
                            Console.WriteLine("Документация успешно обновлена!");
                            if (!string.IsNullOrEmpty(result.ResultDocsUrl))
                            {
                                Console.WriteLine($"Результат доступен по ссылке: {result.ResultDocsUrl}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Обновление завершилось со статусом: {result.Status}");
                            if (!string.IsNullOrEmpty(result.ErrorDescription))
                            {
                                Console.WriteLine($"Ошибка: {result.ErrorDescription}");
                            }
                        }
                        
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
                    // Не выводим ошибку, так как это нормальное поведение при отсутствии аргументов
                    return Task.FromResult(0);
                });
    }
}