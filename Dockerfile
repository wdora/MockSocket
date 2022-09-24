FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS base
WORKDIR /app
EXPOSE 9090

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY "MockSocket.sln" "MockSocket.sln"
COPY "src/MockSocket.Server/MockSocket.Server.csproj" "src/MockSocket.Server/MockSocket.Server.csproj"
COPY "src/MockSocket.Agent/MockSocket.Agent.csproj" "src/MockSocket.Agent/MockSocket.Agent.csproj"
COPY "src/MockSocket/MockSocket.csproj" "src/MockSocket/MockSocket.csproj"
COPY "test/MockSocket.xUnit/MockSocket.xUnit.csproj" "test/MockSocket.xUnit/MockSocket.xUnit.csproj"
COPY "benchmark/MockSocket.Benchmarks/MockSocket.Benchmarks.csproj" "benchmark/MockSocket.Benchmarks/MockSocket.Benchmarks.csproj"

# COPY "NuGet.config" "NuGet.config"
RUN dotnet restore "MockSocket.sln"

COPY ./src .
WORKDIR /src/MockSocket.Server
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MockSocket.Server.dll"]