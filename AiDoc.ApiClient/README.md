# AiDoc.Core.ApiClient

Клиент для работы с Python API генерации документации.

## Описание

Этот проект содержит клиент для взаимодействия с Python API генерации документации. Клиент предоставляет удобный интерфейс для:

- Начала генерации документации
- Отслеживания статуса генерации
- Ожидания завершения с автоматическим поллингом

## Основные компоненты

### Модели данных

#### `DocumentationGenerationRequest`
Удобная модель для начала генерации документации:

```csharp
var request = new DocumentationGenerationRequest
{
    ApplicationName = "MyApplication",
    GithubRepoUrl = "https://github.com/username/myapp",
    ChangedSources = new List<string> { "src/Program.cs", "src/Models/User.cs" },
    ChangedDocs = new List<string> { "docs/README.md" },
    SourcesArchive = sourcesZipBytes,
    DocsArchive = docsZipBytes
};
```

#### `DocumentationGenerationResult`
Результат генерации с удобными свойствами:

```csharp
if (result.IsCompleted)
{
    if (result.IsSuccess)
    {
        Console.WriteLine($"Документация готова: {result.ResultDocsUrl}");
    }
    else if (result.IsFailed)
    {
        Console.WriteLine($"Ошибка: {result.ErrorDescription}");
    }
}
```

### Интерфейс `IDocumentationApiClient`

```csharp
public interface IDocumentationApiClient
{
    Task<string> StartDocumentationGenerationAsync(DocumentationGenerationRequest request);
    Task<DocumentationGenerationResult> GetDocumentationStatusAsync(string taskId);
    Task<DocumentationGenerationResult> WaitForDocumentationCompletionAsync(
        string taskId, 
        int pollingIntervalMs = 5000, 
        int timeoutMs = 300000, 
        CancellationToken cancellationToken = default);
}
```

### Реализация `DocumentationApiClient`

```csharp
var client = new DocumentationApiClient("http://localhost:8000");

// Начинаем генерацию
var taskId = await client.StartDocumentationGenerationAsync(request);

// Ждем завершения
var result = await client.WaitForDocumentationCompletionAsync(taskId);
```

## Использование

### 1. Создание клиента

```csharp
// С URL по умолчанию (http://localhost:8000)
var client = new DocumentationApiClient();

// С кастомным URL
var client = new DocumentationApiClient("http://my-api-server:8000");

// С переменной окружения API_URL
var client = new DocumentationApiClient(); // Использует API_URL
```

### 2. Генерация документации

```csharp
var request = new DocumentationGenerationRequest
{
    ApplicationName = "MyApp",
    ChangedSources = new List<string> { "src/**/*.cs" },
    ChangedDocs = new List<string> { "docs/**/*.md" },
    SourcesArchive = await CreateSourcesZipAsync(),
    DocsArchive = await CreateDocsZipAsync()
};

try
{
    var taskId = await client.StartDocumentationGenerationAsync(request);
    var result = await client.WaitForDocumentationCompletionAsync(taskId);
    
    if (result.IsSuccess)
    {
        Console.WriteLine("Документация готова!");
    }
}
catch (TimeoutException)
{
    Console.WriteLine("Превышен таймаут");
}
```

### 3. Ручной контроль

```csharp
var taskId = await client.StartDocumentationGenerationAsync(request);

// Ручной поллинг
while (true)
{
    var status = await client.GetDocumentationStatusAsync(taskId);
    
    if (status.IsCompleted)
    {
        if (status.IsSuccess)
        {
            Console.WriteLine("Готово!");
            break;
        }
        else
        {
            throw new Exception($"Ошибка: {status.ErrorDescription}");
        }
    }
    
    Console.WriteLine("В процессе...");
    await Task.Delay(5000);
}
```

## Конфигурация

### Параметры поллинга

- **pollingIntervalMs**: Интервал между запросами статуса (по умолчанию 5000ms)
- **timeoutMs**: Общий таймаут ожидания (по умолчанию 300000ms = 5 минут)
- **cancellationToken**: Токен для отмены операции

### Переменные окружения

- `API_URL` - URL API сервера (по умолчанию: http://localhost:8000)

## Обработка ошибок

### Таймаут
```csharp
try
{
    var result = await client.WaitForDocumentationCompletionAsync(taskId, timeoutMs: 60000);
}
catch (TimeoutException)
{
    Console.WriteLine("Генерация не завершилась за 1 минуту");
}
```

### Отмена операции
```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

try
{
    var result = await client.WaitForDocumentationCompletionAsync(
        taskId, 
        cancellationToken: cts.Token
    );
}
catch (OperationCanceledException)
{
    Console.WriteLine("Операция отменена");
}
```

## Внутренняя работа

### Multipart запрос
Клиент автоматически создает multipart/form-data запрос со следующими полями:
- `info`: JSON строка с информацией о проекте
- `sources`: ZIP архив с исходными файлами
- `docs`: ZIP архив с документацией (опционально)

### Поллинг
Реализован через цикл с задержкой:
1. Запрос статуса
2. Проверка завершения
3. Задержка перед следующим запросом
4. Проверка таймаута и отмены

## Зависимости

- .NET 8.0+
- System.Text.Json
- System.Net.Http

## Лицензия

См. основной файл [LICENSE](../../LICENSE).
