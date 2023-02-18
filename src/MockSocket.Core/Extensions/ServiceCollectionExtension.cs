using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockSocket.Core.Configurations;
using MockSocket.Core.Services;

namespace MockSocket.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddAgent(this IServiceCollection services, IConfigurationSection config)
        {
            services.Configure<MockAgentConfig>(config);

            return services
                .AddSingleton<IPairService, PairService>()
                .AddSingleton<IEncodeService, JsonEncodeService>();
        }
    }
}
