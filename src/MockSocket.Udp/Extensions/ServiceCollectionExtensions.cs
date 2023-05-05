using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockClient.Udp.Interfaces;
using MockClient.Udp.Services;
using MockSocket.Common.Interfaces;
using MockSocket.Common.Services;
using MockSocket.Udp.Config;
using MockSocket.Udp.Interfaces;
using MockSocket.Udp.Services;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUdpMockServer(this IServiceCollection services, IConfiguration config)
    {
        return services
            .Configure<MockServerConfig>(config)
            .AddTransient<IUdpClient, UdpClient>() // for mediator handle
            .AddSingleton<IMockServer, UdpMockServer>()
            .AddTransient<IUdpServer, UdpServer>()
            .AddTransient<IMemorySerializer, MemorySerializer>()
            .AddTransient<IBufferService, BufferService>()
            .AddTransient<IUdpPairService, UdpPairService>()
            .AddTransient<ICancellationTokenService, CancellationTokenService>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

    public static IServiceCollection AddUdpMockAgent(this IServiceCollection services, IConfiguration config)
    {
        return services
            .Configure<MockAgentConfig>(config)
            .AddSingleton<IMockAgent, UdpMockAgent>()
            .AddTransient<IUdpClient, UdpClient>()
            .AddTransient<IUdpServer, UdpServer>() // for mediator handle
            .AddTransient<IMemorySerializer, MemorySerializer>()
            .AddTransient<ICancellationTokenService, CancellationTokenService>()
            .AddTransient<IBufferService, BufferService>()
            .AddTransient<IUdpPairService, UdpPairService>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}
