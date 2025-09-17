# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY backend/*.csproj ./
RUN dotnet restore

COPY backend/. ./
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Expose HTTP port
EXPOSE 5115

# Set ASPNETCORE_URLS
ENV ASPNETCORE_URLS=http://0.0.0.0:5115

ENTRYPOINT ["dotnet", "CargoGo.Api.dll"]


