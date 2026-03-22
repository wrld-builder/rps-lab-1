# Build and run the Lab 1 console app with Docker (no local .NET SDK required).
# Run interactively:  docker build -t circle-intersection . && docker run -it --rm circle-intersection
# Run tests:          docker build -t circle-intersection-test -f Dockerfile.test . && docker run --rm circle-intersection-test

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src

COPY CircleIntersection.sln ./
COPY src/ src/
COPY tests/ tests/

RUN dotnet restore CircleIntersection.sln \
    && dotnet publish src/CircleIntersection.App/CircleIntersection.App.csproj \
        -c Release \
        -o /app/publish \
        --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0-bookworm-slim AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CircleIntersection.App.dll"]
