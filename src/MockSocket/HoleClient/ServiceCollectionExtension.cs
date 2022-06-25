using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockSocket.Connection.Tcp;
using MockSocket.Core.Exchange;
using MockSocket.Core.Tcp;
using System.Net;

namespace MockSocket.HoleClient
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddHoleClient(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<HoleClientOptions>(configuration)
                .AddMediatR(typeof(ServiceCollectionExtension))
                .AddTransient<IHoleClient, TcpHoleClient>()
                .AddTransient<ITcpClientConnection, TcpClientConnection>()
                .AddTransient<IExchangeConnection, ExchangeConnection>()
                .AddTransient<TcpClientConnectionFactory>();
        }
    }

    public class HoleClientOptions
    {
        public IPEndPoint HoleServerEP { get; set; } = new IPEndPoint(IPAddress.Loopback, 10000);

        public int HoleAppServerPort { get; set; } = 8081;

        public IPEndPoint AgentRealServerEP { get; set; } = new IPEndPoint(IPAddress.Loopback, 9090);

        public TimeSpan HeartInterval { get; internal set; } = TimeSpan.FromSeconds(60);
    }
}
