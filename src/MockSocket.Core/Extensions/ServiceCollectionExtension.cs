using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockSocket.Core.Configurations;
using MockSocket.Core.Interfaces;
using MockSocket.Core.Services;
using MockSocket.Server;

namespace MockSocket.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAgent(this IServiceCollection services, IConfiguration config)
        {
            return services
                .Configure<MockAgentConfig>(config.GetSection("MockSocket"))
                .AddSingleton<ITcpPairService, TcpPairService>()
                .AddSingleton<ILimitIPService, LimitIPService>()
                .AddSingleton<IEncodeService, JsonEncodeService>();
        }

        public static IServiceCollection AddMockServer(this IServiceCollection services, IConfiguration config)
        {
            return services
                .Configure<MockServerConfig>(config.GetSection("MockSocket"))
                .AddSingleton<ITcpPairService, TcpPairService>()
                .AddTransient<IMockTcpClient, MockTcpClient>()
                .AddTransient<IMockTcpServer, MockTcpServer>()
                .AddTransient<IUdpServer, MockUdpServer>()
                .AddTransient<IMemorySerializer, MemorySerializer>()
                .AddTransient<IBufferService, BufferService>()
                .AddSingleton<IEncodeService, JsonEncodeService>();
        }
    }
}
