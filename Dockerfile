FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app
EXPOSE 9090

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /code

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY "MockSocket.sln" "MockSocket.sln"
COPY "src/MockSocket.Server/MockSocket.Server.csproj" "src/MockSocket.Server/MockSocket.Server.csproj"
COPY "src/MockSocket.Agent/MockSocket.Agent.csproj" "src/MockSocket.Agent/MockSocket.Agent.csproj"
COPY "src/MockSocket.Common/MockSocket.Common.csproj" "src/MockSocket.Common/MockSocket.Common.csproj"
COPY "src/MockSocket.Tcp/MockSocket.Tcp.csproj" "src/MockSocket.Tcp/MockSocket.Tcp.csproj"
COPY "src/MockSocket.Udp/MockSocket.Udp.csproj" "src/MockSocket.Udp/MockSocket.Udp.csproj"
COPY "src/MockClient/MockClient.csproj" "src/MockClient/MockClient.csproj"
COPY "test/MockSocket.xUnit/MockSocket.xUnit.csproj" "test/MockSocket.xUnit/MockSocket.xUnit.csproj"
COPY "benchmark/MockSocket.Benchmarks/MockSocket.Benchmarks.csproj" "benchmark/MockSocket.Benchmarks/MockSocket.Benchmarks.csproj"

# COPY "NuGet.config" "NuGet.config"
RUN dotnet restore "MockSocket.sln"

COPY . .
WORKDIR /code/src/MockSocket.Server
RUN dotnet publish -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MockSocket.Server.dll"]