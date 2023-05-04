using Microsoft.Extensions.Configuration;
using MockSocket.Common.Interfaces;
using MockSocket.Common.Services;
using MockSocket.Tcp.Configurations;
using MockSocket.Tcp.Interfaces;
using MockSocket.Tcp.Services;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTcpMockServer(this IServiceCollection services, IConfiguration config)
    {
        return services
            .Configure<MockServerConfig>(config)
            .Configure<CommonConfig>(config)
            .AddSingleton<TcpClientFactory>()
            .AddTransient<IMockServer, TcpMockServer>()
            .AddTransient<ITcpServer, TcpServer>()
            .AddTransient<ITcpClient, TcpClient>()
            .AddSingleton<IBufferService, BufferService>()
            .AddSingleton<ITcpPairService, TcpPairService>()
            .AddSingleton<IMemorySerializer, MemorySerializer>()
            .AddSingleton<ICancellationTokenService, CancellationTokenService>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

    public static IServiceCollection AddTcpMockAgent(this IServiceCollection services, IConfiguration config)
    {
        return services
            .Configure<MockAgentConfig>(config)
            .Configure<CommonConfig>(config)
            .AddSingleton<TcpClientFactory>()
            .AddTransient<IMockAgent, TcpMockAgent>()
            .AddTransient<ITcpServer, TcpServer>()
            .AddTransient<ITcpClient, TcpClient>()
            .AddSingleton<IBufferService, BufferService>()
            .AddSingleton<ITcpPairService, TcpPairService>()
            .AddSingleton<IMemorySerializer, MemorySerializer>()
            .AddSingleton<ICancellationTokenService, CancellationTokenService>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}
