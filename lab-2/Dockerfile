# Build and run the Lab 2 console app with Docker (no local .NET SDK required).
# Run interactively:  docker compose run --rm app
# Run tests:          docker compose run --rm test

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src

COPY MaxSumSubmatrix.sln ./
COPY src/ src/
COPY tests/ tests/

RUN dotnet restore MaxSumSubmatrix.sln \
    && dotnet publish src/MaxSumSubmatrix.App/MaxSumSubmatrix.App.csproj \
        -c Release \
        -o /app/publish \
        --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0-bookworm-slim AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MaxSumSubmatrix.App.dll"]
