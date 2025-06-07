using CommandLine;

namespace AiDoc.Cli;

[Verb("process", HelpText = "Обработка документов с помощью AI")]
public class ProcessOptions
{
    [Option('s', "source-path", Required = false, Default = null,
        HelpText = "Путь к исходным файлам. По умолчанию используется текущая директория")]
    public string? SourcePath { get; set; }

    [Option('d', "doc-path", Required = true,
        HelpText = "Путь к документам для обработки")]
    public string DocPath { get; set; } = null!;

    [Option('p', "poll-interval", Required = false, Default = 5000,
        HelpText = "Интервал опроса статуса в миллисекундах")]
    public int PollInterval { get; set; }

    [Option('t', "timeout", Required = false, Default = 3600000,
        HelpText = "Таймаут ожидания результата в миллисекундах")]
    public int Timeout { get; set; }
} 