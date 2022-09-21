using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockSocket.Abstractions.Tcp;
using MockSocket.Core.Exchange;
using MockSocket.Core.Tcp;
using MockSocket.HoleClient;
using MockSocket.HoleServer;

namespace MockSocket.Forward
{
    public class TcpForwardServer : IForwardServer
    {
        private readonly ClientOptions options;
        private readonly ITcpServerConnection tcpServer;
        private readonly TcpClientConnectionFactory tcpClientConnectionFactory;
        private readonly IExchangeConnection exchangeConnection;
        private readonly ILogger<TcpForwardServer> logger;

        public TcpForwardServer(IOptions<ClientOptions> options, ITcpServerConnection tcpServer, TcpClientConnectionFactory tcpClientConnectionFactory, IExchangeConnection exchangeConnection, ILogger<TcpForwardServer> logger)
        {
            this.options = options.Value;
            this.tcpServer = tcpServer;
            this.tcpClientConnectionFactory = tcpClientConnectionFactory;
            this.exchangeConnection = exchangeConnection;
            this.logger = logger;
        }

        public async ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            await tcpServer.ListenAsync(options.HoleAppServerPort);

            logger.LogInformation($"Proxy start forward: {options.HoleAppServerPort}");

            while (true)
            {
                var client = await tcpServer.AcceptAsync(cancellationToken);

                _ = SwapAsync(client);
            }
        }

        private async ValueTask SwapAsync(ITcpConnection client)
        {
            var proxy = await tcpClientConnectionFactory.CreateAsync(options.AgentRealServerEP);

            await exchangeConnection.ExchangeAsync(client, proxy);
        }
    }
}
