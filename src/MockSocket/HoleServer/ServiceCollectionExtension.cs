using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockSocket.Cache;
using MockSocket.Connection.Tcp;
using MockSocket.Core.Exchange;
using MockSocket.Core.Tcp;

namespace MockSocket.HoleServer
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddHoleServer(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddMediatR(typeof(ServiceCollectionExtension))
                .AddTransient<IHoleServer, TcpHoleServer>()
                .Configure<HoleServerOption>(configuration)
                .AddTransient<ITcpServerConnection, TcpServerConnection>()
                .AddSingleton<ICacheService, InMemoryCacheService>()
                .AddSingleton<IExchangeConnection, ExchangeConnection>();

        }
    }
}
