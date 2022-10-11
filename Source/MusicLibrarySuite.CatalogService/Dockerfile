FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR "/src"
COPY ["MusicLibrarySuite.CatalogService/MusicLibrarySuite.CatalogService.csproj", "MusicLibrarySuite.CatalogService/"]
COPY ["MusicLibrarySuite.CatalogService.Core/MusicLibrarySuite.CatalogService.Core.csproj", "MusicLibrarySuite.CatalogService.Core/"]
COPY ["MusicLibrarySuite.CatalogService.Interfaces/MusicLibrarySuite.CatalogService.Interfaces.csproj", "MusicLibrarySuite.CatalogService.Interfaces/"]
RUN dotnet restore "MusicLibrarySuite.CatalogService/MusicLibrarySuite.CatalogService.csproj"
COPY "." "."

WORKDIR "/src/MusicLibrarySuite.CatalogService"
RUN dotnet build "MusicLibrarySuite.CatalogService.csproj" -c Release -o "/app/build" --no-restore

FROM build AS publish

RUN dotnet publish "MusicLibrarySuite.CatalogService.csproj" -c Release -o "/app/publish" --no-restore /p:UseAppHost=false

FROM base AS final

WORKDIR "/app"
COPY --from=publish "/app/publish" "."
ENTRYPOINT ["dotnet", "MusicLibrarySuite.CatalogService.dll"]
