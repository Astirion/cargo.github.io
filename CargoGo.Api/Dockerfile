# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY CargoGo.Api/*.csproj ./
RUN dotnet restore

COPY CargoGo.Api/. ./
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Expose HTTP port
EXPOSE 5000

# Set ASPNETCORE_URLS and disable telemetry
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV DOTNET_NOLOGO=1

ENTRYPOINT ["dotnet", "CargoGo.Api.dll"]


