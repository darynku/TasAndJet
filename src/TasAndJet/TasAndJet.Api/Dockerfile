# Используем базовый образ ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

EXPOSE 5000
EXPOSE 5001

# Используем SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TasAndJet.Api/TasAndJet.Api.csproj", "TasAndJet.Api/"]
RUN dotnet restore "TasAndJet.Api/TasAndJet.Api.csproj"
COPY . .
WORKDIR "/src/TasAndJet.Api"
RUN dotnet build "TasAndJet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публикуем приложение
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TasAndJet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный контейнер
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TasAndJet.Api.dll"]
