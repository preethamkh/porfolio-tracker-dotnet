# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files (NOT the solution file)
COPY src/PortfolioTracker.API/PortfolioTracker.API.csproj src/PortfolioTracker.API/
COPY src/PortfolioTracker.Core/PortfolioTracker.Core.csproj src/PortfolioTracker.Core/
COPY src/PortfolioTracker.Infrastructure/PortfolioTracker.Infrastructure.csproj src/PortfolioTracker.Infrastructure/

# Restore just the API project (it will restore dependencies too)
RUN dotnet restore src/PortfolioTracker.API/PortfolioTracker.API.csproj

# Copy source code
COPY src/ src/

# Build and publish
RUN dotnet publish src/PortfolioTracker.API/PortfolioTracker.API.csproj -c Release -o /app/out --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "PortfolioTracker.API.dll"]