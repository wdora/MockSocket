using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MockSocket.Connection.Tcp;
using MockSocket.Core.Exchange;
using MockSocket.Core.Tcp;
using MockSocket.Forward;
using System.Net;

namespace MockSocket.HoleClient
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddHoleClient(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .Configure<ClientOptions>(configuration)
                .PostConfigure<ClientOptions>(options => options.HoleServerEP = new DnsEndPoint(options.HoleServer, options.HoleServerPort))
                .PostConfigure<ClientOptions>(options => options.AgentRealServerEP = new DnsEndPoint(options.RealServer, options.RealServerPort))
                .AddMediatR(typeof(ServiceCollectionExtension))
                .AddTransient<ITcpServerConnection, TcpServerConnection>()
                .AddTransient<IForwardServer, TcpForwardServer>()
                .AddTransient<IHoleClient, TcpHoleClient>()
                .AddTransient<ITcpClientConnection, TcpClientConnection>()
                .AddTransient<IExchangeConnection, ExchangeConnection>()
#if DEBUG
                .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Debug))
#endif
                .AddTransient<TcpClientConnectionFactory>();
        }
    }

    public class ClientOptions
    {
        public string HoleServer { get; set; } = IPAddress.Loopback.ToString();

        public int HoleServerPort { get; set; } = 9090;

        public int HoleAppServerPort { get; set; } = 8080;

        public string RealServer { get; set; } = IPAddress.Loopback.ToString();

        public int RealServerPort { get; set; } = 80;

        public TimeSpan HeartInterval { get; set; } = TimeSpan.FromSeconds(60);

        public EndPoint HoleServerEP { get; internal set; } = default!;

        public EndPoint AgentRealServerEP { get; internal set; } = default!;

        public ClientType ClientType { get; set; }
    }

    public enum ClientType
    {
        Agent,
        Proxy
    }
}
