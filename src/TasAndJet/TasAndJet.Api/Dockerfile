FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TasAndJet.Api/TasAndJet.Api.csproj", "TasAndJet.Api/"]
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]
COPY ["TasAndJet.Infrastructure/TasAndJet.Infrastructure.csproj", "TasAndJet.Infrastructure/"]
COPY ["TasAndJet.Domain/TasAndJet.Domain.csproj", "TasAndJet.Domain/"]
COPY ["TasAndJet.Contracts/TasAndJet.Contracts.csproj", "TasAndJet.Contracts/"]
COPY ["TasAndJet.Application/TasAndJet.Application.csproj", "TasAndJet.Application/"]
RUN dotnet restore "TasAndJet.Api/TasAndJet.Api.csproj"
COPY . .
WORKDIR "/src/TasAndJet.Api"
RUN dotnet build "TasAndJet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TasAndJet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TasAndJet.Api.dll"]
