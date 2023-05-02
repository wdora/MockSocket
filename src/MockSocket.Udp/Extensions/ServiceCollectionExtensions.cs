using Microsoft.Extensions.DependencyInjection;
using MockClient.Udp.Interfaces;
using MockClient.Udp.Services;
using MockSocket.Common.Interfaces;
using MockSocket.Common.Services;
using MockSocket.Udp.Interfaces;
using MockSocket.Udp.Services;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUdpMockServer(this IServiceCollection services)
    {
        return services
            .AddSingleton<IMockServer, UdpMockServer>()
            .AddTransient<IUdpServer, UdpServer>()
            .AddTransient<IMemorySerializer, MemorySerializer>()
            .AddTransient<IBufferService, BufferService>()
            .AddTransient<ICancellationTokenService, CancellationTokenService>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

    public static IServiceCollection AddUdpMockAgent(this IServiceCollection services)
    {
        return services
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
