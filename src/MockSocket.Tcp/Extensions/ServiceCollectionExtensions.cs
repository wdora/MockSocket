using MockSocket.Common.Interfaces;
using MockSocket.Common.Services;
using MockSocket.Tcp.Interfaces;
using MockSocket.Tcp.Services;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTcpMockServer(this IServiceCollection services)
    {
        return services
            .AddSingleton<TcpClientFactory>()
            .AddTransient<IMockServer, TcpMockServer>()
            .AddTransient<ITcpServer, TcpServer>()
            .AddTransient<ITcpClient, TcpClient>()
            .AddTransient<IBufferService, BufferService>()
            .AddTransient<ITcpPairService, TcpPairService>()
            .AddTransient<IMemorySerializer, MemorySerializer>()
            .AddTransient<ICancellationTokenService, CancellationTokenService>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

    public static IServiceCollection AddTcpMockAgent(this IServiceCollection services)
    {
        return services
            .AddSingleton<TcpClientFactory>()
            .AddTransient<IMockAgent, TcpMockAgent>()
            .AddTransient<ITcpServer, TcpServer>()
            .AddTransient<ITcpClient, TcpClient>()
            .AddTransient<IBufferService, BufferService>()
            .AddTransient<ITcpPairService, TcpPairService>()
            .AddTransient<IMemorySerializer, MemorySerializer>()
            .AddTransient<ICancellationTokenService, CancellationTokenService>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}
