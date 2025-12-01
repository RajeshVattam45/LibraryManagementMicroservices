# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY BookCatalogService.API/BookCatalogService.API.csproj BookCatalogService.API/
COPY BookCatalogService.Application/BookCatalogService.Application.csproj BookCatalogService.Application/
COPY BookCatalogService.Domain/BookCatalogService.Domain.csproj BookCatalogService.Domain/
COPY BookCatalogService.Infrastructure/BookCatalogService.Infrastructure.csproj BookCatalogService.Infrastructure/

RUN dotnet restore BookCatalogService.API/BookCatalogService.API.csproj

COPY . .
WORKDIR /src/BookCatalogService.API

RUN dotnet build BookCatalogService.API.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish BookCatalogService.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookCatalogService.API.dll"]
