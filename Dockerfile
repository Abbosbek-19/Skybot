FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy and restore dependencies first (Docker layer caching)
COPY *.csproj nuget.config ./
RUN dotnet restore

# Copy everything else and publish
COPY . .
RUN dotnet publish -c Release -o /out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

EXPOSE 3000
CMD ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-3000} dotnet SkyBot.dll"]
