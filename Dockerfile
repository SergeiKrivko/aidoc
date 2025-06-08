FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AiDoc.Cli/AiDoc.Cli.csproj", "AiDoc.Cli/"]
COPY ["AiDoc.Git/AiDoc.Git.csproj", "AiDoc.Git/"]
COPY ["Core/AiDoc.Core.Models/AiDoc.Core.Models.csproj", "Core/AiDoc.Core.Models/"]
COPY ["Core/AiDoc.Core.Abstractions/AiDoc.Core.Abstractions.csproj", "Core/AiDoc.Core.Abstractions/"]
COPY ["AiDoc.Api/AiDoc.Application/AiDoc.Application.csproj", "AiDoc.Api/AiDoc.Application/"]
RUN dotnet restore "AiDoc.Cli/AiDoc.Cli.csproj"
COPY . .
WORKDIR "/src/AiDoc.Cli"
RUN dotnet build "AiDoc.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AiDoc.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AiDoc.Cli.dll"]
