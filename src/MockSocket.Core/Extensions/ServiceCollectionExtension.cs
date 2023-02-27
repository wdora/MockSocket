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
        public static IServiceCollection AddAgent(this IServiceCollection services, IConfigurationSection config)
        {
            return services
                .Configure<MockAgentConfig>(config)
                .AddSingleton<IPairService, PairService>()
                .AddTransient<IMockTcpClient, MockTcpClient>()
                .AddSingleton<ILimitIPService, LimitIPService>()
                .AddSingleton<IEncodeService, JsonEncodeService>();
        }

        public static IServiceCollection AddMockServer(this IServiceCollection services, IConfigurationSection config)
        {
            return services
                .Configure<MockServerConfig>(config)
                .AddSingleton<IPairService, PairService>()
                .AddTransient<IMockTcpClient, MockTcpClient>()
                .AddTransient<IMockTcpServer, MockTcpServer>()
                .AddSingleton<IEncodeService, JsonEncodeService>();
        }
    }
}
