FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app
EXPOSE 9090

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY "MockSocket.sln" "MockSocket.sln"

# COPY "docker-compose.dcproj" "docker-compose.dcproj"

# COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "MockSocket.sln"

COPY . .
WORKDIR /src/MockSocket.Server
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MockSocket.Server.dll"]
