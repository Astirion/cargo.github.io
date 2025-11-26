# 1. Сборка бэка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем .sln и проекты
COPY *.sln ./
COPY CargoGo.Api/*.csproj ./CargoGo.Api/
COPY CargoGo.Dal/*.csproj ./CargoGo.Dal/
COPY CargoGo.Auth/*.csproj ./CargoGo.Auth/
COPY CargoGo.Services/*.csproj ./CargoGo.Services/

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем всё
COPY . .

# Публикуем бэк
RUN dotnet publish CargoGo.Api/CargoGo.Api.csproj -c Release -o /app/publish

# 2. Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Копируем опубликованный бэк
COPY --from=build /app/publish .

# Копируем фронт в wwwroot (ASP.NET автоматически отдаст статику)
COPY frontend/ ./wwwroot/

# Порт
EXPOSE 80

# Запуск
ENTRYPOINT ["dotnet", "CargoGo.Api.dll"]