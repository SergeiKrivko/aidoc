FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AiDoc.Api/AiDoc.Api/AiDoc.Api.csproj", "AiDoc.Api/AiDoc.Api/"]
COPY ["AiDoc.Api/AiDoc.Application/AiDoc.Application.csproj", "AiDoc.Api/AiDoc.Application/"]
COPY ["Core/AiDoc.Core.Abstractions/AiDoc.Core.Abstractions.csproj", "Core/AiDoc.Core.Abstractions/"]
COPY ["Core/AiDoc.Core.Models/AiDoc.Core.Models.csproj", "Core/AiDoc.Core.Models/"]
RUN dotnet restore "AiDoc.Api/AiDoc.Api/AiDoc.Api.csproj"
COPY . .
WORKDIR "/src/AiDoc.Api/AiDoc.Api"
RUN dotnet build "AiDoc.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AiDoc.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AiDoc.Api.dll"]
