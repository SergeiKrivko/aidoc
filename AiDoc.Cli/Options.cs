using CommandLine;

namespace AiDoc.Cli;

[Verb("generate", HelpText = "Создание документации с нуля.")]
public class GenerateOptions
{
    [Option('s', "source-path", Required = false, Default = null,
        HelpText = "Путь к исходным файлам. По умолчанию используется текущая директория")]
    public string? SourcePath { get; set; }

    [Option('d', "doc-path", Required = false,
        HelpText = "Путь к документации. По умолчанию - проект/docs")]
    public string? DocPath { get; set; }

    [Option('n', "name", Required = false, Default = null,
        HelpText = "Имя проекта. По умолчанию используется имя папки")]
    public string? Name { get; set; }

    [Option("api-url", Required = false,
        HelpText = "Url API. Можно использовать этот параметр, например, для локального тестирования")]
    public string? ApiUrl { get; set; }
}
