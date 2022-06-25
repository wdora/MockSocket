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
                .PostConfigure<HoleClientOptions>(options => options.HoleServerEP = new DnsEndPoint(options.HoleServer, options.HoleServerPort))
                .PostConfigure<HoleClientOptions>(options => options.AgentRealServerEP = new DnsEndPoint(options.RealServer, options.RealServerPort))
                .AddMediatR(typeof(ServiceCollectionExtension))
                .AddTransient<IHoleClient, TcpHoleClient>()
                .AddTransient<ITcpClientConnection, TcpClientConnection>()
                .AddTransient<IExchangeConnection, ExchangeConnection>()
                .AddTransient<TcpClientConnectionFactory>();
        }
    }

    public class HoleClientOptions
    {
        public string HoleServer { get; set; } = IPAddress.Loopback.ToString();

        public int HoleServerPort { get; set; } = 9090;

        public int HoleAppServerPort { get; set; } = 8080;

        public string RealServer { get; set; } = IPAddress.Loopback.ToString();

        public int RealServerPort { get; set; } = 80;

        public TimeSpan HeartInterval { get; set; } = TimeSpan.FromSeconds(60);

        public EndPoint HoleServerEP { get; internal set; } = default!;

        public EndPoint AgentRealServerEP { get; internal set; } = default!;
    }
}
