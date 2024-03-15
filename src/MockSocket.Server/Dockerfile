#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MockSocket.Server/MockSocket.Server.csproj", "src/MockSocket.Server/"]
COPY ["src/MockSocket.Tcp/MockSocket.Tcp.csproj", "src/MockSocket.Tcp/"]
COPY ["src/MockSocket.Common/MockSocket.Common.csproj", "src/MockSocket.Common/"]
COPY ["src/MockSocket.Udp/MockSocket.Udp.csproj", "src/MockSocket.Udp/"]
RUN dotnet restore "./src/MockSocket.Server/MockSocket.Server.csproj"
COPY . .
WORKDIR "/src/src/MockSocket.Server"
RUN dotnet build "./MockSocket.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MockSocket.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MockSocket.Server.dll"]