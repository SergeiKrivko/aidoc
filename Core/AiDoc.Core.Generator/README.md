# AiDoc.Core.Generator

Основная библиотека для генерации документации в проекте AiDoc.

## Описание

Этот проект содержит основную логику для генерации документации и может использоваться как библиотека в других проектах. Он изолирован от пользовательского интерфейса и отвечает только за бизнес-логику.

## Основные компоненты

### Модели

- `GenerationOptions` - настройки для генерации документации
- `SourceAnalysisResult` - результат анализа исходного кода
- `DocumentationAnalysisResult` - результат анализа существующей документации
- `DirectoryConfiguration` - конфигурация директории документации

### Сервисы

- `DocumentationGeneratorService` - основной сервис для генерации документации
- `DocumentationResultApplier` - сервис для применения результата генерации
- `ConsoleLogger` - простая реализация логгера для консоли

### Интерфейсы

- `IGitClient` - интерфейс для работы с Git
- `ILogger` - интерфейс для логирования

### Адаптеры

- `GitClientAdapter` - адаптер для существующего GitClient

## Логика работы

1. **Анализ исходного кода**: Сканирование файлов проекта, исключение игнорируемых файлов, создание архива
2. **Анализ документации**: Проверка существующей документации, создание архива
3. **Генерация**: Вызов API для генерации документации с поллингом результата
4. **Применение результата**: Скачивание и применение сгенерированной документации

## Использование

```csharp
var options = new GenerationOptions
{
    SourcePath = "path/to/source",
    DocumentationPath = "path/to/docs",
    ProjectName = "MyProject",
    ApiUrl = "http://localhost:8000"
};

var generator = new DocumentationGeneratorService(options.ApiUrl);
var result = await generator.GenerateDocumentationAsync(options);
```

## Зависимости

- `AiDoc.Core.Abstractions` - базовые интерфейсы
- `AiDoc.Core.ApiClient` - клиент для API генерации
- `AiDoc.Core.Models` - базовые модели
- `AiDoc.Git` - работа с Git
