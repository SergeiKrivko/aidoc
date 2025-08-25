# AIDoc - Автоматическая генерация документации с помощью ИИ

[![CI/CD Status](https://woodpecker.dev.nachert.art/api/badges/4/status.svg)](https://woodpecker.dev.nachert.art/repos/4)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Python](https://img.shields.io/badge/Python-3.12-green.svg)](https://www.python.org/downloads/)

> **AIDoc** - это система для автоматической генерации технической документации с использованием искусственного интеллекта. Проект разработан в рамках хакатона "Внедрейд" и предоставляет CLI для разработчиков и REST API для интеграции.

## 🏗️ Архитектура проекта

Проект состоит из трех основных компонентов:

### 📱 **AiDoc.Cli** - Консольное приложение
- **Технология**: .NET 8 Console Application
- **Назначение**: Основной интерфейс для пользователей
- **Функции**: Генерация документации
- **Команды**: `generate` для создания документации

### 🔧 **AiDoc.Generator** - Основной модуль генерации со стороны клиента
- **Технология**: .NET 8 Class Library
- **Назначение**: Логика генерации документации
- **Функции**: Работа с Git, управление файлами, интеграция с API
- **Зависимости**: AiDoc.ApiClient

### 🌐 **AiDoc.ApiClient** - HTTP клиент к AIDoc API
- **Технология**: .NET 8 Class Library
- **Назначение**: Взаимодействие с REST API
- **Функции**: HTTP запросы, сериализация данных

### 🚀 **AIDoc API** - Бэкенд-сервис для генерации документации
- **Технология**: FastAPI (Python 3.12)
- **Назначение**: Обработка запросов на генерацию документации
- **Функции**: LLM интеграция, управление задачами, хранение результатов
- **Инфраструктура**: MongoDB, S3, Docker

## ✨ Основные возможности

- 🔄 **Автоматическая генерация** документации из исходного кода
- 📊 **Отслеживание изменений** через Git
- 🤖 **ИИ-анализ** кода с помощью LLM
- 🌍 **REST API** для интеграции

## 🚀 Быстрый старт

### Предварительные требования

- .NET 8.0 SDK
- Python 3.12+ (для API)
- Docker (опционально)

### Установка и запуск

#### 1. CLI приложение

```bash
# Клонирование репозитория
git clone https://github.com/SergeiKrivko/aidoc.git
cd aidoc

# Сборка проекта
dotnet build AiDoc.sln

# Запуск CLI
dotnet run --project AiDoc.Cli -- generate --help
```

#### 2. API

```bash
cd api

# Установка зависимостей
poetry install

# Поднять окружение в Docker (для MongoDB и S3)
make run-environment

# Запуск в режиме разработки
make run-api
```

### Использование CLI

```bash
# Генерация документации для текущего проекта
dotnet run --project AiDoc.Cli -- generate

# Указание путей к исходникам и документации
dotnet run --project AiDoc.Cli -- generate \
  --source-path ./src \
  --doc-path ./docs \
  --name "MyProject"

# Использование локального API
dotnet run --project AiDoc.Cli -- generate \
  --api-url http://localhost:8000
```

## 📁 Структура проекта

```
aidoc/
├── AiDoc.Cli/                 # Консольное приложение
│   ├── Program.cs            # Точка входа
│   └── Options.cs            # CLI опции
├── AiDoc.Generator/          # Библиотека генерации
│   ├── Generator.cs          # Основная логика
│   ├── GitClient.cs          # Git интеграция
│   └── FileStorage/          # Управление файлами
├── AiDoc.ApiClient/          # HTTP клиент
│   ├── AiDocApiClient.cs     # API клиент
│   └── Models/               # Модели данных
├── api/                      # Python API backend
│   ├── app/                  # FastAPI приложение
│   ├── Dockerfile            # Docker образ
│   └── pyproject.toml        # Python зависимости
├── Dockerfile                # .NET Docker образ
└── README.md                 # Документация
```

## 🧪 Разработка

### Линтинг и форматирование

```bash
cd api

# Проверка качества кода
make lint

# Форматирование кода
make format

# Полная проверка
make check
```

### Тестирование

```bash
# Запуск тестов
make test

# С покрытием кода
poetry run pytest --cov=app
```

## 📊 API Endpoints

- `POST /api/v1/documentation/generate` - Запуск генерации документации
- `GET /api/v1/documentation/{task_id}/status` - Статус задачи
- `GET /api/v1/documentation/{task_id}/result` - Результат генерации

## 🤝 Contributing

Мы приветствуем вклад в развитие проекта!

1. Форкните репозиторий
2. Создайте ветку для новой фичи/фикса
3. Внесите изменения
4. Создайте Pull Request

## 📄 Лицензия

Проект распространяется под лицензией MIT. См. файл [LICENSE](LICENSE).
