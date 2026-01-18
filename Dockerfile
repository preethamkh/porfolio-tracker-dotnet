# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY src/PortfolioTracker.API/PortfolioTracker.API.csproj src/PortfolioTracker.API/
COPY src/PortfolioTracker.Core/PortfolioTracker.Core.csproj src/PortfolioTracker.Core/
COPY src/PortfolioTracker.Infrastructure/PortfolioTracker.Infrastructure.csproj src/PortfolioTracker.Infrastructure/

# Restore
RUN dotnet restore src/PortfolioTracker.API/PortfolioTracker.API.csproj

# Copy source
COPY src/ src/

# Publish (CHANGED: /app/publish instead of /app/out)
RUN dotnet publish src/PortfolioTracker.API/PortfolioTracker.API.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy from publish directory
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Start the application (file is now in /app/)
ENTRYPOINT ["dotnet", "PortfolioTracker.API.dll"]