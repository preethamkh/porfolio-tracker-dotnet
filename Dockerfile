# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY PortfolioTracker.sln .
COPY src/PortfolioTracker.API/PortfolioTracker.API.csproj src/PortfolioTracker.API/
COPY src/PortfolioTracker.Core/PortfolioTracker.Core.csproj src/PortfolioTracker.Core/
COPY src/PortfolioTracker.Infrastructure/PortfolioTracker.Infrastructure.csproj src/PortfolioTracker.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish
WORKDIR /src/src/PortfolioTracker.API
RUN dotnet publish -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "PortfolioTracker.API.dll"]